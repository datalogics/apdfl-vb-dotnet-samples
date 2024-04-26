Imports System
Imports Datalogics.PDFL




''' This sample adds an Optional Content Group (a layer) to a PDF document and
''' then adds an image to that layer. 
''' 
''' The related ChangeLayerConfiguration program makes layers visible or invisible.
''' 
''' You can toggle back and forth to make the layer (the duck image) visible or invisible
''' in the PDF file.
'''
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.

Namespace CreateLayer
    Class CreateLayer
        Shared Sub Main(args As String())
            Console.WriteLine("CreateLayer Sample:")

            Using library As New Library()
                Console.WriteLine("Initialized the library.")

                ' Define input and output file paths
                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/ducky.pdf"
                Dim sOutput As String = "CreateLayer-out.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                If args.Length > 1 Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Input file: " & sInput & ", writing to " & sOutput)

                Using doc As New Document(sInput)
                    Console.WriteLine("Opened a document.")

                    Dim pg As Page = doc.GetPage(0)
                    Dim element As Element = pg.Content.GetElement(0)

                    If TypeOf element Is Image Then
                        Dim img As Image = DirectCast(element, Image)

                        ' Containers, Forms and Annotations can be attached to an
                        ' OptionalContentGroup; other content (like Image) can
                        ' be made optional by placing it inside a Container
                        Dim container As New Container()
                        container.Content = New Content()
                        container.Content.AddElement(img)

                        ' We replace the Image with the Container
                        ' (which now holds the image)
                        pg.Content.RemoveElement(0)
                        pg.UpdateContent()

                        pg.Content.AddElement(container)
                        pg.UpdateContent()

                        ' We create a new OptionalContentGroup and place it in the
                        ' OptionalContentConfig.Order array
                        Dim ocg As OptionalContentGroup = CreateNewOptionalContentGroup(doc, "Rubber Ducky")

                        ' Now we associate the Container with the OptionalContentGroup
                        ' via an OptionalContentMembershipDict.  Note that we MUST
                        ' update the Page's content afterwards.
                        AssociateOCGWithContainer(doc, ocg, container)
                        pg.UpdateContent()

                        doc.Save(SaveFlags.Full, sOutput)
                    End If
                End Using
            End Using
        End Sub

        ' Create an OptionalContentGroup with a given name, and add it to the
        ' default OptionalContentConfig's Order array.
        Public Shared Function CreateNewOptionalContentGroup(doc As Document, name As String) As OptionalContentGroup
            ' Create an OptionalContentGroup
            Dim ocg As New OptionalContentGroup(doc, name)

            ' Add it to the Order array -- this is required so that the OptionalContentGroup
            ' will appear in the 'Layers' control panel in a PDF Viewer.  It will appear in
            ' the control panel with the name given in the OptionalContentGroup constructor.
            Dim order_list As OptionalContentOrderArray = doc.DefaultOptionalContentConfig.Order
            order_list.Insert(order_list.Length, New OptionalContentOrderLeaf(ocg))

            Return ocg
        End Function

        ' Associate a Container with an OptionalContentGroup via an OptionalContentMembershipDict.
        ' This function associates a Container with a single OptionalContentGroup and uses
        ' a VisibilityPolicy of AnyOn.
        Public Shared Sub AssociateOCGWithContainer(doc As Document, ocg As OptionalContentGroup, cont As Container)
            ' Create an OptionalContentMembershipDict.  The options here are appropriate for a
            ' 'typical' usage; other options can be used to create an 'inverting' layer
            ' (i.e. 'Display this content when the layer is turned OFF'), or to make the
            ' Container's visibility depend on several OptionalContentGroups
            Dim ocmd As New OptionalContentMembershipDict(doc, New OptionalContentGroup() {ocg}, VisibilityPolicy.AnyOn)

            ' Associate the Container with the OptionalContentMembershipDict
            cont.OptionalContentMembershipDict = ocmd
        End Sub
    End Class
End Namespace
