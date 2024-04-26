Imports System
Imports Datalogics.PDFL

'
' 
' This program describes how to render text from top to bottom on a page.  The program provides
' a WritingMode “Vertical” with a string of Unicode characters to present sample text. 
' 
' The sample offers several rows of Unicode characters. The sample PDF file thus presents multiple columns
' of vertical text.  The characters appear in English as well as Mandarin, Japanese, And Korean.
' 
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'
Namespace AddVerticalText
    Module AddVerticalText
        Sub Main(args As String())
            Console.WriteLine("AddVerticalText Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")

                Dim sOutput As String = "AddVerticalText-out.pdf"

                If args.Length > 0 Then
                    sOutput = args(0)
                End If

                Console.WriteLine("Output file: " + sOutput)

                Dim doc As Document = New Document()
                Dim pageRect As Rect = New Rect(0, 0, 612, 792)
                Dim docpage As Page = doc.CreatePage(Document.BeforeFirstPage, pageRect)

                Dim unicodeText As Text = New Text()
                Dim gs As GraphicState = New GraphicState()
                Dim ts As TextState = New TextState()

                Dim strings As List(Of String) = New List(Of String)

                strings.Add("\u0055\u006e\u0069\u0076\u0065\u0072\u0073\u0061\u006c\u0020\u0044\u0065\u0063" +
                            "\u006c\u0061\u0072\u0061\u0074\u0069\u006f\u006e\u0020\u006f\u0066\u0020\u0048\u0075" +
                            "\u006d\u0061\u006e\u0020\u0052\u0069\u0067\u0068\u0074\u0073")
                strings.Add("\u4e16\u754c\u4eba\u6743\u5ba3\u8a00")
                strings.Add("\u300e\u4e16\u754c\u4eba\u6a29\u5ba3\u8a00\u300f")
                strings.Add("\uc138\u0020\uacc4\u0020\uc778\u0020\uad8c\u0020\uc120\u0020\uc5b8")

                ' Create the fonts with vertical WritingMode.  We don't need any special
                ' FontCreateFlags, so we just pass zero
                Dim fonts As List(Of Font) = New List(Of Font)
                fonts.Add(New Font("KozGoPr6N-Medium", 0, WritingMode.Vertical))
                fonts.Add(New Font("AdobeMyungjoStd-Medium", 0, WritingMode.Vertical))

                ' These will be used to place the strings into position on the page.
                Dim x As Integer = 1 * 72
                Dim y As Integer = 10 * 72

                For Each str As String In strings
                    ' Find a font that can represent all characters in the string, if there is one.
                    Dim font As Font = GetRepresentableFont(fonts, str)
                    If font Is Nothing Then
                        Console.WriteLine(
                            "Couldn't find a font that can represent all characters in the string: " + str)
                    Else
                        ' From this point, the string is handled the same way as non-Unicode text.
                        Dim m As Matrix = New Matrix(14, 0, 0, 14, x, y)
                        Dim tr As TextRun = New TextRun(str, font, gs, ts, m)
                        unicodeText.AddRun(tr)
                    End If

                    ' Start the next string moving across the page to the right
                    x += 30
                Next

                docpage.Content.AddElement(unicodeText)
                docpage.UpdateContent()

                ' Save the document.
                Console.WriteLine("Embedding fonts.")
                doc.EmbedFonts(EmbedFlags.None)
                doc.Save(SaveFlags.Full, sOutput)
            End Using

        End Sub

        Function GetRepresentableFont(fonts As List(Of Font), str As String)
            For Each font As Font In fonts
                If font.IsTextRepresentable(str) Then
                    Return font
                End If
            Next
            Return Nothing
        End Function
    End Module
End Namespace
