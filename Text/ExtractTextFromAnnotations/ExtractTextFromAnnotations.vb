Imports System.Text.Json
Imports Datalogics.PDFL

'
'
' This sample extracts text from the Annotations in a PDF
' document and saves the text to a file.
'
' Copyright (c) 2022-2024, Datalogics, Inc. All rights reserved.
'
'
Namespace ExtractTextFromAnnotations
    Module ExtractTextFromAnnotations
        ' Set Defaults
        Dim sInput As String = Library.ResourceDirectory + "Sample_Input/sample_annotations.pdf"
        Dim sOutput As String = "ExtractTextFromAnnotations-out.json"

        Sub Print(t As ExtractTextNameSpace.AnnotationTextObject)
            Console.WriteLine("Annotation Type > " + t.AnnotationType)
            Console.WriteLine("Annotation Text > " + t.AnnotationText)
        End Sub

        Sub Main(args As String())
            Console.WriteLine("Annotations Sample:")

            Using (New Library())
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/sample_annotations.pdf"

                Console.WriteLine("Input file: " + sInput)

                Using doc As Document = New Document(sInput)
                    Using docAnnotText As ExtractTextNameSpace.ExtractText = New ExtractTextNameSpace.ExtractText(doc)
                        Dim result As List(Of ExtractTextNameSpace.AnnotationTextObject) = docAnnotText.GetAnnotationText()

                        For Each textObject As ExtractTextNameSpace.AnnotationTextObject In result
                            ' Print the output to the console
                            ExtractTextFromAnnotations.Print(textObject)
                        Next

                        ' Save the output to a JSON file.
                        Console.WriteLine("Writing JSON to " + sOutput)
                        Dim options As JsonSerializerOptions = New JsonSerializerOptions()
                        options.WriteIndented = True
                        Dim json As String = JsonSerializer.Serialize(result, options)
                        System.IO.File.WriteAllText(sOutput, json)
                    End Using
                End Using
            End Using
        End Sub
    End Module
End Namespace
