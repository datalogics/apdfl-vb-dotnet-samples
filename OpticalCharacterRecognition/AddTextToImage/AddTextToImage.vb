Imports Datalogics.PDFL

'
' The sample uses an image as input which will be processed by the optical recognition engine.
' We will then place the image and the processed text in an output pdf
'
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'/

Namespace AddTextToImage
    Module AddTextToImage
        Sub Main(args As String())
            Console.WriteLine("AddTextToImage Sample:")

            using New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/text_as_image.jpg"
                Dim sOutput As String = "AddTextToImage-out.pdf"

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
                Dim langList as List(Of LanguageSetting) = new List(Of LanguageSetting)
                Dim languageOne As LanguageSetting = New LanguageSetting(Language.English, false)
                langList.Add(languageOne)

                'You could add additional languages for the OCR engine to detect by adding 
                'more entries to the LanguageSetting list. 

                'LanguageSetting languageTwo = new LanguageSetting(Language.Japanese, false)
                'langList.Add(languageTwo)
                ocrParams.Languages = langList

                ' If your image resolution is not 300 dpi, specify it here. Specifying a
                ' correct resolution gives better results for OCR, especially with
                ' automatic image preprocessing.
                ' ocrParams.Resolution = 600

                Using ocrEngine As OCREngine = New OCREngine(ocrParams)
                    'Create a document object
                    Using doc As Document = new Document()
                        Using newImage As Datalogics.PDFL.Image = New Datalogics.PDFL.Image(sInput, doc)
                            ' Create a PDF page which is the size of the image.
                            ' Matrix.A and Matrix.D fields, respectively.
                            ' There are 72 PDF user space units in one inch.
                            Dim pageRect As Rect = New Rect(0, 0, newimage.Matrix.A, newimage.Matrix.D)
                            Using docpage As Page = doc.CreatePage(Document.BeforeFirstPage, pageRect)
                                docpage.Content.AddElement(newimage)
                                docpage.UpdateContent()
                            End Using
                        End Using

                        Using page As Page = doc.GetPage(0)
                            Dim content As Content = page.Content
                            Dim elem As Element = content.GetElement(0)
                            Dim image As Datalogics.PDFL.Image = CType(elem, Datalogics.PDFL.Image)
                            'PlaceTextUnder creates a form with the image and the generated text
                            'under the image. The original image in the page is then replaced by
                            'by the form.
                            Dim form As Form = ocrEngine.PlaceTextUnder(image, doc)
                            content.RemoveElement(0)
                            content.AddElement(form, Content.BeforeFirst)
                            page.UpdateContent()
                        End Using

                        doc.Save(SaveFlags.Full, sOutput)
                    End Using
                End Using
            End Using
        End Sub
    End Module
End Namespace
