Imports System
Imports Datalogics.PDFL



''' This sample demonstrates merging one PDF document into another. The program
''' prompts the user to enter the names of two PDF files, and then inserts the content 
''' of the second PDF file into the first PDF file and saves the result in a third PDF file.
'''
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.

Namespace MergePDF
    Class MergePDF
        Shared Sub Main(args As String())
            Console.WriteLine("MergePDF Sample:")

            ' ReSharper disable once UnusedVariable
            Using library As New Library()
                Dim sInput1 As String = Library.ResourceDirectory & "Sample_Input/merge_pdf1.pdf"
                Dim sInput2 As String = Library.ResourceDirectory & "Sample_Input/merge_pdf2.pdf"
                Dim sOutput As String = "MergePDF-out.pdf"

                If args.Length > 0 Then
                    sInput1 = args(0)
                End If

                If args.Length > 1 Then
                    sInput2 = args(1)
                End If

                If args.Length > 2 Then
                    sOutput = args(2)
                End If

                Console.WriteLine("MergePDF: adding " & sInput1 & " and " & sInput2 & " and writing to " & sOutput)

                Using doc1 As New Document(sInput1)
                    Using doc2 As New Document(sInput2)
                        Try
                            doc1.InsertPages(Document.LastPage, doc2, 0, Document.AllPages, PageInsertFlags.Bookmarks Or
                                             PageInsertFlags.Threads Or
                                             PageInsertFlags.DoNotMergeFonts Or
                                             PageInsertFlags.DoNotResolveInvalidStructureParentReferences Or
                                             PageInsertFlags.DoNotRemovePageInheritance)
                        Catch ex As LibraryException
                            If Not ex.Message.Contains("An incorrect structure tree was found in the PDF file but operation continued") Then
                                Throw
                            End If

                            Console.Out.WriteLine(ex.Message)
                        End Try

                        doc1.Save(SaveFlags.Full Or
                                  SaveFlags.SaveLinearizedNoOptimizeFonts Or
                                  SaveFlags.Compressed, sOutput)
                    End Using
                End Using
            End Using
        End Sub
    End Class
End Namespace
