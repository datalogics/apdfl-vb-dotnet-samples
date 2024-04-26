Imports Datalogics.PDFL

''' This sample program reads the pages of the PDF file that you provide And extracts images
''' that it finds on each page And saves those images to external graphics files, one each for
''' TIF, JPG, PNG, GIF, And BMP. 
''' 
''' To be more specific, the program examines the content stream for image elements And exports
''' those image objects. If a page in a PDF file has three images, the program will create three
''' sets of graphics files for those three images. The sample program ignores text, parsing the
''' PDF syntax to identify any raster Or vector images found on every page.
'''
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.

Namespace ImageExport
    Public Class ExportDocumentImages
        Private exporttype As ImageType
        Private nextImageIndex As Integer
        Private ic As New ImageCollection()

        Public Sub ExportDocImagesType(ByVal doc As Document, ByVal imtype As ImageType)
            exporttype = imtype
            Dim pgno As Integer
            For pgno = 0 To doc.NumPages - 1
                Dim pg As Page = doc.GetPage(pgno)
                Dim content As Content = pg.Content
                ExportElementImages(content)
            Next

            If ic.Count <> 0 Then
                Try
                    ic.Save("ImageExport-page" & pgno & "-out.tif", ImageType.TIFF)
                Catch ex As Exception
                    Console.WriteLine("Cannot write file: " & ex.Message)
                End Try
            End If
        End Sub

        Public Sub ExportDocImages(ByVal doc As Document)
            ExportDocImagesType(doc, ImageType.TIFF)
            ExportDocImagesType(doc, ImageType.JPEG)
            ExportDocImagesType(doc, ImageType.PNG)
            ExportDocImagesType(doc, ImageType.GIF)
            ExportDocImagesType(doc, ImageType.BMP)
        End Sub

        Public Sub ExportElementImages(ByVal content As Content)
            Dim i As Integer = 0
            Dim isp As ImageSaveParams

            Do While i < content.NumElements
                Dim e As Element = content.GetElement(i)
                If TypeOf e Is Datalogics.PDFL.Image Then
                    Dim img As Datalogics.PDFL.Image = DirectCast(e, Datalogics.PDFL.Image)
                    ' Weed out impossible or nonsensical combinations.
                    If img.ColorSpace = ColorSpace.DeviceCMYK AndAlso exporttype <> ImageType.JPEG Then
                        i += 1
                        Continue Do
                    End If

                    If exporttype = ImageType.TIFF Then
                        ic.Append(img)
                        isp = New ImageSaveParams()
                        isp.Compression = CompressionCode.LZW
                        img.Save("ImageExport-out" & nextImageIndex & ".tif", exporttype, isp)
                    End If

                    Try
                        If exporttype = ImageType.JPEG Then
                            isp = New ImageSaveParams()
                            isp.JPEGQuality = 80
                            img.Save("ImageExport-out" & nextImageIndex & ".jpg", exporttype, isp)
                        End If

                        If exporttype = ImageType.PNG Then
                            img.Save("ImageExport-out" & nextImageIndex & ".png", exporttype)
                        End If

                        If exporttype = ImageType.GIF Then
                            img.Save("ImageExport-out" & nextImageIndex & ".gif", exporttype)
                        End If

                        If exporttype = ImageType.BMP Then
                            img.Save("ImageExport-out" & nextImageIndex & ".bmp", exporttype)
                        End If
                    Catch ex As Exception
                        Console.WriteLine("Cannot write file: " & ex.Message)
                    End Try

                    nextImageIndex += 1
                ElseIf TypeOf e Is Container Then
                    Console.WriteLine("Recursing through a Container")
                    ExportElementImages(DirectCast(e, Container).Content)
                ElseIf TypeOf e Is Group Then
                    Console.WriteLine("Recursing through a Group")
                    ExportElementImages(DirectCast(e, Group).Content)
                ElseIf TypeOf e Is Form Then
                    Console.WriteLine("Recursing through a Form")
                    ExportElementImages(DirectCast(e, Form).Content)
                End If

                i += 1
            Loop
        End Sub
    End Class

    Class ImageExport
        Public Shared Sub Main(ByVal args As String())
            Console.WriteLine("Image Export sample:")

            Using (New Library())
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/ducky.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                Console.WriteLine("Input file: " & sInput)

                Dim doc As New Document(sInput)
                Dim expdoc As New ExportDocumentImages()
                expdoc.ExportDocImages(doc)
            End Using
        End Sub
    End Class
End Namespace
