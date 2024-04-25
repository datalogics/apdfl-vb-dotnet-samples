Imports Datalogics.PDFL

'
' This sample searches for And lists the contents of paths found in an existing PDF document.
' Paths in PDF documents, Or clipping paths, define the boundaries for art Or graphics.
' 
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'

Namespace ListPaths
    Module ListPaths
        Sub Main(args As String())
            Console.WriteLine("ListLayers Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/sample.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                Console.WriteLine("Input file: " + sInput)

                Using doc As Document = New Document(sInput)
                    ListPathsInDocument(doc)
                End Using
            End Using
        End Sub

        Private Sub ListPathsInDocument(doc As Document)
            For pgno As Integer = 0 To doc.NumPages - 1
                ListPathsInContent(doc.GetPage(pgno).Content, pgno)
            Next
        End Sub

        Private Sub ListPathsInContent(content As Content, pgno As Integer)
            For i As Integer = 0 To content.NumElements - 1
                Dim e As Element = content.GetElement(i)

                If e.GetType() Is GetType(Datalogics.PDFL.Path) Then
                    ListPath(DirectCast(e, Datalogics.PDFL.Path), pgno)
                ElseIf e.GetType() Is GetType(Container) Then
                    Console.WriteLine("Recurring through a Container")
                    ListPathsInContent(DirectCast(e, Datalogics.PDFL.Container).Content, pgno)
                ElseIf e.GetType() Is GetType(Container) Then
                    Console.WriteLine("Recurring through a Group")
                    ListPathsInContent(DirectCast(e, Datalogics.PDFL.Group).Content, pgno)
                ElseIf e.GetType() Is GetType(Container) Then
                    Console.WriteLine("Recurring through a Form")
                    ListPathsInContent(DirectCast(e, Datalogics.PDFL.Form).Content, pgno)
                End If
            Next
        End Sub

        Private Sub ListPath(path As Datalogics.PDFL.Path, pgno As Integer)
            Dim segments As IList(Of Segment) = path.Segments
            Console.WriteLine("Path on page {0}:", pgno)
            Console.WriteLine("Transformation matrix: {0}", path.Matrix)
            For Each segment As Segment In segments
                If segment.GetType() Is GetType(MoveTo) Then
                    Dim moveto As MoveTo = DirectCast(segment, MoveTo)
                    Console.WriteLine("  MoveTo x={0}, y={1}", moveto.Point.H, moveto.Point.V)
                ElseIf segment.GetType() Is GetType(LineTo) Then
                    Dim lineto As LineTo = DirectCast(segment, LineTo)
                    Console.WriteLine("  LineTo x={0}, y={1}",
                        lineto.Point.H, lineto.Point.V)
                ElseIf segment.GetType() Is GetType(CurveTo) Then
                    Dim curveto As CurveTo = DirectCast(segment, CurveTo)
                    Console.WriteLine("  CurveTo x1={0}, y1={1}, x2={2}, y2={3}, x3={4}, y3={5}",
                        curveto.Point1.H, curveto.Point1.V,
                        curveto.Point2.H, curveto.Point2.V,
                        curveto.Point3.H, curveto.Point3.V)
                ElseIf segment.GetType() Is GetType(CurveToV) Then
                    Dim curveto As CurveToV = DirectCast(segment, CurveToV)
                    Console.WriteLine("  CurveToV x2={0}, y2={1}, x3={2}, y3={3}",
                        curveto.Point2.H, curveto.Point2.V,
                        curveto.Point3.H, curveto.Point3.V)
                ElseIf segment.GetType() Is GetType(CurveToY) Then
                    Dim curveto As CurveToY = DirectCast(segment, CurveToY)
                    Console.WriteLine("  CurveToV x1={0}, y1={1}, x3={2}, y3={3}",
                        curveto.Point1.H, curveto.Point1.V,
                        curveto.Point3.H, curveto.Point3.V)
                ElseIf segment.GetType() Is GetType(RectSegment) Then
                    Dim rect As RectSegment = DirectCast(segment, RectSegment)
                    Console.WriteLine("  Rectangle x={0}, y={1}, width={2}, height={3}",
                        rect.Point.H, rect.Point.V,
                        rect.Width, rect.Height)
                ElseIf segment.GetType() Is GetType(ClosePath) Then
                    Console.WriteLine("  ClosePath")
                End If
            Next
        End Sub
    End Module
End Namespace
