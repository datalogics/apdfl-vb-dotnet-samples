Imports System
Imports Datalogics.PDFL

 '
 '
 ' This sample demonstrates converting a standard PDF document into a
 ' PDF Archive, or PDF/A, compliant version of a PDF file.
 '
 ' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
 '
 '/
Namespace PDFAConverter
    Module PDFAConverter
        Sub Main(args As String())
            Console.WriteLine("PDFAConverter Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/ducky.pdf"
                Dim sOutput As String = "PDFAConverter-out.pdf"

                If (args.Length > 0) Then
                    sInput = args(0)
                End If
                If (args.Length > 1) Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Converting " + sInput + ", output file is " + sOutput)

                Using doc As Document = New Document(sInput)
                    ' Make a conversion parameters object
                    Dim pdfaParams As PDFAConvertParams = New PDFAConvertParams()
                    pdfaParams.AbortIfXFAIsPresent = true
                    pdfaParams.IgnoreFontErrors = false
                    pdfaParams.NoValidationErrors = false
                    pdfaParams.ValidateImplementationLimitsOfDocument = true

                    ' Create a PDF/A compliant version of the document
                    Dim pdfaResult As PDFAConvertResult = doc.CloneAsPDFADocument(PDFAConvertType.RGB3b, pdfaParams)

                    ' The conversion may have failed: we must check if the result has a valid Document
                    If (pdfaResult.PDFADocument Is Nothing) Then
                        Console.WriteLine("ERROR: Could not convert " + sInput + " to PDF/A.")
                    Else
                        Console.WriteLine("Successfully converted " + sInput + " to PDF/A.")
                    End If

                    Dim pdfaDoc As Document = pdfaResult.PDFADocument

                    'Save the result.
                    pdfaDoc.Save(pdfaResult.PDFASaveFlags, sOutput)
                End Using
            End Using
        End Sub
    End Module
End Namespace
