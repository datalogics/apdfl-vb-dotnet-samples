Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Datalogics.PDFL




''' This sample demonstrates creating a new PDF document with a Header and Footer.
''' 
''' Copyright (c) 2022-2024, Datalogics, Inc. All rights reserved.

Namespace AddHeaderFooter
    Class AddHeaderFooter
        Shared Sub Main(args As String())
            Dim fontSize As Double = 12.0
            Dim topBottomMargin As Double = 0.5
            Dim pageWidth As Double = 8.5 * 72
            Dim pageHeight As Double = 11 * 72
            Dim headerText As String = "Title of Document"
            Dim footerText As String = "Page 1"

            Console.WriteLine("AddHeaderFooter Sample:")

            Using library As New Library()
                Dim sOutput As String = "AddHeaderFooter-out.pdf"

                Console.WriteLine("Output file: " + sOutput)

                Using doc As New Document()
                    Dim pageRect As New Rect(0, 0, pageWidth, pageHeight)

                    ' Create the new page
                    Using newPage As Page = doc.CreatePage(Document.BeforeFirstPage, pageRect)
                        Dim font As New Font("Times-Roman", FontCreateFlags.Subset)

                        Dim textHeight As Double = (font.Ascent + Math.Abs(font.Descent)) / 1000

                        Dim graphicState As New GraphicState()
                        Dim textState As New TextState()

                        ' Calculate the positioning of the Header and Footer
                        Dim headerMatrix As New Matrix(fontSize, 0, 0, fontSize, (pageWidth - font.MeasureTextWidth(headerText, fontSize)) / 2, pageHeight - topBottomMargin * 72.0 + textHeight)

                        Dim footerMatrix As New Matrix(fontSize, 0, 0, fontSize, (pageWidth - font.MeasureTextWidth(footerText, fontSize)) / 2, topBottomMargin * 72.0 - textHeight)

                        Dim headerTextRun As New TextRun(headerText, font, graphicState, textState, headerMatrix)

                        Dim footerTextRun As New TextRun(footerText, font, graphicState, textState, footerMatrix)

                        ' Add the Text Run elements to the Text
                        Dim text As New Text()
                        text.AddRun(headerTextRun)
                        text.AddRun(footerTextRun)

                        ' Add the Text to the Page Content
                        newPage.Content.AddElement(text)
                        newPage.UpdateContent()

                        doc.Save(SaveFlags.Full, sOutput)
                    End Using
                End Using
            End Using
        End Sub
    End Class
End Namespace

