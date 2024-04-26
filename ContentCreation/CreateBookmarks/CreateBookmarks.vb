Imports System
Imports Datalogics.PDFL




''' This sample program shows how to automatically add bookmarks to a PDF file.
''' The program opens a source file called sample.PDF, adds bookmarks to it, and
''' saves an output file called Bookmark-out.PDF that shows the new bookmark structure.
''' 
''' You can add a bookmark to a PDF file that connects one part of the file to another.
''' For example, you could put bookmarks for each section heading, allowing a reader to
''' click on a hyperlink and move directly to that section from cross reference elsewhere in the document.
''' 
''' Open the Bookmark-out.PDF file in a PDF Viewer and click the Bookmark icon to display the bookmarks that
''' the CreateBookmarks program added to the output PDF document. Several of these bookmarks will zoom to
''' parts of the page. The last three, Child1, 2, and 3, are dummy bookmarks that do not respond when you
''' click on them.  They demonstrate how to rearrange existing bookmarks.
''' 
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.

Namespace CreateBookmarks
    Class CreateBookmarks
        ''' <summary>
        ''' This method allows to create GoToAction. The action uses
        ''' during bookmark creation process. The GoToAction creates from ViewDestination object.
        ''' </summary>
        ''' <param name="doc">Document for wich ViewDestination will be created</param>
        ''' <param name="rect">Destination rectangle for ViewDestination</param>
        ''' <param name="zoom">Zoom value for ViewDestination</param>
        ''' <returns>new Action object</returns>
        Private Shared Function CreateGoToAction(doc As Document, rect As Rect, zoom As Double) As Datalogics.PDFL.Action
            Dim viewDest As New ViewDestination(doc, 0, "XYZ", rect, zoom)
            Return New GoToAction(viewDest)
        End Function

        ''' <summary>
        ''' The  method allows to find specified bookmark
        ''' and then unlink the bookmark and returns it.
        ''' Unlinking bookmark is needed for bookmarks re-base.
        ''' </summary>
        ''' <param name="doc">The document in which bookmarks are located</param>
        ''' <param name="bookmarkName">bookmark's name</param>
        ''' <returns>Bookmark if it present in the document or null otherwise</returns>
        Private Shared Function PrepareDescendentBookmarkToRebase(doc As Document, bookmarkName As String) As Bookmark
            Dim bm As Bookmark = doc.BookmarkRoot.FindDescendentBookmark(bookmarkName)
            bm.Unlink()
            Return bm
        End Function

        ''' <summary>
        ''' The method allows add child to the other one.
        ''' Both bookmarks should exist in the document.
        ''' first step is to find parent bookmark and then add another(child) bookmark
        ''' </summary>
        ''' <param name="doc">document in wich bookmarks are located</param>
        ''' <param name="parentBookmarkName">the name of parent bookmark</param>
        ''' <param name="childBookmarkName">the name of child bookmark</param>
        Private Shared Sub AddBookmarkAsChild(doc As Document, parentBookmarkName As String, childBookmarkName As String)
            doc.BookmarkRoot.FindDescendentBookmark(parentBookmarkName).AddChild(PrepareDescendentBookmarkToRebase(doc, childBookmarkName))
        End Sub

        ''' <summary>
        ''' Method allows to add bookmarks to other one as subtree.
        ''' Firstly parent bookmark should be found and then we try to
        ''' add other bookmarks as subtree to the parent one.
        ''' </summary>
        ''' <param name="doc">Document in which bookmarks are located</param>
        ''' <param name="maxDepth">search depth</param>
        ''' <param name="parentBookmarkName">name of the parent bookmark</param>
        ''' <param name="childBookmarkName">name of the child bookmark</param>
        ''' <param name="subTreeTitle">name for new subtree</param>
        Private Shared Sub AddBookmarkAsSubtree(doc As Document, maxDepth As Integer, parentBookmarkName As String, childBookmarkName As String, subTreeTitle As String)
            doc.FindBookmark(parentBookmarkName, maxDepth).AddSubtree(PrepareDescendentBookmarkToRebase(doc, childBookmarkName), subTreeTitle)
        End Sub

        Shared Sub Main(args As String())
            Console.WriteLine("CreateBookmarks Sample:")

            ' ReSharper disable once UnusedVariable
            Using library As New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/sample.pdf"
                Dim sOutput As String = "Bookmark-out.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                If args.Length > 1 Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Input file: " & sInput & ", writing to " & sOutput)

                Using doc As New Document(sInput)
                    Dim rootBookmark As Bookmark = doc.BookmarkRoot

                    ' Create a few bookmarks that point to page "0" (developer page 0, user page 1)
                    ' with different ViewDestinations that have different Rects and zoom levels
                    Using page As Page = doc.GetPage(0)
                        ' Use CreateNewChild() to hang a new bookmark in tree off root bookmark
                        Dim bm0 As Bookmark = rootBookmark.CreateNewChild("(A) Root child, points to page 1, upper left corner, 300% zoom")
                        Dim rect As Rect = page.MediaBox
                        bm0.Action = CreateGoToAction(doc, rect, 3.0)

                        ' Use CreateNewChild() to hang a new bookmark in tree off newly created bookmark
                        Dim bm1 As Bookmark = bm0.CreateNewChild("(B) Root child's child, points to page 1, halfway down page, 75% zoom")
                        rect = New Rect(rect.Left, rect.Bottom, rect.Right, rect.Top / 2.0)
                        bm1.Action = CreateGoToAction(doc, rect, 0.75)

                        ' Use CreateNewSibling() to hang a new bookmark in tree next to existing bookmark
                        bm1 = bm0.CreateNewSibling("(C) Root child's sibling, points to page 1, 1/4 from top of page, 133% zoom")
                        rect = New Rect(rect.Left, rect.Bottom, rect.Right, rect.Top * 0.75)
                        bm1.Action = CreateGoToAction(doc, rect, 1.33)

                        AddBookmarkAsChild(doc, "(C) Root child's sibling, points to page 1, 1/4 from top of page, 133% zoom", "(B) Root child's child, points to page 1, halfway down page, 75% zoom")

                        AddBookmarkAsSubtree(doc, 12, "(A) Root child, points to page 1, upper left corner, 300% zoom", "(C) Root child's sibling, points to page 1, 1/4 from top of page, 133% zoom", "Bookmark formerly known as '(C) ... '")

                        ' Create three bookmarks as new children to the root
                        bm0 = rootBookmark.CreateNewChild("Child 2")
                        bm0.AddNextSibling(rootBookmark.CreateNewChild("Child 1"))
                        bm0.AddPreviousSibling(rootBookmark.CreateNewChild("Child 3"))

                        doc.Save(SaveFlags.Full, sOutput)
                    End Using
                End Using
            End Using
        End Sub
    End Class
End Namespace
