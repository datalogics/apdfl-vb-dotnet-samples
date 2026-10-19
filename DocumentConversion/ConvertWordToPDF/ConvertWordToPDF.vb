Imports System
Imports Datalogics.PDFL

'
' This sample demonstrates the OfficeToPDF plug-in, which converts a Microsoft
' Word (.docx) document into a PDF.
'
' By default the sample converts DOCXLink.docx from the Sample_Input
' directory and writes the result to ConvertWordToPDF-out.pdf in the current
' working directory. The first optional argument is the input .docx path and
' the second is the output PDF path.
'
' Document.FromOfficeFile takes the input path, an OfficeToPDFConvertParams
' and an OfficeConvertInfo. It returns the converted Document, and fills the
' OfficeConvertInfo with the page count and the per-asset diagnostics.
'
' Copyright (c) Datalogics, Inc. All rights reserved.
'
Namespace ConvertWordToPDF
    Module ConvertWordToPDF
        Sub Main(args As String())
            Console.WriteLine("ConvertWordToPDF sample:")

            Using New Library()
                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/DOCXLink.docx"
                Dim sOutput As String = "ConvertWordToPDF-out.pdf"

                If (args.Length > 0) Then
                    sInput = args(0)
                End If
                If (args.Length > 1) Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Converting " + sInput + " and saving as " + sOutput)

                ' Configure the conversion. One OfficeToPDFConvertParams serves
                ' any number of conversions.
                Using convertParams As OfficeToPDFConvertParams = New OfficeToPDFConvertParams()
                    ' OfficeCommentRendering.Omit (default), Margin, or Annotations.
                    convertParams.Comments = OfficeCommentRendering.Margin

                    ' A fixed instant is stamped as /CreationDate and /ModDate and
                    ' used for DATE and TIME fields, so every conversion carries
                    ' the same dates. Leave it unset to stamp the system clock.
                    convertParams.SetConversionTime(2026, 1, 1, 0, 0, 0)

                    Using info As OfficeConvertInfo = New OfficeConvertInfo()
                        Dim doc As Document
                        Try
                            doc = Document.FromOfficeFile(sInput, convertParams, info)
                        Catch ex As Exception
                            ' The message names the reason: the source is missing,
                            ' not a valid .docx, or password-protected, or the
                            ' OfficeToPDF plug-in is not available.
                            Console.WriteLine("Conversion failed: " + ex.Message)
                            Return
                        End Try

                        Using doc
                            Console.WriteLine("Successfully converted the document: " + info.PageCount.ToString() + " page(s).")

                            ' A successful conversion can still report diagnostics:
                            ' a substituted font, an undecodable image, a hyperlink
                            ' dropped as unsafe. They describe the rendered assets;
                            ' they do not make the conversion fail.
                            For Each diagnostic As OfficeDiagnostic In info.Diagnostics
                                Console.WriteLine("  [" + diagnostic.Kind.ToString() + "] " + diagnostic.Asset + ": " _
                                    + diagnostic.Message)
                            Next

                            doc.Save(SaveFlags.Full, sOutput)
                            Console.WriteLine("Saved to " + sOutput)
                        End Using
                    End Using
                End Using
            End Using
        End Sub
    End Module
End Namespace
