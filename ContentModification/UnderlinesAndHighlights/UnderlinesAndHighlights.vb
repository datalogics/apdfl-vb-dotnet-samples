Imports System
Imports System.Collections.Generic
Imports Datalogics.PDFL





''' This program shows how to add annotations to an existing PDF file that will highlight and underline words.
''' When you run it, the program generates a PDF output file. The output sample annotates a PDF file showing
''' a National Weather Service web page, highlighting the word “Cloudy” wherever it appears and underlining
''' the word “Rain.”
''' 
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.

Namespace UnderlinesAndHighlights
    Public Class UnderlinesAndHighlights
        Public Shared Sub Main(args As String())
            Console.WriteLine("UnderlinesAndHighlights Sample:")

            ' ReSharper disable once UnusedVariable
            Using library As New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/sample.pdf"
                Dim sOutput As String = "UnderlinesAndHighlights-out.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                If args.Length > 1 Then
                    sOutput = args(1)
                End If

                Dim doc As New Document(sInput)

                Console.WriteLine("Opened a document " & sInput)

                Dim docpage As Page = doc.GetPage(0)

                '
                ' Highlight occurrences of the word "cloudy" on the page.
                ' Underline occurrences of the word "rain" on the page.
                '
                ' For a more in-depth example of using the WordFinder, see the TextExtract sample.
                '
                Dim cloudyQuads As New List(Of Quad)()
                Dim rainQuads As New List(Of Quad)()
                Dim wfc As New WordFinderConfig()
                Dim wf As New WordFinder(doc, WordFinderVersion.Latest, wfc)
                Dim words As IList(Of Word) = wf.GetWordList(docpage.PageNumber)
                For Each w As Word In words
                    ' Store the Quads of all "Cloudy" words in a list for later use in
                    ' creating the annotation.
                    If w.Text.ToLower().Equals("cloudy") OrElse
                        ((w.Attributes And WordAttributeFlags.HasTrailingPunctuation) =
                         WordAttributeFlags.HasTrailingPunctuation AndAlso
                         w.Text.ToLower().StartsWith("cloudy")) Then
                        cloudyQuads.AddRange(w.Quads)
                    End If

                    ' Store the Quads of all "Rain" words
                    If w.Text.ToLower().Equals("rain") OrElse
                        ((w.Attributes And WordAttributeFlags.HasTrailingPunctuation) =
                         WordAttributeFlags.HasTrailingPunctuation AndAlso
                         w.Text.ToLower().StartsWith("rain")) Then
                        rainQuads.AddRange(w.Quads)
                    End If
                Next

                Dim highlights As New HighlightAnnotation(docpage, cloudyQuads)
                highlights.Color = New Color(1.0, 0.75, 1.0)
                highlights.NormalAppearance = highlights.GenerateAppearance()

                Dim underlines As New UnderlineAnnotation(docpage, rainQuads)
                underlines.Color = New Color(0.0, 0.0, 0.0)
                underlines.NormalAppearance = underlines.GenerateAppearance()

                ' Read back the text that was annotated.
                Console.WriteLine("Cloudy text: {0}", highlights.GetAnnotatedText(True))
                Console.WriteLine("Rainy text: {0}", underlines.GetAnnotatedText(False))

                doc.Save(SaveFlags.Full, sOutput)
            End Using
        End Sub
    End Class
End Namespace

