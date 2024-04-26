Imports Datalogics.PDFL

''' This sample demonstrates drawing a list of grayscale separations from a PDF file to multi-paged TIFF file.
'''
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.

Namespace GetSeparatedImages
    Class GetSeparatedImages
        Shared Sub Main(args As String())
            Console.WriteLine("GetSeparatedImages Sample:")

            Dim sInput As String = Library.ResourceDirectory & "Sample_Input/ducky.pdf"
            Dim sOutput As String = "GetSeparatedImages-out.tiff"

            If args.Length > 0 Then
                sInput = args(0)
            End If

            If args.Length > 1 Then
                sOutput = args(1)
            End If

            Console.WriteLine("Input file: " & sInput + ", will write to " & sOutput)

            Using library As New Library()
                Dim doc As New Document(sInput)
                Dim pg As Page = doc.GetPage(0)

                Dim inks As IList(Of Ink) = pg.ListInks()
                Dim colorants As New List(Of SeparationColorSpace)()

                For Each theInk As Ink In inks
                    colorants.Add(New SeparationColorSpace(pg, theInk))
                Next

                Dim pip As New PageImageParams()
                pip.PageDrawFlags = DrawFlags.UseAnnotFaces
                pip.HorizontalResolution = 300
                pip.VerticalResolution = 300

                Dim images As ImageCollection = pg.GetImageSeparations(pg.CropBox, pip, colorants)
                images.Save(sOutput, ImageType.TIFF)
            End Using
        End Sub
    End Class
End Namespace
