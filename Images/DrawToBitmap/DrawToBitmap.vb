Imports System.IO
Imports System.Runtime.InteropServices
Imports Datalogics.PDFL
Imports SkiaSharp

''' This program sample converts a PDF file to a series of image files.
''' 
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.

Namespace DrawToBitmap
    ''' <summary>
    ''' The class implements the SampleCancelProc type
    ''' </summary>
    Class SampleCancelProc
        Inherits CancelProc
        ''' <summary>
        ''' The method implements a callback (it MUST be named "Call", return a bool and take no arguments)
        ''' It will be driven by PDFL and if the method returns true rendering will attempt to cancel.
        ''' </summary>
        ''' <returns>A boolean that tells PDFL to continue (false) or cancel (true)</returns>
        Public Overrides Function [Call]() As Boolean
            mSomeBoolean = If(mSomeBoolean, False, True)
            Console.WriteLine("SampleCancelProc Call.")

            Return False
        End Function

        Private Shared mSomeBoolean As Boolean
    End Class

    ''' <summary>
    ''' The class implements the SampleRenderProgressProc type
    ''' </summary>
    Class SampleRenderProgressProc
        Inherits RenderProgressProc
        ''' <summary>
        ''' The method implements a callback (it MUST be named "Call" and exhibit the method signature described)
        ''' It will be driven by PDFL and provide data that can be used to update a progress bar, etc.
        ''' </summary>
        ''' <param name="stagePercent">A percentage complete (of the stage!). Values will always be in the range of 0.0 (0%) to 1.0 (100%)</param>
        ''' <param name="info">A string that will present optional information that may be written to user interface</param>
        ''' <param name="stage">An enumeration that indicates the current rendering stage</param>
        Public Overrides Sub [Call](stagePercent As Single, info As String, stage As RenderProgressStage)
            mSomeBoolean = If(mSomeBoolean, False, True)
            Console.WriteLine("SampleRenderProgressProc Call (stage/stagePercent/info): {0} {1} {2}", stage, stagePercent, info)
        End Sub

        Private Shared mSomeBoolean As Boolean
    End Class

    Class ScaledDrawToBitmap
        Private Const resolution As Double = 96.0

        ''' <summary>
        ''' The method constructs a DrawParams instance.
        ''' </summary>
        ''' <param name="matrix">the matrix for DrawParams</param>
        ''' <param name="updateRect">The portation of the page to draw</param>
        ''' <param name="blackPointCompensation">the flag which allows to turn on black point compensation</param>
        ''' <returns>DrawParams instance</returns>
        Private Shared Function ConstructDrawParams(matrix As Matrix, updateRect As Rect, blackPointCompensation As Boolean) As DrawParams
            Dim parms As New DrawParams()

            parms.Matrix = matrix
            parms.UpdateRect = updateRect
            parms.ColorSpace = ColorSpace.DeviceRGBA
            parms.DestRect = parms.UpdateRect.Transform(matrix)

            parms.Flags = DrawFlags.DoLazyErase Or DrawFlags.UseAnnotFaces

            parms.EnableBlackPointCompensation = blackPointCompensation

            Return parms
        End Function

        ''' <summary>
        ''' The method saves a bitmap to various files.
        ''' </summary>
        ''' <param name="bitmap">the bitmap to save</param>
        ''' <param name="nameSuffix">the suffix for saved file.</param>
        Private Shared Sub SaveBitmap(sKBitmap As SKBitmap, nameSuffix As String)
            'Known issue in JPEG encoding in SkiaSharp v2.88.6: https://github.com/mono/SkiaSharp/issues/2643 on non-Windows.
            'Previous versions have known vulnerability.
            If RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
                Using f As FileStream = File.OpenWrite(String.Format("DrawTo{0}.jpg", nameSuffix))
                    sKBitmap.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(f)
                End Using
            End If

            Using f As FileStream = File.OpenWrite(String.Format("DrawTo{0}.png", nameSuffix))
                sKBitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(f)
            End Using
        End Sub

        ''' <summary>
        ''' The method renders and saves the specified layer to the SkiaSharp.SKBitmap.
        ''' </summary>
        ''' <param name="pg">the page that should be rendered into the SKBitmap</param>
        ''' <param name="parms">the DrawParams object</param>
        ''' <param name="layerName">the layer name. It uses for saving process.</param>
        Private Shared Sub DrawLayerToBitmap(pg As Page, parms As DrawParams, layerName As String)
            Dim width As Integer = CInt(Math.Ceiling(parms.DestRect.Width))
            Dim height As Integer = CInt(Math.Ceiling(parms.DestRect.Height))

            Using sKBitmap As New SKBitmap(width, height)
                pg.DrawContentsToSKBitmap(sKBitmap, parms)
                Using f As FileStream = File.OpenWrite(String.Format("DrawLayer{0}ToBitmap.png", layerName))
                    sKBitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(f)
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' The method renders the specified page's layers to a SkiaSharp.SKBitmap.
        ''' </summary>
        ''' <param name="doc">Document whose layers should be rendered</param>
        ''' <param name="pg">page to render</param>
        ''' <param name="parms">the DrawParams object</param>
        Private Shared Sub DrawLayersToBitmap(doc As Document, pg As Page, parms As DrawParams)
            Dim ocgs As IList(Of OptionalContentGroup) = doc.OptionalContentGroups
            If ocgs.Count > 0 Then
                Dim states As New List(Of Boolean)(New Boolean(ocgs.Count - 1) {})

                For i As Integer = 0 To ocgs.Count - 1
                    states(i) = True

                    Using occ As New OptionalContentContext(doc)
                        ' Render and save current layer
                        occ.SetOCGStates(ocgs, states)
                        parms.OptionalContentContext = occ

                        DrawLayerToBitmap(pg, parms, ocgs(i).Name)

                        ' Return DrawParams to its initial value
                        parms.OptionalContentContext = Nothing
                    End Using

                    states(i) = False
                Next
            End If
        End Sub

        ''' <summary>
        ''' The method renders the specified page to a SkiaSharp.SKBitmap using DrawParams.
        ''' </summary>
        ''' <param name="pg">page to render</param>
        ''' <param name="parms">the DrawParams object</param>
        Private Shared Sub DrawToBitmapWithDrawParams(pg As Page, parms As DrawParams)
            Dim boundBox As Rect = parms.DestRect

            Dim width As Integer = CInt(Math.Ceiling(boundBox.Width))
            Dim height As Integer = CInt(Math.Ceiling(boundBox.Height))

            Using sKBitmap As New SKBitmap(width, height)
                parms.CancelProc = New SampleCancelProc()
                parms.ProgressProc = New SampleRenderProgressProc()
                pg.DrawContentsToSKBitmap(sKBitmap, parms)
                SaveBitmap(sKBitmap, "Bitmap")
            End Using
        End Sub

        Shared Sub Main(args As String())
            Console.WriteLine("DrawToBitmap Sample")

            Try
                Using library As New Library()
                    Console.WriteLine("Initialized the library.")

                    Dim sInput As String = Library.ResourceDirectory & "Sample_Input/ducky.pdf"

                    If args.Length > 0 Then
                        sInput = args(0)
                    End If

                    Console.WriteLine("Input file: " & sInput)

                    Using doc As New Document(sInput)
                        Using pg As Page = doc.GetPage(0)
                            '
                            ' Must tumble the page to get from PDF with origin at lower left
                            ' to a Bitmap with the origin at upper left.
                            '

                            Dim scaleFactor As Double = resolution / 72.0
                            Dim width As Double = (pg.MediaBox.Width * scaleFactor)
                            Dim height As Double = (pg.MediaBox.Height * scaleFactor)

                            'When the MediaBox's origin isn't at the lower-left of the page we can't use the 'Height' member and
                            'instead used the 'Top'.
                            Dim ty As Double = pg.MediaBox.Height
                            If pg.MediaBox.Bottom <> 0 Then
                                ty = pg.MediaBox.Top
                            End If

                            Dim matrix As New Matrix()
                            matrix.Scale(scaleFactor, -scaleFactor).Translate(0, -ty)

                            Dim enableBlackPointCompensation As Boolean = True

                            Using parms As DrawParams = ConstructDrawParams(matrix, pg.MediaBox, enableBlackPointCompensation)
                                ' Demonstrate drawing to Bitmaps with params and OCGs
                                ' Demonstrate drawing layers
                                Console.WriteLine("DrawLayersToBitmap: {0} {1} {2}", parms.Matrix, parms.UpdateRect.Width, parms.UpdateRect.Height)
                                DrawLayersToBitmap(doc, pg, parms) ' Will NOT drive SampleRenderProgressProc

                                ' Make a Bitmap using DrawParams with black point compensation turned on
                                Console.WriteLine("DrawToBitmapWithDrawParams: {0} {1} {2}", parms.Matrix, parms.UpdateRect.Width, parms.UpdateRect.Height)
                                DrawToBitmapWithDrawParams(pg, parms) ' Will drive SampleRenderProgress(Cancel)Proc
                            End Using
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                Console.WriteLine("An exception occurred. Here is the related information:")
                Console.Write(ex.ToString())
            End Try
        End Sub
    End Class
End Namespace
