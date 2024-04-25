Imports Datalogics.PDFL

'
'
' This sample shows how to redact a PDF document. The program opens an input PDF, searches for
' specific words using the Adobe PDF Library WordFinder, and then removes these words from the text.
'
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'
Namespace Redactions
    Module Redactions
        Sub Main(args As String())
            Console.WriteLine("Redactions Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")
                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/sample.pdf"
                Dim sOutput1 As String = "Redactions-out.pdf"
                Dim sOutput2 As String = "Redactions-out-applied.pdf"

                If (args.Length > 0) Then
                    sInput = args(0)
                End If

                Console.WriteLine("Input file: " + sInput)

                Dim doc As Document = New Document(sInput)

                Dim docpage As Page = doc.GetPage(0)
                '
                ' Redact occurrences of the word "rain" on the page.
                ' Redact occurrences of the word "cloudy" on the page, changing the display details.
                '
                ' For a more in-depth example of using the WordFinder, see the TextExtract sample.
                '

                Dim cloudyQuads As List(Of Quad) = New List(Of Quad)()

                Dim rainQuads As List(Of Quad) = New List(Of Quad)()

                Dim wordConfig As WordFinderConfig = New WordFinderConfig()
                Dim wf As WordFinder = New WordFinder(doc, WordFinderVersion.Latest, wordConfig)

                Dim words As IList(Of Word) = wf.GetWordList(docpage.PageNumber)

                For Each w As Word In words
                    Console.WriteLine(" " + w.Text.ToLower())
                    ' Store the Quads of all "Cloudy" words in a list for later use in
                    ' creating the redaction object.
                    If (w.Text.ToLower().Equals("cloudy") Or
                        ((w.Attributes & WordAttributeFlags.HasTrailingPunctuation) =
                         WordAttributeFlags.HasTrailingPunctuation And
                         w.Text.ToLower().StartsWith("cloudy"))) Then
                        cloudyQuads.AddRange(w.Quads)
                    End If

                    ' Store the Quads of all "Rain" words
                    If (w.Text.ToLower().Equals("rain") Or
                        ((w.Attributes & WordAttributeFlags.HasTrailingPunctuation) =
                         WordAttributeFlags.HasTrailingPunctuation And
                         w.Text.ToLower().StartsWith("rain"))) Then
                        rainQuads.AddRange(w.Quads)
                    End If
                Next

                Console.WriteLine("Found Cloudy instances: " + CStr(cloudyQuads.Count))
                Dim red As Color = New Color(1.0, 0.0, 0.0)
                Dim white As Color = New Color(1.0)

                Dim not_cloudy As Redaction = New Redaction(docpage, cloudyQuads, red)

                ' fill the "normal" appearance with 20% red '/
                not_cloudy.FillNormal = True
                not_cloudy.SetFillColor(red, 0.25)

                Console.WriteLine("Found rain instances: " + CStr(rainQuads.Count))
                Dim no_rain As Redaction = New Redaction(docpage, rainQuads)
                no_rain.InternalColor = New Color(0.0, 1.0, 0.0)

                ' Fill the redaction with the word "rain", drawn in white '/
                no_rain.OverlayText = "rain"
                no_rain.Repeat = True
                no_rain.ScaleToFit = True
                no_rain.TextColor = white
                no_rain.FontFace = "CourierStd"
                no_rain.FontSize = 8.0

                doc.Save(SaveFlags.Full, sOutput1)

                Console.WriteLine("Wrote a pdf doc with unapplied redactions.")

                ' actually all the redactions in the document
                doc.ApplyRedactions()

                doc.Save(SaveFlags.Full, sOutput2)

                Console.WriteLine("Wrote a redacted pdf doc.")
            End Using
        End Sub
    End Module
End Namespace
