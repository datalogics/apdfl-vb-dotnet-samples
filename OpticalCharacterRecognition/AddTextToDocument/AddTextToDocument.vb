Imports System
Imports System.Collections.Generic
Imports Datalogics.PDFL

'
' Process a document using the optical recognition engine.
' Then place the image and the processed text in an output pdf
'
' Copyright (c) 2024, Datalogics, Inc. All rights reserved.
'
'

Namespace AddTextToDocument
    Module AddTextToDocument
        'This function will find every image in the document and add text if 
        'possible
        Sub AddTextToImages(doc As Document, content As Content, engine As OCREngine)
            For index As Integer = 0 To content.NumElements - 1
                Dim e As Element = content.GetElement(index)
                If TypeOf e Is Datalogics.PDFL.Image Then
                    'PlaceTextUnder creates a form with the image and the generated text
                    'under the image. The original image in the page is then replaced by
                    'by the form.
                    Dim form As Form = engine.PlaceTextUnder(e, doc)
                    content.RemoveElement(index)
                    content.AddElement(form, index - 1)
                ElseIf TypeOf e Is Container Then
                    AddTextToImages(doc, CType(e, Container).Content, engine)
                ElseIf TypeOf e Is Group Then
                    AddTextToImages(doc, CType(e, Group).Content, engine)
                ElseIf TypeOf e Is Form Then
                    AddTextToImages(doc, CType(e, Form).Content, engine)
                End If
            Next
        End Sub

        Sub Main(args As String())
            Console.WriteLine("AddTextToDocument Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = "scanned_images.pdf"
                Dim sOutput As String = "AddTextToDocument-out.pdf"

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
                                Dim content As Content = page.Content
                                Console.WriteLine("Adding text to page: " + CStr(numPage))
                                AddTextToImages(doc, content, ocrEngine)
                                page.UpdateContent()
                            End Using
                        Next
                        doc.Save(SaveFlags.Full, sOutput)
                    End Using
                End Using
            End Using
        End Sub
    End Module
End Namespace
