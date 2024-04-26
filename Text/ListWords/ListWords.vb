Imports Datalogics.PDFL

'
' 
' This sample lists the text For the words in a PDF document.
' 
' 
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'
Namespace ListWords
    Module ListWords
        Sub Main(args As String())
            Console.WriteLine("ListWords Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/sample.pdf"

                If (args.Length > 0) Then
                    sInput = args(0)
                End If

                Console.WriteLine("Input file: " + sInput)

                Dim doc As Document = New Document(sInput)

                Dim nPages As Integer = doc.NumPages

                Dim wordConfig As WordFinderConfig = New WordFinderConfig()
                wordConfig.IgnoreCharGaps = True
                wordConfig.IgnoreLineGaps = False
                wordConfig.NoAnnots = True
                wordConfig.NoEncodingGuess = True ' leave non-Roman single-byte font alone

                ' Std Roman treatment For custom encoding overrides the noEncodingGuess option
                wordConfig.UnknownToStdEnc = False

                wordConfig.DisableTaggedPDF = True ' legacy mode WordFinder creation
                wordConfig.NoXYSort = False
                wordConfig.PreserveSpaces = False
                wordConfig.NoLigatureExp = False
                wordConfig.NoHyphenDetection = False
                wordConfig.TrustNBSpace = False
                wordConfig.NoExtCharOffset = False ' text extraction efficiency
                wordConfig.NoStyleInfo = False ' text extraction efficiency

                Dim wordFinder As WordFinder = New WordFinder(doc, WordFinderVersion.Latest, wordConfig)
                Dim pageWords As IList(Of Word) = New List(Of Word)
                For i As Integer = 0 To nPages - 1
                    pageWords = wordFinder.GetWordList(i)
                    For Each wInfo As Word In pageWords
                        Dim s As String = wInfo.Text
                        Dim QuadList As IList(Of Quad) = wInfo.Quads

                        For Each Q As Quad In QuadList
                            Console.WriteLine(Q)
                        Next

                        For Each st As StyleTransition In wInfo.StyleTransitions
                            Console.WriteLine(st)
                        Next

                        Dim styleList As IList(Of StyleTransition) = wInfo.StyleTransitions
                        For Each st As StyleTransition In styleList
                            Console.WriteLine(st)
                        Next

                        Console.WriteLine(wInfo.Attributes)
                        Console.WriteLine(s)
                    Next
                Next

                Console.WriteLine("Pages=" + CStr(nPages))
            End Using
        End Sub
    End Module
End Namespace
