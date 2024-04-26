Imports System
Imports System.Collections.Generic
Imports SkiaSharp
Imports Datalogics.PDFL
Imports System.IO

''' This sample demonstrates for drawing a list of grayscale separations from a PDF file.
''' 
''' Copyright (c) 2024, Datalogics, Inc. All rights reserved.

Namespace DrawSeparations
    Class DrawSeparations
        Shared Sub Main(args As String())
            Console.WriteLine("DrawSeparations Sample:")

            ' ReSharper disable once UnusedVariable
            Using (New Library())
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/ducky.pdf"
                Dim sOutput As String = "DrawSeparations-out"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                If args.Length > 1 Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Input file: " & sInput & ", writing output using prefix: " & sOutput)

                Dim doc As New Document(sInput)
                Dim pg As Page = doc.GetPage(0)

                ' Get all inks that are present on the page
                Dim inks As IList(Of Ink) = pg.ListInks()
                Dim colorants As New List(Of SeparationColorSpace)()

                ' Here we decide, which inks should be drawn
                For Each theInk As Ink In inks
                    ' note: if the Ink can't be found in page's resources,
                    ' default tintTransform and alternate will be used
                    colorants.Add(New SeparationColorSpace(pg, theInk))
                Next

                Dim width As Double = pg.MediaBox.Right - pg.MediaBox.Left
                Dim height As Double = pg.MediaBox.Top - pg.MediaBox.Bottom

                ' Must invert the page to get from PDF with origin at lower left,
                ' to a bitmap with the origin at upper right.
                Dim matrix As New Matrix()
                matrix = matrix.Scale(1, -1).Translate(-pg.MediaBox.Left, -pg.MediaBox.Top)

                Dim parms As New DrawParams()
                parms.Matrix = matrix
                parms.DestRect = New Rect(0, 0, width, height)
                parms.Flags = DrawFlags.DoLazyErase Or DrawFlags.UseAnnotFaces

                Dim bottom As Double = 0
                If pg.MediaBox.Bottom <> 0 Then
                    bottom = If(pg.MediaBox.Bottom < 0, -pg.MediaBox.Top, pg.MediaBox.Top)
                End If

                parms.UpdateRect = New Rect(pg.MediaBox.Left, bottom, width, height)

                ' Acquiring list of separations
                Dim separatedColorChannels As List(Of SKBitmap) = pg.DrawContents(parms, colorants)

                For i As Integer = 0 To separatedColorChannels.Count - 1
                    Using f As FileStream = File.OpenWrite(sOutput & i & "-" & colorants(i).SeparationName & ".png")
                        separatedColorChannels(i).Encode(SKEncodedImageFormat.Png, 100).SaveTo(f)
                    End Using
                Next
            End Using
        End Sub
    End Class
End Namespace
