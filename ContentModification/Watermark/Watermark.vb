Imports System
Imports Datalogics.PDFL




''' The Watermark sample program shows how to create a watermark And copy it to a New PDF file.
''' You could use this code to create a message to apply to PDF files you select, Like
''' “Confidential” Or “Draft Copy.” Or you might want to place a copyright statement over
''' a set of photographs shown in a PDF file so that they cannot be easily duplicated without
''' the permission of the owner.
''' 
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.

Namespace Watermark
    Class Watermark
        Public Shared Sub Main(args As String())
            Console.WriteLine("Watermark Sample:")

            ' ReSharper disable once UnusedVariable
            Using library As New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/sample.pdf"
                Dim sWatermark As String = Library.ResourceDirectory & "Sample_Input/ducky.pdf"
                Dim sOutput As String = "Watermark-out.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                If args.Length > 1 Then
                    sWatermark = args(1)
                End If

                If args.Length > 2 Then
                    sOutput = args(2)
                End If

                Console.WriteLine("Adding watermark from " & sWatermark & " to " & sInput & " and saving to " & sOutput)

                Dim doc As New Document(sInput)

                Dim watermarkDoc As New Document(sWatermark)

                Dim watermarkParams As New WatermarkParams()
                watermarkParams.Opacity = 0.8F
                watermarkParams.Rotation = 45.3F
                watermarkParams.Scale = 0.5F
                watermarkParams.TargetRange.PageSpec = PageSpec.EvenPagesOnly

                doc.Watermark(watermarkDoc.GetPage(0), watermarkParams)

                watermarkParams.TargetRange.PageSpec = PageSpec.OddPagesOnly

                Dim watermarkTextParams As New WatermarkTextParams()
                Dim color As New Color(109.0F / 255.0F, 15.0F / 255.0F, 161.0F / 255.0F)
                watermarkTextParams.Color = color

                watermarkTextParams.Text = "Multiline" & vbCrLf & "Watermark"

                Dim f As New Font("Courier", FontCreateFlags.Embedded Or FontCreateFlags.Subset)
                watermarkTextParams.Font = f
                watermarkTextParams.TextAlign = HorizontalAlignment.Center

                doc.Watermark(watermarkTextParams, watermarkParams)

                doc.EmbedFonts()
                doc.Save(SaveFlags.Full Or SaveFlags.Linearized, sOutput)
            End Using
        End Sub
    End Class
End Namespace

