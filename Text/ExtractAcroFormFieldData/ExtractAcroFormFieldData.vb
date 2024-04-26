Imports System
Imports System.Text.Json
Imports Datalogics.PDFL

'
' This sample extracts text from the AcroForm fields in a PDF
' document And saves the text to a file.
'
' Copyright (c) 2022-2024, Datalogics, Inc. All rights reserved.
'
'
Namespace ExtractAcroFormFieldData
    Module ExtractAcroFormFieldData
        Public sInput As String = Library.ResourceDirectory + "Sample_Input/ExtractAcroFormFieldData.pdf"
        Public sOutput As String = "ExtractAcroFormFieldData-out.json"
        Sub Print(t As ExtractTextNameSpace.AcroFormTextFieldObject)
            Console.WriteLine("AcroForm Field Name > " + t.AcroFormFieldName)
            Console.WriteLine("AcroForm Field Text > " + t.AcroFormFieldText)
        End Sub
        Sub Main(args As String())
            Console.WriteLine("ExtractAcroFormFieldData Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")
                Using doc As Document = New Document(sInput)
                    Console.WriteLine("Input file: " + sInput)

                    Using docFormText As ExtractTextNameSpace.ExtractText = New ExtractTextNameSpace.ExtractText(doc)
                        Dim result As List(Of ExtractTextNameSpace.AcroFormTextFieldObject) = docFormText.GetAcroFormFieldData()

                        ' Print the output to the console
                        For Each item In result
                            Console.WriteLine(item)
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
