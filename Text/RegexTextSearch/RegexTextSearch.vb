Imports Datalogics.PDFL

'
' This sample demonstrates Using the DocTextFinder to find examples of a specific phrase in a PDF
' document that match a user-supplied regular expression. When the sample finds the text it
' highlights each match and saves the file as an output document.
' 
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'
Namespace RegexTextSearch
    Module RegexTextSearch
        Sub Main(args As String())
            Console.WriteLine("RegexTextSearch Sample:") '

            Using New Library()
                Console.WriteLine("Initialized the library.") '

                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/RegexTextSearch.pdf" '
                Dim sOutput As String = "RegexTextSearch-out.pdf" '

                ' Highlight occurrences of the phrases that match this regular expression.
                ' Uncomment only the one you are interested in seeing displayed with highlights.
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

                    Dim wordConfig As WordFinderConfig = New WordFinderConfig()

                    ' Need to set this to true so phrases will be concatenated properly
                    wordConfig.NoHyphenDetection = True

                    ' Create a DocTextFinder with the default wordfinder parameters
                    Using docTextFinder As DocTextFinder = New DocTextFinder(doc, wordConfig)
                        ' Retrieve the phrases matching a regular expression
                        Dim docMatches As IList(Of DocTextFinderMatch) = docTextFinder.GetMatchList(0, nPages - 1, sRegex)

                        For Each wInfo As DocTextFinderMatch In docMatches
                            ' Show the matching phrase
                            Console.WriteLine(wInfo.MatchString)

                            ' Get the quads
                            Dim QuadInfo As IList(Of DocTextFinderQuadInfo) = wInfo.QuadInfo

                            ' Iterate through the quad info and create highlights
                            For Each qInfo As DocTextFinderQuadInfo In QuadInfo
                                Dim docpage As Page = doc.GetPage(qInfo.PageNum)
                                ' Highlight the matched string words
                                Dim highlight As HighlightAnnotation = New HighlightAnnotation(docpage, qInfo.Quads)
                                highlight.NormalAppearance = highlight.GenerateAppearance()
                            Next
                        Next
                        ' Save the document with the highlighted matched strings
                        doc.Save(SaveFlags.Full, sOutput)
                    End Using
                End Using
            End Using
        End Sub
    End Module
End Namespace
