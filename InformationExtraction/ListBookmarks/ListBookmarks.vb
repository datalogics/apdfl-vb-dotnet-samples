Imports System
Imports Datalogics.PDFL

'
' 
' This sample finds And describes the bookmarks included in a PDF document.
' 
' 
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'

Namespace ListBookmarks
    Module ListBookmarks
        Sub EnumerateBookmarks(b As Bookmark)
            If b IsNot Nothing Then
                Console.Write(b)
                Console.Write(": ")
                Console.Write(b.Title)
                Dim v As ViewDestination = b.ViewDestination
                If v IsNot Nothing Then
                    Console.Write(", page ")
                    Console.Write(v.PageNumber)
                    Console.Write(", fit ")
                    Console.Write(v.FitType)
                    Console.Write(", dest rect ")
                    Console.Write(v.DestRect)
                    Console.Write(", zoom ")
                    Console.Write(v.Zoom)
                End If
                Console.WriteLine()
                EnumerateBookmarks(b.FirstChild)
                EnumerateBookmarks(b.Next)
            End If
        End Sub
        Sub Main(args As String())
            Console.WriteLine("ListBookmarks Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")
                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/sample.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                Console.WriteLine("Input file: " + sInput)

                Dim doc As Document = New Document(sInput)

                Dim rootBookmark As Bookmark = doc.BookmarkRoot
                EnumerateBookmarks(rootBookmark)
            End Using
        End Sub
    End Module
End Namespace
