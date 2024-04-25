Imports System.IO
Imports Datalogics.PDFL

'
'
' Run this program to extract content from a PDF file from a stream. The system will prompt you to enter
' the name of a PDF file to use for input. The program generates two output PDF files.
'
' A stream is a string of bytes of any length, embedded in a PDF document with a dictionary
' that is used to interpret the values in the stream.
'
' This program is similar to ImageFromStream, but in this example the PDF file streams hold text.
'
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'
Namespace StreamIO
    Class StreamIOSample
        Sub ReadFromStream(path As String, output As String)
            Dim fs As FileStream = New FileStream(path, FileMode.Open)

            ' A document is then opened, using the FileStream as its data source.
            Using d = New Document(fs)
                ' Add a watermark to have some visible change to the PDF
                Dim wp As WatermarkParams = New WatermarkParams()
                wp.TargetRange.PageSpec = PageSpec.AllPages
                Dim wtp As WatermarkTextParams = New WatermarkTextParams()
                wtp.Text = "This PDF was opened\nfrom a Stream"
                d.Watermark(wtp, wp)

                ' Instead, the document can be saved to a file. Saving the document
                ' to a file causes the document to use the file as its data source.
                d.Save(SaveFlags.Full, output)

                ' Make another minor change.
                d.Creator = "PDFL StreamIO Sample"

                ' Since the document is now backed by a file, an incremental save is okay.
                d.Save(SaveFlags.Incremental)
            End Using
        End Sub

        Sub WriteToStream(output As String)
            ' A MemoryStream will be used as the destination.
            Dim ms As MemoryStream = New MemoryStream()

            Using d = New Document()
                ' Add some content to the Document so there will be something to see.
                d.Creator = "PDFL StreamIO Sample"
                Dim p As Page = d.CreatePage(Document.BeforeFirstPage, New Rect(0, 0, 612, 792))
                AddContentToPage(p)

                ' Save the file to the MemoryStream.  The document is saved as a
                ' copy, so the full contents will be written.
                d.Save(SaveFlags.Full, ms)
            End Using

            ' Though the document has been disposed, we can open a new document
            ' using the saved stream.
            Using d As Document = New Document(ms)
                Console.WriteLine("creator: " + d.Creator)
            End Using

            ' Another possibility would be to write the stream to a file, and open
            ' the document from the file.
            Dim fs As FileStream = New FileStream(output, FileMode.Create)
            ms.WriteTo(fs)
            fs.Close()
            Using d As Document = New Document(output)
                Console.WriteLine("creator: " + d.Creator)
            End Using
        End Sub

        Sub AddContentToPage(p As Page)
            Dim rect As Datalogics.PDFL.Path = New Datalogics.PDFL.Path()
            rect.AddRect(New Point(100, 100), 100, 100)
            Dim gs As GraphicState = New GraphicState()
            gs.StrokeColor = New Color(0, 1.0, 0)
            gs.Width = 3.0
            rect.GraphicState = gs
            rect.PaintOp = PathPaintOpFlags.Stroke
            p.Content.AddElement(rect)
            p.UpdateContent()
        End Sub
    End Class
    Module StreamIO
        Sub Main(args As String())
            Dim sample As StreamIOSample = New StreamIOSample
            Console.WriteLine("StreamIO Sample:")

            Using New Library()
                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/sample.pdf"
                Dim sOutput1 As String = "StreamIO-out1.pdf"
                Dim sOutput2 As String = "StreamIO-out2.pdf"

                If (args.Length > 0) Then
                    sInput = args(0)
                End If
                If (args.Length > 1) Then
                    sOutput1 = args(1)
                End If
                If (args.Length > 2) Then
                    sOutput2 = args(2)
                End If
                Console.WriteLine("Input file: " + sInput + ". Writing to output " + sOutput1 + " and " + sOutput2)

                sample.ReadFromStream(sInput, sOutput1)

                sample.WriteToStream(sOutput2)
            End Using
        End Sub
    End Module
End Namespace
