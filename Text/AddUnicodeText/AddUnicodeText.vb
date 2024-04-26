Imports System
Imports Datalogics.PDFL

'
' 
' This sample program adds six lines of Unicode text to a PDF file, in six different languages.
'
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'

Namespace AddUnicodeText
    Module AddUnicodeText
        Sub Main(args As String())
            Console.WriteLine("AddUnicodeText Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")

                Dim sOutput As String = "AddUnicodeText-out.pdf"

                If args.Length > 0 Then
                    sOutput = args(0)
                End If

                Console.WriteLine("Output file: " & sOutput)

                Dim doc As Document = New Document()
                Dim pageRect As Rect = New Rect(0, 0, 612, 792)
                Dim docpage As Page = doc.CreatePage(Document.BeforeFirstPage, pageRect)

                Dim unicodeText As Text = New Text()
                Dim gs As GraphicState = New GraphicState()
                Dim ts As TextState = New TextState()

                Dim strings As New List(Of String) From {
                    "Chinese (Mandarin) - \u4e16\u754c\u4eba\u6743\u5ba3\u8a00",
                    "Japanese - \u300e\u4e16\u754c\u4eba\u6a29\u5ba3\u8a00\u300f",
                    "French - \u0044\u00e9\u0063\u006c\u0061\u0072\u0061\u0074\u0069\u006f\u006e" +
                            "\u0020\u0075\u006e\u0069\u0076\u0065\u0072\u0073\u0065\u006c\u006c\u0065\u0020\u0064" +
                            "\u0065\u0073\u0020\u0064\u0072\u006f\u0069\u0074\u0073\u0020\u0064\u0065\u0020\u006c" +
                            "\u2019\u0068\u006f\u006d\u006d\u0065",
                    "Korean - \uc138\u0020\uacc4\u0020\uc778\u0020\uad8c\u0020\uc120\u0020\uc5b8",
                    "English - \u0055\u006e\u0069\u0076\u0065\u0072\u0073\u0061\u006c\u0020\u0044" +
                            "\u0065\u0063\u006c\u0061\u0072\u0061\u0074\u0069\u006f\u006e\u0020\u006f\u0066\u0020" +
                            "\u0048\u0075\u006d\u0061\u006e\u0020\u0052\u0069\u0067\u0068\u0074\u0073",
                    "Greek - \u039f\u0399\u039a\u039f\u03a5\u039c\u0395\u039d\u0399\u039a\u0397\u0020" +
                            "\u0394\u0399\u0391\u039a\u0397\u03a1\u03a5\u039e\u0397\u0020\u0393\u0399\u0391\u0020" +
                            "\u03a4\u0391\u0020\u0391\u039d\u0398\u03a1\u03a9\u03a0\u0399\u039d\u0391\u0020\u0394" +
                            "\u0399\u039a\u0391\u0399\u03a9\u039c\u0391\u03a4\u0391",
                    "Russian - \u0412\u0441\u0435\u043e\u0431\u0449\u0430\u044f\u0020\u0434\u0435" +
                            "\u043a\u043b\u0430\u0440\u0430\u0446\u0438\u044f\u0020\u043f\u0440\u0430\u0432\u0020" +
                            "\u0447\u0435\u043b\u043e\u0432\u0435\u043a\u0430"
                }

                Dim fonts As List(Of Font) = New List(Of Font)
                Try
                    fonts.Add(New Font("Arial"))
                Catch ex As ApplicationException
                    If ex.Message.Equals("The specified font could not be found.") And
                            System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) And
                            Not System.IO.Directory.Exists("/usr/share/fonts/msttcore/") Then
                        Console.WriteLine("Please install Microsoft Core Fonts on Linux first.")
                        Return
                    End If
                    Throw
                End Try

                fonts.Add(New Font("KozGoPr6N-Medium"))
                fonts.Add(New Font("AdobeMyungjoStd-Medium"))

                ' These will be used to place the strings into position on the page.
                Dim x As Integer = 1 * 72
                Dim y As Integer = 10 * 72

                For Each str As String In strings
                    ' Find a font that can represent all characters in the string, if there is one.
                    Dim font As Font = GetRepresentableFont(fonts, str)
                    If font Is Nothing Then
                        Console.WriteLine("Couldn't find a font that can represent all characters in the string: " + str)
                    Else
                        ' From this point, the string is handled the same way as non-Unicode text.
                        Dim m As Matrix = New Matrix(14, 0, 0, 14, x, y)
                        Dim tr As TextRun = New TextRun(font.Name & " - " & str, font, gs, ts, m)
                        unicodeText.AddRun(tr)
                    End If

                    ' Start the next string lower down the page.
                    y -= 18
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
