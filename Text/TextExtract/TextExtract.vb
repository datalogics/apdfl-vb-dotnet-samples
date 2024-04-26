Imports Datalogics.PDFL

'
' This program pulls text from a PDF file and exports it to a text file (TXT).
' It will open a PDF file called Constitution.PDF and create an output file called
' TextExtract-untagged-out.txt. The export file includes page number references, and
' the text is produced Using standard Times Roman encoding. The program is also written
' to include a provision For working with tagged documents, and determines If the original
' PDF file is tagged or untagged. Tagging is used to make PDF files accessible
' to the blind or to people with vision problems. 
'
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'
Namespace TextExtract
    Module TextExtract
        Sub Main(args As String())
            Console.WriteLine("TextExtract Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")

                ' This is a tagged PDF.
                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/pdf_intro.pdf"

                If (args.Length > 0) Then
                    sInput = args(0)
                End If

                ' This is an untagged PDF.
                'Resources/Sample_Input/constitution.pdf"

                Dim doc As Document = New Document(sInput)

                Console.WriteLine("Input file:  " + sInput)

                ' Determine If the PDF is tagged.  We'll use a slightly different set of rules 
                ' For parsing tagged and untagged PDFs.
                '
                ' We'll determine If the PDF is tagged by examining the MarkInfo 
                ' dictionary of the document.  First, check For the existence of the MarkInfo dict.
                Dim docIsTagged As Boolean = False
                Dim markInfoDict As PDFDict
                Dim markedEntry As PDFBoolean
                markInfoDict = CType(doc.Root.Get("MarkInfo"), PDFDict)
                If Not (markInfoDict Is Nothing) Then
                    markedEntry = CType(doc.Root.Get("Marked"), PDFBoolean)
                    If Not (markedEntry Is Nothing) Then
                        If (markedEntry.Value) Then
                            docIsTagged = True
                        End If
                    End If
                End If

                Dim wordConfig As WordFinderConfig = New WordFinderConfig()
                wordConfig.IgnoreCharGaps = False
                wordConfig.IgnoreLineGaps = False
                wordConfig.NoAnnots = False
                wordConfig.NoEncodingGuess = False

                ' Std Roman treatment For custom encoding overrides the noEncodingGuess option
                wordConfig.UnknownToStdEnc = False

                wordConfig.DisableTaggedPDF = False ' legacy mode WordFinder creation
                wordConfig.NoXYSort = True
                wordConfig.PreserveSpaces = False
                wordConfig.NoLigatureExp = False
                wordConfig.NoHyphenDetection = False
                wordConfig.TrustNBSpace = False
                wordConfig.NoExtCharOffset = False ' text extraction efficiency
                wordConfig.NoStyleInfo = False ' text extraction efficiency

                Dim wordFinder As WordFinder = New WordFinder(doc, WordFinderVersion.Latest, wordConfig)

                If (docIsTagged) Then
                    ExtractTextTagged(doc, wordFinder)
                Else
                    ExtractTextUntagged(doc, wordFinder)
                End If
            End Using
        End Sub

        Sub ExtractTextUntagged(doc As Document, wordFinder As WordFinder)
            Dim nPages As Integer = doc.NumPages
            Dim pageWords As IList(Of Word) = New List(Of Word)

            Dim logfile As System.IO.StreamWriter = New System.IO.StreamWriter("TextExtract-untagged-out.txt")
            Console.WriteLine("Writing TextExtract-untagged-out.txt")

            For i As Integer = 0 To nPages - 1
                pageWords = wordFinder.GetWordList(i)

                Dim textToExtract As String = ""

                For wordnum As Integer = 0 To pageWords.Count - 1
                    Dim wInfo As Word
                    wInfo = pageWords(wordnum)
                    Dim s As String = wInfo.Text

                    ' Check For hyphenated words that break across a line.  
                    If (((wInfo.Attributes And WordAttributeFlags.HasSoftHyphen) = WordAttributeFlags.HasSoftHyphen) And
                        ((wInfo.Attributes And WordAttributeFlags.LastWordOnLine) = WordAttributeFlags.LastWordOnLine)) Then

                        ' Remove the hyphen and combine the two parts of the word beFore adding to the extracted text.
                        ' Note that we pass in the Unicode character For soft hyphen as well as the regular hyphen.
                        '
                        ' In untagged PDF, it's not uncommon to find a mixture of hard and soft hyphens that may
                        ' not be used For their intended purposes.
                        ' (Soft hyphens are intended only For words that break across lines.)
                        '
                        ' For the purposes of this sample, we'll remove all hyphens.  In practice, you may need to check 
                        ' words against a dictionary to determine If the hyphenated word is actually one word or two.
                        ' Note we remove ascii hyphen, Unicode soft hyphen(\u00ad) and Unicode hyphen(0x2010).
                        Dim splitstrs As String() = s.Split(New Char() {"-"c, Convert.ToChar(&HAD), Convert.ToChar(&H2010)})
                        textToExtract += splitstrs(0) + splitstrs(1)
                    Else
                        textToExtract += s
                    End If

                    ' Check For space adjacency and add a space If necessary.
                    If ((wInfo.Attributes And WordAttributeFlags.AdjacentToSpace) = WordAttributeFlags.AdjacentToSpace) Then
                        textToExtract += " "
                    End If

                    ' Check For a line break and add one If necessary
                    If ((wInfo.Attributes And WordAttributeFlags.LastWordOnLine) = WordAttributeFlags.LastWordOnLine) Then
                        textToExtract += "\n"
                    End If
                Next

                logfile.WriteLine("<page " + CStr(i + 1) + ">")
                logfile.WriteLine(textToExtract)

                ' Release requested WordList
                For wordnum As Integer = 0 To pageWords.Count - 1
                    pageWords(wordnum).Dispose()
                Next
            Next

            Console.WriteLine("Extracted " + CStr(nPages) + " pages.")
            logfile.Close()
        End Sub

        Sub ExtractTextTagged(doc As Document, wordFinder As WordFinder)
            Dim nPages As Integer = doc.NumPages
            Dim pageWords As IList(Of Word) = New List(Of Word)

            Dim logfile As System.IO.StreamWriter = New System.IO.StreamWriter("TextExtract-tagged-out.txt")
            Console.WriteLine("Writing TextExtract-tagged-out.txt")

            For i As Integer = 0 To nPages - 1
                pageWords = wordFinder.GetWordList(i)

                Dim textToExtract As String = ""

                For wordnum As Integer = 0 To pageWords.Count - 1
                    Dim wInfo As Word
                    wInfo = pageWords(wordnum)
                    Dim s As String = wInfo.Text

                    ' In most tagged PDFs, soft hyphens are used only to break words across lines, so we'll
                    ' check For any soft hyphens and remove them from our text output.
                    ' 
                    ' Note that we're not checking For the LastWordOnLine flag, unlike untagged PDF.  For Tagged PDF,
                    ' words are not flagged as being the last on the line If they are not at the end of a sentence.
                    If (((wInfo.Attributes And WordAttributeFlags.HasSoftHyphen) = WordAttributeFlags.HasSoftHyphen)) Then
                        ' Remove the hyphen and combine the two parts of the word beFore adding to the extracted text.
                        ' Note that we pass in the Unicode character For soft hyphen(\u00ad) and hyphen(0x2010).
                        Dim splitstrs As String() = s.Split(New Char() {"-"c, Convert.ToChar(&HAD), Convert.ToChar(&H2010)})
                        textToExtract += splitstrs(0) + splitstrs(1)
                    Else
                        textToExtract += s
                    End If

                    ' Check For space adjacency and add a space If necessary.
                    If ((wInfo.Attributes And WordAttributeFlags.AdjacentToSpace) = WordAttributeFlags.AdjacentToSpace) Then
                        textToExtract += " "
                    End If

                    ' Check For a line break and add one If necessary.
                    ' Normally this is accomplished Using WordAttributeFlags.LastWordOnLine,
                    ' but For tagged PDFs, the LastWordOnLine flag is set according to the
                    ' tags in the PDF, not according to visual line breaks in the document.
                    '
                    ' To preserve the visual line breaks in the document, we'll check whether
                    ' the word is the last word in the region.  If you instead prefer to
                    ' break lines according to the tags in the PDF, use
                    ' (wInfo.Attributes & WordAttributeFlags.LastWordOnLine) == WordAttributeFlags.LastWordOnLine, 
                    ' similar to the untagged case.
                    If (wInfo.IsLastWordInRegion) Then
                            textToExtract += "\n"
                        End If
                Next

                logfile.WriteLine("<page " + CStr(i + 1) + ">")
                logfile.WriteLine(textToExtract)

                ' Release requested WordList
                For wordnum As Integer = 0 To pageWords.Count - 1
                    pageWords(wordnum).Dispose()
                Next
            Next

            Console.WriteLine("Extracted " + CStr(nPages) + " pages.")
            logfile.Close()
        End Sub
    End Module
End Namespace
