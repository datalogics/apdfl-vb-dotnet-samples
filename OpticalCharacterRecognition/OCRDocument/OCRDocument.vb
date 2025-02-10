Imports Datalogics.PDFL

'
' Runs OCR on the document recognizing text found on its rasterized pages.
'
' Copyright (c) 2007-2025, Datalogics, Inc. All rights reserved.
'
'

Namespace OCRDocument
    Module OCRDocument
        Sub Main(args As String())
            Console.WriteLine("OCRDocument Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/scanned_images.pdf"
                Dim sOutput As String = "OCRDocument-out.pdf"

                If (args.Length > 0) Then
                    sInput = args(0)
                End If
                If (args.Length > 1) Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Input file: " + sInput)
                Console.WriteLine("Writing output to: " + sOutput)

                Dim ocrParams As OCRParams = New OCRParams()
                'The OCRParams.Languages parameter controls which languages the OCR engine attempts
                'to detect. By default the OCR engine searches for English.
                Dim langList As List(Of LanguageSetting) = New List(Of LanguageSetting)
                Dim languageOne As LanguageSetting = New LanguageSetting(Language.English, False)
                langList.Add(languageOne)

                'You could add additional languages for the OCR engine to detect by adding 
                'more entries to the LanguageSetting list. 

                'LanguageSetting languageTwo = new LanguageSetting(Language.Japanese, false)
                'langList.Add(languageTwo)
                OCRParams.Languages = langList

                ' If the resolution for the images in your document are not
                ' 300 dpi, specify a default resolution here. Specifying a
                ' correct resolution gives better results for OCR, especially
                ' with automatic image preprocessing.
                ' ocrParams.Resolution = 600

                Using ocrEngine As OCREngine = New OCREngine(OCRParams)
                    'Create a document object Imports the input file
                    Using doc As Document = New Document(sInput)
                        For numPage As Integer = 0 To doc.NumPages - 1
                            Using page As Page = doc.GetPage(numPage)
                                page.RecognizePageContents(doc, ocrEngine)
                            End Using
                        Next
                        doc.Save(SaveFlags.Full, sOutput)
                    End Using
                End Using
            End Using
        End Sub
    End Module
End Namespace
