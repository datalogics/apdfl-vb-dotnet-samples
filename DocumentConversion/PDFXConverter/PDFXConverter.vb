Imports System
Imports Datalogics.PDFL

 '
 '
 ' This sample shows how to convert a PDF document into a PDF/X compliant version of that file.
 '
 ' PDF/X is used for graphics exchange when printing content. It is a version of the PDF format that guarantees accuracy in colors used.
 '
 ' The sample takes a default input and output a document (both optional). 
 '
 ' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
 '
 '
Namespace PDFXConverter
    Module PDFXConverter
        Sub Main(args As String())
            Console.WriteLine("PDFXConverter Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/sample.pdf"
                Dim sOutput As String = "../PDFXConverter-out.pdf"

                If (args.Length > 0) Then
                    sInput = args(0)
                End If

                If (args.Length > 1) Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Input file: " + sInput + ". Writing to output " + sOutput)

                Using doc As Document = New Document(sInput)
                    ' Make a conversion parameters object
                    Dim pdfxParams As PDFXConvertParams = New PDFXConvertParams()

                    ' Create a PDF/X compliant version of the document
                    Dim pdfxResult As PDFXConvertResult = doc.CloneAsPDFXDocument(PDFXConvertType.X4, pdfxParams)

                    ' The conversion may have failed, we must check if the result has a valid Document.
                    If (pdfxResult.PDFXDocument Is Nothing) Then
                        Console.WriteLine("ERROR: Could not convert " + sInput + " to PDF/X.")
                    Else
                        Console.WriteLine("Successfully converted " + sInput + " to PDF/X.")
                    End If

                    Dim pdfxDoc As Document = pdfxResult.PDFXDocument

                    ' Set the SaveFlags returned in the PDFConvertResult.
                    pdfxDoc.Save(pdfxResult.PDFXSaveFlags, sOutput)
                End Using
            End Using
        End Sub
    End Module
End Namespace
