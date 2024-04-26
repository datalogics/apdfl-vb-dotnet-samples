Imports System
Imports Datalogics.PDFL


''' The Graphics State is an internal data structure in a PDF file that holds the parameters that describe graphics
''' within that file. These parameters define how individual graphics are presented on the page. Adobe Systems introduced
''' the Extended Graphic State to expand the original Graphics State data structure, providing space to define and store
''' more data objects within a PDF.
''' 
''' This sample program shows how to use the Extended Graphic State object to add graphics parameters to an image.
''' 
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.


Namespace ExtendedGraphicStates

    Class ExtendedGraphicStates

        Shared Sub blendPage(doc As Document, foregroundImage As Image, backgroundImage As Image)
            Dim height As Double = 792
            Dim width As Double = 612
            Dim pageRect As New Rect(0, 0, width, height)
            Dim docpage As Page = doc.CreatePage(doc.NumPages - 1, pageRect)

            ' This section demonstrates all the Blend Modes one can achieve
            ' by setting the BlendMode property to each of the 16 enumerations
            ' on a foreground "ducky" over a background rainbow pattern, And 
            ' plopping all these images on a single page.

            Dim t As New Text()
            Dim f As Font
            Try
                f = New Font("Arial", FontCreateFlags.Embedded Or FontCreateFlags.Subset)
            Catch ex As ApplicationException
                If ex.Message.Equals("The specified font could not be found.") AndAlso
           System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) AndAlso
           Not System.IO.Directory.Exists("/usr/share/fonts/msttcore/") Then
                    Console.WriteLine("Please install Microsoft Core Fonts on Linux first.")
                    Return
                End If
                Throw
            End Try

            Dim gsText As New GraphicState()
            gsText.FillColor = New Color(0, 0, 1.0)
            Dim ts As New TextState()

            For i As Integer = 0 To 15
                Dim individualForegroundImage As Image = foregroundImage.Clone()
                Dim individualBackgroundImage As Image = backgroundImage.Clone()

                Dim gs As GraphicState = individualForegroundImage.GraphicState
                individualForegroundImage.Scale(0.125, 0.125)
                individualForegroundImage.Translate(800, 200 + height * (7 - i))
                individualBackgroundImage.Scale(0.125, 0.125)
                individualBackgroundImage.Translate(800, 200 + height * (7 - i))

                ' Halfway through, create 2nd column by shifting over And up
                If i > 7 Then
                    individualForegroundImage.Translate(2400, height * 8)
                    individualBackgroundImage.Translate(2400, height * 8)
                End If

                docpage.Content.AddElement(individualBackgroundImage)
                Console.WriteLine("Added background image " & (i + 1) & " to the content.")
                docpage.Content.AddElement(individualForegroundImage)
                Console.WriteLine("Added foreground image " & (i + 1) & " to the content.")

                Dim m As New Matrix()
                If i > 7 Then
                    m = m.Translate(480, 750 - ((i - 8) * 100)) ' second column
                Else
                    m = m.Translate(180, 750 - (i * 100)) ' first column
                End If
                m = m.Scale(12.0, 12.0)

                Dim xgs As New ExtendedGraphicState()
                Dim tr As TextRun = Nothing
                Select Case i
                    Case 0
                        xgs.BlendMode = BlendMode.Normal
                        tr = New TextRun("Normal", f, gsText, ts, m)
                    Case 1
                        xgs.BlendMode = BlendMode.Multiply
                        tr = New TextRun("Multiply", f, gsText, ts, m)
                    Case 2
                        xgs.BlendMode = BlendMode.Screen
                        tr = New TextRun("Screen", f, gsText, ts, m)
                    Case 3
                        xgs.BlendMode = BlendMode.Overlay
                        tr = New TextRun("Overlay", f, gsText, ts, m)
                    Case 4
                        xgs.BlendMode = BlendMode.Darken
                        tr = New TextRun("Darken", f, gsText, ts, m)
                    Case 5
                        xgs.BlendMode = BlendMode.Lighten
                        tr = New TextRun("Lighten", f, gsText, ts, m)
                    Case 6
                        xgs.BlendMode = BlendMode.ColorDodge
                        tr = New TextRun("Color Dodge", f, gsText, ts, m)
                    Case 7
                        xgs.BlendMode = BlendMode.ColorBurn
                        tr = New TextRun("Color Burn", f, gsText, ts, m)
                    Case 8
                        xgs.BlendMode = BlendMode.HardLight
                        tr = New TextRun("Hard Light", f, gsText, ts, m)
                    Case 9
                        xgs.BlendMode = BlendMode.SoftLight
                        tr = New TextRun("SoftLight", f, gsText, ts, m)
                    Case 10
                        xgs.BlendMode = BlendMode.Difference
                        tr = New TextRun("Difference", f, gsText, ts, m)
                    Case 11
                        xgs.BlendMode = BlendMode.Exclusion
                        tr = New TextRun("Exclusion", f, gsText, ts, m)
                    Case 12
                        xgs.BlendMode = BlendMode.Hue
                        tr = New TextRun("Hue", f, gsText, ts, m)
                    Case 13
                        xgs.BlendMode = BlendMode.Saturation
                        tr = New TextRun("Saturation", f, gsText, ts, m)
                    Case 14
                        xgs.BlendMode = BlendMode.Color
                        tr = New TextRun("Color", f, gsText, ts, m)
                    Case 15
                        xgs.BlendMode = BlendMode.Luminosity
                        tr = New TextRun("Luminosity", f, gsText, ts, m)
                    Case Else
                        ' Handle the default case if none of the above cases match
                End Select

                t.AddRun(tr)
                docpage.Content.AddElement(t)
                docpage.UpdateContent()
                Console.WriteLine("Updated the content on page 1.")

                gs.ExtendedGraphicState = xgs
                individualForegroundImage.GraphicState = gs
                Console.WriteLine("Set blend mode in extended graphic state.")
            Next
        End Sub


        Shared Sub Main(args As String())
            Console.WriteLine("ExtendedGraphicStates Sample:")

            Using library As New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput1 As String = Library.ResourceDirectory & "Sample_Input/ducky_alpha.tif"
                Dim sInput2 As String = Library.ResourceDirectory & "Sample_Input/rainbow.tif"
                Dim sOutput As String = "ExtendedGraphicStates-out.pdf"

                If args.Length > 0 Then
                    sInput1 = args(0)
                End If

                If args.Length > 1 Then
                    sInput2 = args(1)
                End If

                If args.Length > 2 Then
                    sOutput = args(2)
                End If

                Console.WriteLine("Input files: " & sInput1 & " and " & sInput2 & ". Saving to output file: " &
                                  sOutput)

                Dim doc As New Document()

                Dim ImageOne As New Image(sInput1, doc)
                Dim imageTwo As New Image(sInput2, doc)

                blendPage(doc, ImageOne, imageTwo)

                blendPage(doc, imageTwo, ImageOne)

                doc.EmbedFonts()
                doc.Save(SaveFlags.Full, sOutput)
            End Using
        End Sub

    End Class
End Namespace

