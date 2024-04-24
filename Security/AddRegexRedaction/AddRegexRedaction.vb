Imports System
Imports System.Collections.Generic
Imports Datalogics.PDFL

 '
 ' This sample demonstrates using the DocTextFinder to find examples of a specific phrase in a PDF
 ' document that matches a user-supplied regular expression. When the sample finds the text it
 ' will redact the phrase from the output document.
 '
 ' Copyright (c) 2024, Datalogics, Inc. All rights reserved.
 '
 '
Namespace AddRegexRedaction

    Module AddRegexRedaction
    
        Sub Main(args as String())
        
            Console.WriteLine("AddRegexRedaction Sample:")

            Using (New Library())
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = "AddRegexRedaction.pdf"
                Dim sOutput1 As String = "AddRegexRedaction-out.pdf"
                Dim sOutput2 As String = "AddRegexRedaction-out-applied.pdf"

                ' Highlight and redact occurrences of the phrases that match this regular expression.
                ' Uncomment only the one you are interested in seeing displayed redacted.
                ' Phone numbers
                Dim sRegex As String = "((1-)?(\()?\d{3}(\))?(\s)?(-)?\d{3}-\d{4})"
                ' Email addresses
                'Dim sRegex As String = "(\b[\w.!#$%&'*+\/=?^`{|}~-]+@[\w-]+(?:\.[\w-]+)*\b)";
                ' URLs
                ' Dim sRegex As String = "((https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,}))"

                If (args.Length > 0) Then
                    sInput = args(0)
                End If

                Using doc As Document = New Document(sInput)
                    Dim nPages As Integer = doc.NumPages

                    Console.WriteLine("Input file:  " + sInput)

                    ' Create a WordFinder configuration
                    Dim wordConfig As WordFinderConfig = New WordFinderConfig()

                    ' Need to set this to true so phrases will be concatenated properly
                    wordConfig.NoHyphenDetection = True

                    ' Create a DocTextFinder with the default wordfinder parameters
                    Using docTextFinder As DocTextFinder = New DocTextFinder(doc, wordConfig)
                        ' Retrieve the phrases and words matching a regular expression
                        Dim docMatches As IList(Of DocTextFinderMatch)
                        docMatches = docTextFinder.GetMatchList(0, nPages - 1, sRegex)

                        ' Redaction color will be red
                        Dim red As Color = New Color(1.0, 0.0, 0.0)

                        For Each wInfo As DocTextFinderMatch In docMatches
                            ' Show the matching phrase
                            Console.WriteLine(wInfo.MatchString)

                            ' Get the quads
                            Dim QuadInfo As IList(Of DocTextFinderQuadInfo) = wInfo.QuadInfo

                            ' Iterate through the quad info and create highlights
                            For Each qInfo As DocTextFinderQuadInfo In QuadInfo
                                Dim docpage As Page = doc.GetPage(qInfo.PageNum)

                                Dim red_fill As Redaction = New Redaction(docpage, qInfo.Quads, red)

                                ' fill the "normal" appearance with 25% red 
                                red_fill.FillNormal = True
                                red_fill.SetFillColor(red, 0.25)
                            Next
                        Next
                    End Using
                    ' Save the document with the highlighted matched strings
                    doc.Save(SaveFlags.Full, sOutput1)

                    Console.WriteLine("Wrote a PDF document with unapplied redactions.")

                    ' Apply all the redactions in the document
                    doc.ApplyRedactions()

                    ' Save the document with the redacted matched strings
                    doc.Save(SaveFlags.Full, sOutput2)

                    Console.WriteLine("Wrote a redacted PDF document.")
                End Using
            End Using
        End Sub
        End Module
End Namespace