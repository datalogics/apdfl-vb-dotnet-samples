Imports System
Imports System.Collections.Generic
Imports Datalogics.PDFL

'
'
' Use this program to create a New PDF file And add glyphs to the page,
' managing them by individual Glyph ID codes.
' 
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'

Namespace AddGlyphs
    Module AddGlyphs
        Sub Main(args As String())
            Console.WriteLine("AddGlyphs Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")

                Dim sOutput As String = "AddGlyphs-out.pdf"

                If args.Length > 0 Then
                    sOutput = args(0)
                End If

                Console.WriteLine("Output file: " + sOutput)

                Dim doc As Document = New Document()

                Dim pageRect As Rect = New Rect(0, 0, 612, 792)
                Dim docpage As Page = doc.CreatePage(Document.BeforeFirstPage, pageRect)
                Console.WriteLine("Created page.")

                Dim font As Font
                Try
                    font = New Font("Times-Roman")
                Catch ex As ApplicationException
                    If ex.Message.Equals("The specified font could not be found.") And
                        System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) And
                            Not System.IO.Directory.Exists("/usr/share/fonts/msttcore/") Then
                        Console.WriteLine("Please install Microsoft Core Fonts on Linux first.")
                        Return
                    End If
                    Throw
                End Try

                Dim glyphIDs As List(Of Char) = New List(Of Char) From {
                    ChrW(&H2B),
                    ChrW(&H28),
                    ChrW(&H2F),
                    ChrW(&H2F),
                    ChrW(&H32)
                }

                Dim unicode As List(Of Char) = New List(Of Char) From {
                    "H"c,
                    "E"c,
                    "L"c,
                    "L"c,
                    "O"c
                }

                Dim state As TextState = New TextState()
                state.FontSize = 50

                Dim m As Matrix = New Matrix()
                m = m.Translate(docpage.CropBox.Bottom, docpage.CropBox.Right)

                Dim text As Text = New Text()
                text.AddGlyphs(glyphIDs, unicode, font, New GraphicState(), state, m, TextFlags.TextRun)

                docpage.Content.AddElement(text)
                docpage.UpdateContent()

                doc.EmbedFonts(EmbedFlags.None)

                doc.Save(SaveFlags.Full, sOutput)
            End Using
        End Sub
    End Module
End Namespace
