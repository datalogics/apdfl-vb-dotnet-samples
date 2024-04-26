Imports System
Imports Datalogics.PDFL




''' This sample shows how to flatten transparencies in a PDF document.
'''
''' PDF files can have objects that are partially or fully transparent, and thus
''' can blend in various ways with objects behind them. Transparent graphics or images
''' can be stacked in a PDF file, with each one contributing to the final result that
''' appears on the page. The process to flatten a set of transparencies merges them
''' into a single image on the page.
'''
''' 
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.

Namespace FlattenTransparency
    Class FlattenTransparency
        Shared Sub Main(args As String())
            Console.WriteLine("FlattenTransparency sample:")

            Using library As New Library()
                Dim sInput1 As String = Library.ResourceDirectory & "Sample_Input/trans_1page.pdf"
                Dim sOutput1 As String = "FlattenTransparency-out1.pdf"
                Dim sInput2 As String = Library.ResourceDirectory & "Sample_Input/trans_multipage.pdf"
                Dim sOutput2 As String = "FlattenTransparency-out2.pdf"

                If args.Length > 0 Then
                    sInput1 = args(0)
                End If
                If args.Length > 1 Then
                    sInput2 = args(1)
                End If
                If args.Length > 2 Then
                    sOutput1 = args(2)
                End If
                If args.Length > 3 Then
                    sOutput2 = args(3)
                End If

                ' Open a document with a single page.
                Dim doc1 As New Document(sInput1)

                ' Verify that the page has transparency.  The parameter indicates
                ' whether to include the appearances of annotations or not when
                ' checking for transparency.
                Dim pg1 As Page = doc1.GetPage(0)
                Dim isTransparent As Boolean = pg1.HasTransparency(True)

                ' If there is transparency, flatten the document.
                If isTransparent Then
                    ' Flattening the document will check each page for transparency.
                    ' If a page has transparency, PDFL will create a new, flattened
                    ' version of the page and replace the original page with the
                    ' new one.  Because of this, make sure to dispose of outstanding Page objects
                    ' that refer to pages in the Document before calling flattenTransparency.
                    pg1.Dispose()

                    doc1.FlattenTransparency()
                    Console.WriteLine("Flattened single page document " & sInput1 & " as " & sOutput1 & ".")
                    doc1.Save(SaveFlags.Full, sOutput1)
                End If

                ' Open a document with multiple pages.
                Dim doc2 As New Document(sInput2)

                ' Iterate over the pages of the document and find the first page that has
                ' transparency.
                isTransparent = False
                Dim totalPages As Integer = doc2.NumPages
                Dim pageCounter As Integer = 0
                While Not isTransparent AndAlso pageCounter <= totalPages
                    Dim pg As Page = doc2.GetPage(pageCounter)
                    If pg.HasTransparency(True) Then
                        isTransparent = True
                        ' Explicitly delete the page here, to ensure the reference is gone before we 
                        ' attempt to flatten the document.  
                        pg.Dispose()
                        Exit While
                    End If

                    pageCounter += 1
                End While

                If isTransparent Then
                    ' Set up some parameters for the flattening.
                    Dim ftParams As New FlattenTransparencyParams()

                    ' The Quality setting indicates the percentage (0%-100%) of vector information
                    ' that is preserved.  Lower values result in higher rasterization of vectors.
                    ftParams.Quality = 50

                    ' Flatten transparency in the document, starting from the first page
                    ' that has transparency.
                    doc2.FlattenTransparency(ftParams, pageCounter, Document.LastPage)
                    Console.WriteLine("Flattened a multi-page document " & sInput2 & " as " & sOutput2 & ".")
                    doc2.Save(SaveFlags.Full, sOutput2)
                End If
            End Using
        End Sub
    End Class
End Namespace

