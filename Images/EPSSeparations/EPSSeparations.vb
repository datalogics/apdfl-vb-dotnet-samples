Imports System
Imports System.Collections.Generic
Imports Datalogics.PDFL



''' This sample demonstrates working with color separations with Encapsulated PostScript (EPS) graphics
''' from a PDF file.
'''
''' Copyright (c) 2024, Datalogics, Inc. All rights reserved.

Namespace EPSSeparations
    Class EPSSeparations
        Shared Sub Main(args As String())
            Console.WriteLine("EPS Separations Sample:")

            ' ReSharper disable once UnusedVariable
            Using library As New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput1 As String = Library.ResourceDirectory & "Sample_Input/spotcolors1.pdf"
                Dim sInput2 As String = Library.ResourceDirectory & "Sample_Input/spotcolors.pdf"

                If args.Length > 0 Then
                    sInput1 = args(0)
                End If

                If args.Length > 1 Then
                    sInput2 = args(1)
                End If

                Console.WriteLine("Will perform simple separation on " & sInput1 & " and complex separation on " & sInput2)

                SimpleSeparationsExample(sInput1)
                ComplexSeparationsExample(sInput2)
            End Using
        End Sub

        Shared Sub SimpleSeparationsExample(input As String)
            Dim doc As New Document(input)

            Console.WriteLine("Opened " & input)

            For pgNum As Integer = 0 To doc.NumPages - 1
                Dim pg As Page = doc.GetPage(pgNum)

                Dim pageinks As IList(Of Ink) = pg.ListInks()
                Dim plates As New List(Of SeparationPlate)()

                For Each i As Ink In pageinks
                    Console.WriteLine("Found color " & i.ColorantName & " on page " & (pgNum + 1) & ".")
                    Dim newplate As New SeparationPlate(i,
                        New System.IO.FileStream("Simple-Pg" & (pgNum + 1) & "-" & i.ColorantName & ".eps",
                            System.IO.FileMode.Create))
                    plates.Add(newplate)
                Next

                Dim parms As New SeparationParams(plates)

                Console.WriteLine("Making separations for page " & (pgNum + 1) & "...")
                pg.MakeSeparations(parms)

                For Each p As SeparationPlate In plates
                    p.EPSOutput.Close()
                Next
            Next
        End Sub

        Shared Sub ComplexSeparationsExample(input As String)
            Dim doc As New Document(input)

            Console.WriteLine("Opened " & input)

            Dim pg As Page = doc.GetPage(0)
            Dim pageinks As IList(Of Ink) = pg.ListInks()
            Dim plates As IList(Of SeparationPlate) = New List(Of SeparationPlate)()
            Dim colors As New Dictionary(Of String, SeparationPlate)()

            For Each i As Ink In pageinks
                Console.WriteLine("Found color " & i.ColorantName & " on page 1.")

                If i.ColorantName = "PANTONE 7442 C" Then
                    i.Density = 0.75
                ElseIf i.ColorantName = "PANTONE 7467 C" Then
                    i.Density = 0.5
                End If

                Dim newplate As New SeparationPlate(i,
                    New System.IO.FileStream("Complex-Pg1-" & i.ColorantName & ".eps", System.IO.FileMode.Create))
                plates.Add(newplate)
                colors.Add(i.ColorantName, newplate)
            Next

            Dim parms As New SeparationParams(plates)

            Console.WriteLine("Making separations for page 1...")
            pg.MakeSeparations(parms)

            For Each p As SeparationPlate In plates
                p.EPSOutput.Close()
            Next

            For pgNum As Integer = 1 To doc.NumPages - 1
                pg = doc.GetPage(pgNum)
                pageinks = pg.ListInks()
                plates = New List(Of SeparationPlate)()

                For Each i As Ink In pageinks
                    Dim newplate As SeparationPlate

                    If colors.ContainsKey(i.ColorantName) Then
                        newplate = colors(i.ColorantName)
                        newplate.EPSOutput = New System.IO.FileStream(
                            "Complex-Pg" & (pgNum + 1) & "-" & i.ColorantName & ".eps", System.IO.FileMode.Create)
                    Else
                        newplate = New SeparationPlate(i,
                            New System.IO.FileStream("Complex-Pg" & (pgNum + 1) & "-" & i.ColorantName & ".eps",
                                System.IO.FileMode.Create))
                        colors.Add(i.ColorantName, newplate)
                    End If

                    plates.Add(newplate)
                Next

                parms.Plates = plates

                Console.WriteLine("Making separations for page " & (pgNum + 1) & "...")
                pg.MakeSeparations(parms)

                For Each p As SeparationPlate In plates
                    Dim emptyFile As Boolean = False

                    If p.EPSOutput.Length = 0 Then
                        emptyFile = True
                    End If

                    p.EPSOutput.Close()

                    If emptyFile Then
                        System.IO.File.Delete("Complex-Pg" & (pgNum + 1) & "-" & p.ColorantName & ".eps")
                    End If
                Next
            Next
        End Sub
    End Class
End Namespace
