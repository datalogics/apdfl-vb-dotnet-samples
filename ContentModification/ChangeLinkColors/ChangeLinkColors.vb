Imports Datalogics.PDFL

'
' This sample demonstrates how to change the color of hyperlink text (usually blue).
'
' The program works by identifying text in a PDF file that Is associated with hyperlinks.
' Each link appears as a rectangle layer in the PDF file; ChangeLinkColors identifies these
' rectangles, And then finds the text that lines up within these rectangles And changes the
' color of each character that Is a part of the hyperlink.
'
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'

Namespace ChangeLinkColors
    Module ChangeLinkColors
        Sub Main(args As String())
            Console.WriteLine("ChangeLinkColors Sample:")

            Using (New Library())
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/sample_links.pdf"
                Dim sOutput As String = "ChangeLinkColors-out.pdf"

                If (args.Length > 0) Then
                    sInput = args(0)
                End If
                If (args.Length > 1) Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Input file: " + sInput + ", writing to " + sOutput)

                Dim doc As Document = New Document(sInput)

                Console.WriteLine("Opened a document.")

                Dim page As Page = doc.GetPage(0)

                Dim linkAnnots As List(Of LinkAnnotation) = New List(Of LinkAnnotation)

                ' First, make a list of all the link annotations on the page.
                For i As Integer = 0 To page.NumAnnotations - 1
                    Dim annot As Annotation = page.GetAnnotation(i)
                    If annot.GetType() Is GetType(LinkAnnotation) Then
                        linkAnnots.Add(DirectCast(annot, LinkAnnotation))
                    End If
                Next

                Dim content As Content = page.Content

                ' Iterate over the page's content and process the Text objects.
                FindAndProcessText(content, linkAnnots)

                page.UpdateContent()
                doc.Save(SaveFlags.Full, sOutput)

                doc.Close()
            End Using
        End Sub

        Sub FindAndProcessText(content As Content, linkAnnots As List(Of LinkAnnotation))
            ' This function recursively searches the given content for Text objects.
            For i As Integer = 0 To content.NumElements - 1
                Dim element As Element = content.GetElement(i)
                If element.GetType() Is GetType(Container) Then
                    Dim nestedContent As Content = DirectCast(element, Container).Content
                    FindAndProcessText(nestedContent, linkAnnots)
                ElseIf element.GetType() Is GetType(Form) Then
                    Dim nestedContent As Content = DirectCast(element, Form).Content
                    FindAndProcessText(nestedContent, linkAnnots)
                ElseIf element.GetType() Is GetType(Group) Then
                    Dim nestedContent As Content = DirectCast(element, Group).Content
                    FindAndProcessText(nestedContent, linkAnnots)
                ElseIf element.GetType() Is GetType(Text) Then
                    Console.WriteLine("Found a Text object.")
                    CheckCharactersInText(DirectCast(element, Text), linkAnnots)
                End If
            Next
        End Sub

        Sub CheckCharactersInText(txt As Text, linkAnnots As List(Of LinkAnnotation))
            ' This function checks to see if any characters in this Text object
            ' fall within the bounds of the LinkAnnotations on this page.
            For i As Integer = 0 To linkAnnots.Count - 1
                Dim charIndex As Integer

                ' Find the index of the first character in this Text object that intersects
                ' with the LinkAnnotation rectangle, if there is one.
                For charIndex = 0 To txt.NumberOfCharacters - 1
                    If txt.RectIntersectsCharacter(linkAnnots(1).Rect, charIndex) Then
                        Exit For
                    End If
                Next
                If charIndex >= txt.NumberOfCharacters Then
                    Continue For
                End If

                ' We may get some false positives using only Text.RectIntersectsCharacter().
                ' This function checks to see whether any part of the rectangle lies on the
                ' character's bounding box.  It's common for a character's bounding box
                ' to be taller than the height of the line; in these cases, the bounding
                ' box may intersect a LinkAnnotation rectangle on the line above.
                '
                ' We can weed out these false positives by checking the lower left corner
                ' of the character's text matrix.  If the lower left corner falls within
                ' the rectangle of the LinkAnnotation, then this character really
                ' Is contained within the LinkAnnotation rectangle.  Taking the floor And 
                ' ceiling as shown below accounts for precision errors in the rect And
                ' matrix values.
                '
                ' This heuristic works well for many cases.  If you find that you're still
                ' getting false positives Or false negatives, you may need to tweak the
                ' heuristic.
                Dim charMatrix As Matrix = txt.GetTextMatrixForCharacter(charIndex)
                Dim hWithinLinkBox As Boolean = (Math.Floor(linkAnnots(i).Rect.LLx) < Math.Ceiling(charMatrix.H)) And
                                                (Math.Floor(charMatrix.H) < Math.Ceiling(linkAnnots(i).Rect.URx))
                Dim vWithinLinkBox As Boolean = (Math.Floor(linkAnnots(i).Rect.LLy) < Math.Ceiling(charMatrix.V)) And
                                                (Math.Floor(charMatrix.V) < Math.Ceiling(linkAnnots(i).Rect.URy))

                If hWithinLinkBox And vWithinLinkBox Then
                    Console.WriteLine($"Found a character that falls within the bounds of LinkAnnotation {0}", i)

                    Dim startRunIndex As Integer

                    If charIndex = 0 Then
                        ' This is the first character, no splitting needed.
                        startRunIndex = 0
                    Else
                        txt.SplitTextRunAtCharacter(charIndex)
                        startRunIndex = txt.FindTextRunIndexForCharacter(charIndex)
                    End If

                    ' Now search for the last character that intersects with the LinkAnnotation rectangle.
                    While charIndex < txt.NumberOfCharacters
                        If Not txt.RectIntersectsCharacter(linkAnnots(i).Rect, charIndex) Then
                            Exit While
                        End If
                        charIndex += 1
                    End While

                    Dim endRunIndex As Integer

                    If charIndex >= txt.NumberOfCharacters Then
                        ' The LinkAnnotation rect goes past the bounding box of
                        ' this Text object, so the endRunIndex is the last TextRun
                        ' in this Text.
                        endRunIndex = txt.NumberOfRuns - 1
                    Else
                        txt.SplitTextRunAtCharacter(charIndex)
                        endRunIndex = txt.FindTextRunIndexForCharacter(charIndex)
                    End If

                    ' Change the GraphicState on all the TextRuns from
                    ' startRunIndex to endRunIndex.
                    For runIndex As Integer = startRunIndex To endRunIndex
                        Dim txtRun As TextRun = txt.GetRun(runIndex)
                        Dim gs As GraphicState = txtRun.GraphicState
                        gs.FillColor = New Color(0.0, 0.0, 1.0)
                        txtRun.GraphicState = gs
                    Next
                End If
            Next
        End Sub
    End Module
End Namespace
