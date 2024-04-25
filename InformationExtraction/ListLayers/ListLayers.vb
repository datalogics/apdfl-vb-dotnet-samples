Imports System
Imports Datalogics.PDFL

'
' This sample searches for And lists the names of the color layers found in a PDF document.
'  
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'

Namespace ListLayers
    Module ListLayers
        Sub Main(args As String())
            Console.WriteLine("ListLayers Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/Layers.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                Console.WriteLine("Input file: " + sInput)

                Dim doc As Document = New Document(sInput)

                Dim ocgs As IList(Of OptionalContentGroup) = doc.OptionalContentGroups
                For Each ocg As OptionalContentGroup In ocgs
                    Console.WriteLine(ocg.Name)
                    Console.Write("  Intent: [")
                    If ocg.Intent.Count > 0 Then
                        Dim i As IEnumerator(Of String) = ocg.Intent.GetEnumerator()
                        i.MoveNext()
                        Console.Write(i.Current)
                        While i.MoveNext()
                            Console.Write(", ")
                            Console.Write(i.Current)
                        End While
                    End If

                    Console.WriteLine("]")
                Next

                Dim ctx As OptionalContentContext = doc.OptionalContentContext
                Console.Write("Optional content states: [")
                Dim states As IList(Of Boolean) = ctx.GetOCGStates(ocgs)
                If states.Count > 0 Then
                    Dim i As IEnumerator(Of Boolean) = states.GetEnumerator()
                    i.MoveNext()
                    Console.Write(i.Current)
                    While i.MoveNext()
                        Console.Write(", ")
                        Console.Write(i.Current)
                    End While
                End If

                Console.WriteLine("]")
            End Using
        End Sub
    End Module
End Namespace
