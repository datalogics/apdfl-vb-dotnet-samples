Imports System
Imports Datalogics.PDFL




''' This sample shows how to add a Collection to a PDF document to turn that document into a PDF Portfolio.
''' 
''' A PDF Portfolio can hold And display multiple additional files as attachments.
'''  
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.

Namespace AddCollection
    Class AddCollection
        Shared Sub Main(args As String())
            Console.WriteLine("AddCollection Sample:")

            Using library As New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/Attachments.pdf"
                Dim sOutput As String = "Portfolio.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                Dim doc As New Document(sInput)

                Console.WriteLine("Input file: " & sInput & ". Writing to " & sOutput)

                ' Check if document already has collection
                Dim collection As Collection = doc.Collection

                ' if document doesn't have collection create it.
                If collection Is Nothing Then
                    doc.CreateCollection()
                    collection = doc.Collection
                End If

                ' Create a couple of schema fields
                Dim field As New CollectionSchemaField("Description", SchemaFieldSubtype.Description)
                field.Name = "DescriptionField"
                field.Index = 0
                field.Visible = True
                field.Editable = False

                Dim field1 As New CollectionSchemaField("Number", SchemaFieldSubtype.Number)
                field1.Name = "NumberField"
                field1.Index = 1
                field1.Visible = True
                field1.Editable = True

                ' Retrieve schema from collection.
                Dim schema As CollectionSchema = collection.Schema

                ' Add fields to the obtained schema.
                schema.AddField(field)
                schema.AddField(field1)

                ' Create sort collection.
                ' Each element of the array is a name that identifies a field
                ' described in the parent collection dictionary.
                ' The array form is used to allow additional fields to contribute
                ' to the sort, where each additional field is used to break ties.
                Dim colSort As New System.Collections.Generic.List(Of CollectionSortItem)()
                colSort.Add(New CollectionSortItem("Description", False))
                colSort.Add(New CollectionSortItem("Number", True))

                ' Set sort array to the collection
                collection.Sort = colSort

                ' Set view mode
                collection.ChangeCollectionViewMode(CollectionViewType.Detail, CollectionSplitType.SplitPreview)

                Dim fieldsCount As Integer = schema.FieldsNumber
                For i As Integer = 0 To fieldsCount - 1
                    Dim fld As CollectionSchemaField = schema.GetField(i)
                    Console.WriteLine("Name: " & fld.Name & " Index:" & fld.Index)
                Next

                For Each item As CollectionSortItem In collection.Sort
                    Console.WriteLine("Sort item name: " & item.Name & " Order:" & item.Ascending)
                Next

                doc.Save(SaveFlags.Full, sOutput)
            End Using
        End Sub
    End Class
End Namespace

