Imports Datalogics.PDFL

'
'
' This program generates a PDF output file with a polygon shape (a triangle) as an annotation to the file.
' The program defines the vertices for the outlines of the annotation, And the line And fill colors.
'
' Copyright(c) 2007-204, Datalogics, Inc. All rights reserved.

'

Namespace PolygonAnnotations
    Class PolygonAnnotations
        Shared Sub Main(args As String())
            Console.WriteLine("PolygonAnnotation Sample:")

            Using (New Library())
                Console.WriteLine("Initialized the library.")

                Dim sOutput As String = "PolygonAnnotations-out.pdf"

                If args.Length > 0 Then
                    sOutput = args(0)
                End If

                Console.WriteLine("Writing to output " & sOutput)

                ' Create a new document and blank first page
                Dim doc As New Document()
                Dim rect As New Rect(0, 0, 612, 792)
                Dim page As Page = doc.CreatePage(Document.BeforeFirstPage, rect)
                Console.WriteLine("Created new document and first page.")

                ' Create a list of polygon vertices
                Dim vertices As New List(Of Point)()
                Dim p As New Point(100, 100)
                vertices.Add(p)
                p = New Point(200, 300)
                vertices.Add(p)
                p = New Point(400, 200)
                vertices.Add(p)
                Console.WriteLine("Created an array of vertex points.")

                ' Create and add a new PolygonAnnotation to the 0th element of the first page's annotation array
                Dim polygonAnnot As New PolygonAnnotation(page, rect, vertices, -1)
                Console.WriteLine("Created new PolygonAnnotation as 0th element of annotation array.")

                ' Now let's retrieve and display the vertices
                Dim vertices2 As IList(Of Point) = polygonAnnot.Vertices
                Console.WriteLine("Retrieved the vertices of the polygon annotation.")
                Console.WriteLine("They are:")
                For i As Integer = 0 To vertices2.Count - 1
                    Console.WriteLine("Vertex " & i & ": " & vertices2(i).ToString())
                Next

                ' Let's set some colors and then ask the polygon to generate an appearance stream
                Dim color As New Color(0.5, 0.3, 0.8)
                polygonAnnot.InteriorColor = color
                color = New Color(0.9, 0.7, 0.1)
                polygonAnnot.Color = color
                Console.WriteLine("Set the stroke and fill colors.")
                Dim form As Form = polygonAnnot.GenerateAppearance()
                polygonAnnot.NormalAppearance = form
                Console.WriteLine("Generated the appearance stream.")

                ' Update the page's content and save the file with clipping
                page.UpdateContent()
                doc.Save(SaveFlags.Full, sOutput)

                ' Dispose the doc object
                doc.Dispose()
                Console.WriteLine("Disposed document object.")
            End Using
        End Sub
    End Class
End Namespace
