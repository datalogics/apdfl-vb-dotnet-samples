Imports System
Imports System.Collections.Generic
Imports Datalogics.PDFL


''' This sample demonstrates working with page labels in a PDF document. Each PDF file has a 
''' data structure that governs how page numbers appear, such as the font and type of numeral.
''' 
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.

Namespace PageLabels
    Class PageLabels
        Shared Sub Main(args As String())
            Console.WriteLine("Page Labels Sample:")

            Using library As New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/pagelabels.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                Console.WriteLine("Input file " & sInput)

                Dim doc As New Document(sInput)

                ' Extract a page label from the document
                Dim labelString As String = doc.FindLabelForPageNum(doc.NumPages - 1)
                Console.WriteLine("Last page in the document is labeled " & labelString)

                ' Index will equal (doc.NumPages - 1)
                Dim index As Integer = doc.FindPageNumForLabel(labelString)
                Console.WriteLine(labelString & " has an index of " & index & " in the document.")

                ' Add a new page to the document
                ' It will start on page 5 and have numbers of the style A-i, A-ii, etc.
                Dim pl As New PageLabel(5, NumberStyle.RomanLowercase, "A-", 1)

                Dim labels As IList(Of PageLabel) = doc.PageLabels
                labels.Add(pl)
                doc.PageLabels = labels

                Console.WriteLine("Added page range starting on page 5.")

                ' Change the properties of the third page range
                labels = doc.PageLabels ' Get a freshly sorted list
                labels(2).Prefix = "Section 3-"
                labels(2).FirstNumberInRange = 2
                doc.PageLabels = labels

                Console.WriteLine("Changed the prefix for the third range.")

                ' Now walk the list of page labels
                For Each label As PageLabel In doc.PageLabels
                    Console.WriteLine("Label range starts on page " & label.StartPageIndex &
                                      ", ends on page " & label.EndPageIndex)
                    Console.WriteLine("The prefix is '" & label.Prefix &
                                      "' and begins with number " & label.FirstNumberInRange)
                    Console.WriteLine()
                Next
            End Using
        End Sub
    End Class
End Namespace
