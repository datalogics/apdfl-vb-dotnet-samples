Imports System
Imports System.IO
Imports Datalogics.PDFL
Imports SkiaSharp


''' This sample program searches through the PDF file that you select And identifies
''' raster drawings, diagrams And photographs among the text. Then, it extracts these
''' images from the PDF file And copies them to a separate set of graphics files in the
''' same directory. Vector images, such as clip art, will Not be exported.
'''
''' Copyright (c) 2024, Datalogics, Inc. All rights reserved.

Namespace ImageExtraction
    Class ImageExtraction
        Private Shared nextImageIndex As Integer

        Private Shared Sub ExtractImages(ByVal content As Content)
            For i As Integer = 0 To content.NumElements - 1
                Dim e As Element = content.GetElement(i)
                If TypeOf e Is Datalogics.PDFL.Image Then
                    Console.WriteLine("Saving an image")
                    Dim img As Datalogics.PDFL.Image = DirectCast(e, Datalogics.PDFL.Image)
                    Using sKBitmap As SKBitmap = img.SKBitmap
                        Using f As FileStream = File.OpenWrite("ImageExtraction-extract-out" & nextImageIndex & ".png")
                            sKBitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(f)
                        End Using
                    End Using

                    Dim newimg As Datalogics.PDFL.Image = img.ChangeResolution(500)
                    Using sKBitmap As SKBitmap = newimg.SKBitmap
                        Using f As FileStream = File.OpenWrite("ImageExtraction-extract-Resolution-500-out" & nextImageIndex & ".png")
                            sKBitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(f)
                        End Using
                    End Using
                    nextImageIndex += 1
                ElseIf TypeOf e Is Datalogics.PDFL.Container Then
                    ExtractImages(DirectCast(e, Datalogics.PDFL.Container).Content)
                ElseIf TypeOf e Is Datalogics.PDFL.Group Then
                    ExtractImages(DirectCast(e, Datalogics.PDFL.Group).Content)
                ElseIf TypeOf e Is Form Then
                    ExtractImages(DirectCast(e, Form).Content)
                End If
            Next
        End Sub

        Shared Sub Main(ByVal args As String())
            Console.WriteLine("ImageExtraction Sample:")

            Using (New Library())
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/ducky.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                Console.WriteLine("Input file: " & sInput)

                Dim doc As New Document(sInput)
                Dim pg As Page = doc.GetPage(0)
                Dim content As Content = pg.Content
                ExtractImages(content)
            End Using
        End Sub
    End Class
End Namespace
