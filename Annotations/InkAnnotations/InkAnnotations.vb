Imports Datalogics.PDFL

'
'
' This sample creates And adds a New Ink annotation to a PDF document. An Ink annotation Is a freeform line,
' similar to what you would create with a pen, Or with a stylus on a mobile device.
'
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'

Namespace InkAnnotations
    Class InkAnnotations
        Shared Sub Main()
            Console.WriteLine("InkAnnotations Sample:")

            Using (New Library())
                Console.WriteLine("Initialized the library.")

                ' Create a new document and blank first page
                Dim doc As New Document()
                Dim rect As New Rect(0, 0, 612, 792)
                Dim page As Page = doc.CreatePage(Document.BeforeFirstPage, rect)
                Console.WriteLine("Created new document and first page.")

                ' Create and add a new InkAnnotation to the 0th element of first page's annotation array
                Dim inkAnnot As New InkAnnotation(page, rect, -1)
                Console.WriteLine("Created new InkAnnotation as 0th element of annotation array.")

                ' Ask how many scribbles are in the ink annotation
                Console.WriteLine("Number of scribbles in ink annotation: " & inkAnnot.NumScribbles)

                ' Create a vector of scribble vertices
                Dim scribble As New List(Of Point)()
                Dim p As New Point(100, 100)
                scribble.Add(p)
                p = New Point(200, 300)
                scribble.Add(p)
                p = New Point(400, 200)
                scribble.Add(p)
                Console.WriteLine("Created an array of scribble points.")

                ' Add the scribble to the ink annotation
                inkAnnot.AddScribble(scribble)
                Console.WriteLine("Added the scribble to the ink annotation.")

                ' Ask how many scribbles are in the ink annotation
                Console.WriteLine("Number of scribbles in ink annotation: " & inkAnnot.NumScribbles)

                ' Create another vector of scribble vertices
                scribble = New List(Of Point)()
                p = New Point(200, 200)
                scribble.Add(p)
                p = New Point(200, 300)
                scribble.Add(p)
                p = New Point(300, 300)
                scribble.Add(p)
                p = New Point(300, 200)
                scribble.Add(p)
                p = New Point(200, 100)
                scribble.Add(p)
                Console.WriteLine("Created another array of scribble points.")

                ' Add the scribble to the ink annotation
                inkAnnot.AddScribble(scribble)
                Console.WriteLine("Added the scribble to the ink annotation.")

                ' Create another vector of scribble vertices
                scribble = New List(Of Point)()
                p = New Point(300, 400)
                scribble.Add(p)
                p = New Point(200, 300)
                scribble.Add(p)
                p = New Point(300, 300)
                scribble.Add(p)
                Console.WriteLine("Created another array of scribble points.")

                ' Add the scribble to the ink annotation
                inkAnnot.AddScribble(scribble)
                Console.WriteLine("Added the scribble to the ink annotation.")

                ' Ask how many scribbles are in the ink annotation
                Console.WriteLine("Number of scribbles in ink annotation: " & inkAnnot.NumScribbles)

                ' Get and display the points in ink annotation 0
                Dim scribbleToGet As IList(Of Point) = inkAnnot.GetScribble(0)
                For i As Integer = 0 To scribbleToGet.Count - 1
                    Console.WriteLine($"Scribble 0, point{i} : {scribbleToGet(i)}")
                Next

                ' Get and display the points in ink annotation 1
                scribbleToGet = New List(Of Point)()
                scribbleToGet = inkAnnot.GetScribble(1)
                For i As Integer = 0 To scribbleToGet.Count - 1
                    Console.WriteLine($"Scribble 1, point{i} : {scribbleToGet(i)}")
                Next

                ' Let's set the color and then ask the ink annotation to generate an appearance stream
                Dim color As New Color(0.5, 0.3, 0.8)
                inkAnnot.Color = color
                Console.WriteLine("Set the stroke color.")
                Dim form As Form = inkAnnot.GenerateAppearance()
                inkAnnot.NormalAppearance = form
                Console.WriteLine("Generated the appearance stream.")

                ' Update the page's content and save the file with clipping
                page.UpdateContent()
                doc.Save(SaveFlags.Full, "InkAnnotations-out1.pdf")
                Console.WriteLine("Saved InkAnnotations-out1.pdf")

                ' Remove 0th scribble
                inkAnnot.RemoveScribble(0)
                ' Ask how many scribbles are in the ink annotation
                Console.WriteLine("Number of scribbles in ink annotation: " & inkAnnot.NumScribbles)

                ' Generate a new appearance stream, since we have removed the first scribble.
                inkAnnot.NormalAppearance = inkAnnot.GenerateAppearance()

                ' Update the page's content and save the file with clipping
                page.UpdateContent()
                doc.Save(SaveFlags.Full, "InkAnnotations-out2.pdf")

                ' Dispose the doc object
                doc.Dispose()
                Console.WriteLine("Disposed document object.")
            End Using
        End Sub
    End Class
End Namespace
