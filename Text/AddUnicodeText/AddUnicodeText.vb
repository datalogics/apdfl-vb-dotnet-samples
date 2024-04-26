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
                    "Chinese (Mandarin) - " + Convert.ToChar(&H4E16) + Convert.ToChar(&H754C) + Convert.ToChar(&H4EBA) + Convert.ToChar(&H6743) + Convert.ToChar(&H5BA3) + Convert.ToChar(&H8A00),
                    "Japanese - " + Convert.ToChar(&H300E) + Convert.ToChar(&H4E16) + Convert.ToChar(&H754C) + Convert.ToChar(&H4EBA) + Convert.ToChar(&H6A29) + Convert.ToChar(&H5BA3) + Convert.ToChar(&H8A00) + Convert.ToChar(&H300F),
                    "French - " + Convert.ToChar(&H44) + Convert.ToChar(&HE9) + Convert.ToChar(&H63) + Convert.ToChar(&H6C) + Convert.ToChar(&H61) + Convert.ToChar(&H72) + Convert.ToChar(&H61) + Convert.ToChar(&H74) + Convert.ToChar(&H69) + Convert.ToChar(&H6F) + Convert.ToChar(&H6E) +
                            Convert.ToChar(&H20) + Convert.ToChar(&H75) + Convert.ToChar(&H6E) + Convert.ToChar(&H69) + Convert.ToChar(&H76) + Convert.ToChar(&H65) + Convert.ToChar(&H72) + Convert.ToChar(&H73) + Convert.ToChar(&H65) + Convert.ToChar(&H6C) + Convert.ToChar(&H6C) + Convert.ToChar(&H65) + Convert.ToChar(&H20) + Convert.ToChar(&H64) +
                            Convert.ToChar(&H65) + Convert.ToChar(&H73) + Convert.ToChar(&H20) + Convert.ToChar(&H64) + Convert.ToChar(&H72) + Convert.ToChar(&H6F) + Convert.ToChar(&H69) + Convert.ToChar(&H74) + Convert.ToChar(&H73) + Convert.ToChar(&H20) + Convert.ToChar(&H64) + Convert.ToChar(&H65) + Convert.ToChar(&H20) + Convert.ToChar(&H6C) +
                            Convert.ToChar(&H2019) + Convert.ToChar(&H68) + Convert.ToChar(&H6F) + Convert.ToChar(&H6D) + Convert.ToChar(&H6D) + Convert.ToChar(&H65),
                    "Korean - " + Convert.ToChar(&HC138) + Convert.ToChar(&H20) + Convert.ToChar(&HACC4) + Convert.ToChar(&H20) + Convert.ToChar(&HC778) + Convert.ToChar(&H20) + Convert.ToChar(&HAD8C) + Convert.ToChar(&H20) + Convert.ToChar(&HC120) + Convert.ToChar(&H20) + Convert.ToChar(&HC5B8) +
                    "English - " + Convert.ToChar(&H55) + Convert.ToChar(&H6E) + Convert.ToChar(&H69) + Convert.ToChar(&H76) + Convert.ToChar(&H65) + Convert.ToChar(&H72) + Convert.ToChar(&H73) + Convert.ToChar(&H61) + Convert.ToChar(&H6C) + Convert.ToChar(&H20) + Convert.ToChar(&H44) +
                            Convert.ToChar(&H65) + Convert.ToChar(&H63) + Convert.ToChar(&H6C) + Convert.ToChar(&H61) + Convert.ToChar(&H72) + Convert.ToChar(&H61) + Convert.ToChar(&H74) + Convert.ToChar(&H69) + Convert.ToChar(&H6F) + Convert.ToChar(&H6E) + Convert.ToChar(&H20) + Convert.ToChar(&H6F) + Convert.ToChar(&H66) + Convert.ToChar(&H20) +
                            Convert.ToChar(&H48) + Convert.ToChar(&H75) + Convert.ToChar(&H6D) + Convert.ToChar(&H61) + Convert.ToChar(&H6E) + Convert.ToChar(&H20) + Convert.ToChar(&H52) + Convert.ToChar(&H69) + Convert.ToChar(&H67) + Convert.ToChar(&H68) + Convert.ToChar(&H74) + Convert.ToChar(&H73),
                    "Greek - " + Convert.ToChar(&H39F) + Convert.ToChar(&H399) + Convert.ToChar(&H39A) + Convert.ToChar(&H39F) + Convert.ToChar(&H3A5) + Convert.ToChar(&H39C) + Convert.ToChar(&H395) + Convert.ToChar(&H39D) + Convert.ToChar(&H399) + Convert.ToChar(&H39A) + Convert.ToChar(&H397) + Convert.ToChar(&H20) +
                            Convert.ToChar(&H394) + Convert.ToChar(&H399) + Convert.ToChar(&H391) + Convert.ToChar(&H39A) + Convert.ToChar(&H397) + Convert.ToChar(&H3A1) + Convert.ToChar(&H3A5) + Convert.ToChar(&H39E) + Convert.ToChar(&H397) + Convert.ToChar(&H20) + Convert.ToChar(&H393) + Convert.ToChar(&H399) + Convert.ToChar(&H391) + Convert.ToChar(&H20) +
                            Convert.ToChar(&H3A4) + Convert.ToChar(&H391) + Convert.ToChar(&H20) + Convert.ToChar(&H391) + Convert.ToChar(&H39D) + Convert.ToChar(&H398) + Convert.ToChar(&H3A1) + Convert.ToChar(&H3A9) + Convert.ToChar(&H3A0) + Convert.ToChar(&H399) + Convert.ToChar(&H39D) + Convert.ToChar(&H391) + Convert.ToChar(&H20) + Convert.ToChar(&H394) +
                            Convert.ToChar(&H399) + Convert.ToChar(&H39A) + Convert.ToChar(&H391) + Convert.ToChar(&H399) + Convert.ToChar(&H3A9) + Convert.ToChar(&H39C) + Convert.ToChar(&H391) + Convert.ToChar(&H3A4) + Convert.ToChar(&H391),
                    "Russian - " + Convert.ToChar(&H412) + Convert.ToChar(&H441) + Convert.ToChar(&H435) + Convert.ToChar(&H43E) + Convert.ToChar(&H431) + Convert.ToChar(&H449) + Convert.ToChar(&H430) + Convert.ToChar(&H44F) + Convert.ToChar(&H20) + Convert.ToChar(&H434) + Convert.ToChar(&H435) +
                            Convert.ToChar(&H43A) + Convert.ToChar(&H43B) + Convert.ToChar(&H430) + Convert.ToChar(&H440) + Convert.ToChar(&H430) + Convert.ToChar(&H446) + Convert.ToChar(&H438) + Convert.ToChar(&H44F) + Convert.ToChar(&H20) + Convert.ToChar(&H43F) + Convert.ToChar(&H440) + Convert.ToChar(&H430) + Convert.ToChar(&H432) + Convert.ToChar(&H20) +
                            Convert.ToChar(&H447) + Convert.ToChar(&H435) + Convert.ToChar(&H43B) + Convert.ToChar(&H43E) + Convert.ToChar(&H432) + Convert.ToChar(&H435) + Convert.ToChar(&H43A) + Convert.ToChar(&H430)
                }

                Dim fonts As List(Of Font) = New List(Of Font)
                Try
                    fonts.Add(New Font("Arial"))
                Catch ex As ApplicationException
                    If ex.Message.Equals("The specified font could Not be found.") And
                            System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) And
                            Not System.IO.Directory.Exists("/usr/share/fonts/msttcore/") Then
                        Console.WriteLine("Please install Microsoft Core Fonts On Linux first.")
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
