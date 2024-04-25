Imports System
Imports System.IO
Imports Datalogics.PDFL

''' This sample program demonstrates how to embed an ICC color profile in a graphics file.
''' The program sets up how the output will be rendered And generates a TIF image file Or
''' series of TIF files as output.
''' 
''' Copyright (c) 2024, Datalogics, Inc. All rights reserved.

Namespace ImageEmbedICCProfile
    Public Class ExportDocumentImages
        Private exporttype As ImageType

        Public Sub export_doc_images_type(doc As Document, profileStream As PDFStream, imtype As ImageType)
            exporttype = imtype
            Dim cs As New ICCBasedColorSpace(profileStream)
            For pgno As Integer = 0 To doc.NumPages - 1
                Dim pg As Page = doc.GetPage(pgno)

                Export_Image(pg.Content, cs, pg, pgno)
            Next
        End Sub

        Public Sub export_doc_images(doc As Document, profileStream As PDFStream)
            export_doc_images_type(doc, profileStream, ImageType.TIFF)
        End Sub

        Public Sub Export_Image(content As Content, csp As ColorSpace, pg As Page, pNum As Integer)
            Dim isp As ImageSaveParams

            Try
                isp = New ImageSaveParams()
                isp.Compression = CompressionCode.LZW

                Dim pip As New PageImageParams()
                pip.ImageColorSpace = csp

                Dim outImage As Datalogics.PDFL.Image = pg.GetImage(pg.CropBox, pip)

                Dim filenamevar As String = ""

                pip.RenderIntent = RenderIntent.Saturation
                outImage = pg.GetImage(pg.CropBox, pip)
                filenamevar = "ImageEmbedICCProfile-out_sat" & pNum & ".tif"
                outImage.Save(filenamevar, exporttype, isp)

                pip.RenderIntent = RenderIntent.AbsColorimetric
                outImage = pg.GetImage(pg.CropBox, pip)
                filenamevar = "ImageEmbedICCProfile-out_abs" & pNum & ".tif"
                outImage.Save(filenamevar, exporttype, isp)

                pip.RenderIntent = RenderIntent.Perceptual
                outImage = pg.GetImage(pg.CropBox, pip)
                filenamevar = "ImageEmbedICCProfile-out_per" & pNum & ".tif"
                outImage.Save(filenamevar, exporttype, isp)

                pip.RenderIntent = RenderIntent.RelColorimetric
                outImage = pg.GetImage(pg.CropBox, pip)
                filenamevar = "ImageEmbedICCProfile-out_rel" & pNum & ".tif"
                outImage.Save(filenamevar, exporttype, isp)
            Catch ex As Exception
                Console.WriteLine("Cannot write file: " & ex.Message)
            End Try
        End Sub
    End Class

    Public Class ImageEmbedICCProfile
        Shared Sub Main(args As String())
            Console.WriteLine("Image Embed ICC Profile sample:")

            Dim sInput As String = Library.ResourceDirectory & "Sample_Input/ducky.pdf"
            Dim profileName As String = Library.ResourceDirectory & "Sample_Input/Probev1_ICCv2.icc"

            Console.WriteLine("Input file: " & sInput & " will have profile " & profileName & " applied.")

            If args.Length > 0 Then
                sInput = args(0)
            End If

            If args.Length > 1 Then
                profileName = args(1)
            End If

            Using library As New Library()
                Console.WriteLine("Initialized the library.")

                Dim doc As New Document(sInput)

                Dim expdoc As New ExportDocumentImages()

                Dim profileFileStream As New FileStream(profileName, FileMode.Open)
                Dim profilePDFStream As New PDFStream(profileFileStream, doc, Nothing, Nothing)

                expdoc.export_doc_images(doc, profilePDFStream)
            End Using
        End Sub
    End Class
End Namespace
