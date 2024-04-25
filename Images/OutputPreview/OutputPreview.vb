Imports System
Imports System.Collections.Generic
Imports Datalogics.PDFL




''' This sample demonstrates creating an Output Preview Image which Is used during Soft Proofing prior to printing to visualize combining different Colorants.
''' 
''' Copyright (c) 2024, Datalogics, Inc. All rights reserved.

Namespace OutputPreview
    Class OutputPreview
        Private Shared Function CreateOutputFileName(ByVal colorants As List(Of String)) As String
            Dim outputFileName As String = "OutputPreview_"

            For Each colorant As String In colorants
                outputFileName += colorant & "_"
            Next

            outputFileName += ".tiff"

            Return outputFileName
        End Function

        Shared Sub Main(ByVal args As String())
            Console.WriteLine("OutputPreview Sample:")

            Dim sInput As String = Library.ResourceDirectory & "Sample_Input/spotcolors1.pdf"

            ' Here you specify the Colorant names of interest
            Dim colorantsToUse As New List(Of String) From {"Yellow", "Black"}

            Dim colorantsToUse2 As New List(Of String) From {"PANTONE 554 CVC", "PANTONE 814 2X CVC", "PANTONE 185 2X CVC"}

            Using library As New Library()
                Using doc As New Document(sInput)
                    Using pg As Page = doc.GetPage(0)
                        ' Get all inks that are present on the page
                        Dim inks As List(Of Ink) = CType(pg.ListInks(), List(Of Ink))

                        Dim colorants As New List(Of SeparationColorSpace)()

                        For Each theInk As Ink In inks
                            For Each theColorant As String In colorantsToUse
                                If theInk.ColorantName = theColorant Then
                                    colorants.Add(New SeparationColorSpace(pg, theInk))
                                End If
                            Next
                        Next

                        Dim colorants2 As New List(Of SeparationColorSpace)()

                        For Each theInk As Ink In inks
                            For Each theColorant As String In colorantsToUse2
                                If theInk.ColorantName = theColorant Then
                                    colorants2.Add(New SeparationColorSpace(pg, theInk))
                                End If
                            Next
                        Next

                        Dim pip As New PageImageParams()
                        pip.PageDrawFlags = DrawFlags.UseAnnotFaces
                        pip.HorizontalResolution = 300
                        pip.VerticalResolution = 300

                        Dim sp As New ImageSaveParams()

                        ' Create Output Preview images using the Specified Colorants
                        Dim image As Datalogics.PDFL.Image = pg.GetOutputPreviewImage(pg.CropBox, pip, colorants)

                        image.Save(CreateOutputFileName(colorantsToUse), ImageType.TIFF)

                        Dim image2 As Datalogics.PDFL.Image = pg.GetOutputPreviewImage(pg.CropBox, pip, colorants2)

                        image2.Save(CreateOutputFileName(colorantsToUse2), ImageType.TIFF)
                    End Using
                End Using
            End Using
        End Sub
    End Class
End Namespace
