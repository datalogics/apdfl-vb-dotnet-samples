Imports System.Runtime.InteropServices
Imports Datalogics.PDFL
Imports SkiaSharp


''' This sample program searches through the PDF file that you select And identifies drawings,
''' diagrams And photographs from the System.IO.Streams.
''' 
''' A stream Is a string of bytes of any length, embedded in a PDF document with a dictionary
''' that Is used to interpret the values in the stream.
''' 
''' This program Is similar to StreamIO.

''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.

Namespace ImageFromStream
    Class ImageFromStream
        Shared Sub Main(args As String())
            Console.WriteLine("ImageFromStream Sample:")

            Using (New Library())
                Console.WriteLine("Initialized the library.")

                Dim bitmapInput As String = Library.ResourceDirectory + "Sample_Input/Datalogics.bmp"
                Dim jpegInput As String = Library.ResourceDirectory + "Sample_Input/ducky.jpg"
                Dim docOutput As String = "ImageFromStream-out2.pdf"

                If args.Length > 0 Then
                    bitmapInput = args(0)
                End If

                If args.Length > 1 Then
                    jpegInput = args(1)
                End If

                If args.Length > 2 Then
                    docOutput = args(2)
                End If

                Console.WriteLine("using bitmap input " & bitmapInput & " and jpeg input " & jpegInput &
                                  ". Writing to output " & docOutput)

                ' Create a MemoryStream object.
                ' A MemoryStream is used here for demonstration only, but the technique
                ' works just as well for other streams which support seeking.
                Using BitmapStream As New System.IO.MemoryStream()
                    ' Load a bitmap image into the MemoryStream.
                    Using BitmapImage As SKBitmap = SKBitmap.FromImage(SKImage.FromEncodedData(bitmapInput))
                        BitmapImage.Encode(SKEncodedImageFormat.Png, 100).SaveTo(BitmapStream)

                        ' Reset the MemoryStream's seek position before handing it to the PDFL API,
                        ' which expects the seek position to be at the beginning of the stream.
                        BitmapStream.Seek(0, System.IO.SeekOrigin.Begin)

                        ' Create the PDFL Image object.
                        Dim PDFLBitmapImage As New Datalogics.PDFL.Image(BitmapStream)

                        ' Save the image to a PNG file.
                        PDFLBitmapImage.Save("ImageFromStream-out.png", ImageType.PNG)

                        ' The following demonstrates reading an image from a Stream and placing it into a document.
                        ' First, create a new Document and add a Page to it.
                        Using doc As New Document()
                            doc.CreatePage(Document.BeforeFirstPage, New Rect(0, 0, 612, 792))

                            ' Create a new MemoryStream for a new image file.
                            Using outputImageStream As New System.IO.MemoryStream()
                                ' Load a JPEG image into the MemoryStream.
                                Using inputImage As SKImage = SKImage.FromEncodedData(jpegInput)
                                    If RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
                                        inputImage.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(outputImageStream)
                                    Else
                                        ' Known issue in JPEG encoding in SkiaSharp v2.88.6: https://github.com/mono/SkiaSharp/issues/2643
                                        ' Previous versions have known vulnerability, so use PNG instead.
                                        inputImage.Encode(SKEncodedImageFormat.Png, 100).SaveTo(outputImageStream)
                                    End If

                                    ' An alternative method for resetting the MemoryStream's seek position.
                                    outputImageStream.Position = 0

                                    ' Create the PDFL Image object and put it in the Document.
                                    ' Since the image will be placed in a Document, use the constructor with the optional
                                    ' Document parameter to optimize data usage for this image within the Document.
                                    Dim PDFLJpegImage As New Datalogics.PDFL.Image(outputImageStream, doc)
                                    Dim pg As Page = doc.GetPage(0)
                                    pg.Content.AddElement(PDFLJpegImage)
                                    pg.UpdateContent()
                                    doc.Save(SaveFlags.Full, docOutput)
                                End Using
                            End Using
                        End Using
                    End Using
                End Using
            End Using
        End Sub
    End Class
End Namespace
