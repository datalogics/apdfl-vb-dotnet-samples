Imports Datalogics.PDFL
Imports System

'
' 
' This sample demonstrates working with Clip objects. A clipping path Is used to edit the borders of a graphics object.
'
' 
' Copyright (c) 2007-2023, Datalogics, Inc. All rights reserved.
'
'/
Namespace Clips
    Module Clips
        Sub Main(args As String())
            System.Console.WriteLine("Clips Sample:")

            Using (New Library())
                Console.WriteLine("Initialized the Library")
                Dim sOutput As String = "Clips-out.pdf"
                If (args.Length > 0) Then
                    sOutput = args(0)
                End If
                Console.WriteLine("Output file: " + sOutput)

                ' Create a new document and blank first page
                Dim doc As Document = New Document()
                Dim rect As Rect = New Rect(0, 0, 612, 792)
                Dim page As Page = doc.CreatePage(Document.BeforeFirstPage, rect)
                Console.WriteLine("Created new document and first page.")

                ' Create a new path, set up its graphic state and PaintOp
                Dim path As Datalogics.PDFL.Path = New Datalogics.PDFL.Path()
                Dim gs As GraphicState = New GraphicState()
                Dim color As Color = New Color(0.0, 0.0, 0.0)
                gs.FillColor = color
                path.GraphicState = gs
                path.PaintOp = PathPaintOpFlags.Fill

                ' Add a rectangle to the path
                Dim point As Point = New Point(100, 500)
                path.AddRect(point, 300, 200)
                Console.WriteLine("Created new path and added rectangle to it.")

                ' Add a curve to the path too so we have something
                ' interesting to look at after clipping
                Dim linePoint1 As Point = New Point(400, 450)
                Dim linePoint2 As Point = New Point(350, 300)
                path.AddCurveV(linePoint1, linePoint2)
                Console.WriteLine("Added curve to the path.")

                ' Add the path to the page in the document
                Dim content As Content = page.Content
                content.AddElement(path)
                Console.WriteLine("Added path to page in document.")

                ' Create a new path and add a rectangle to it
                Dim clipPath As Datalogics.PDFL.Path = New Datalogics.PDFL.Path()
                point = New Point(50, 300)
                clipPath.AddRect(point, 300, 250)
                Console.WriteLine("Created clipping path and added rectangle to it.")

                ' Create a new clip and add the new path to it and then add
                ' this new clip to the original path as its Clip property
                Dim clip As Clip = New Clip()
                clip.AddElement(clipPath)
                path.Clip = clip
                Console.WriteLine("Created new clip, assigned clipping path to it, and added new clip to original path.")

                ' Update the page's content and save the file with clipping
                page.UpdateContent()
                doc.Save(SaveFlags.Full, sOutput)

                ' Dispose the doc object
                doc.Dispose()
                Console.WriteLine("Disposed document object.")
            End Using
        End Sub
    End Module
End Namespace
