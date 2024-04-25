Imports System
Imports Datalogics.PDFL



''' This sample demonstrates how to find and resample images within a PDF document. The images are
''' then put back into the PDF document with a new resolution.
''' 
''' Resampling involves resizing an image or images within a PDF document. Commonly this process is 
''' used to reduce the resolution of an image or series of images, to make them smaller. As a result
''' the process makes the PDF document smaller.
'''
''' Copyright (c) 2024, Datalogics, Inc. All rights reserved.



Namespace ImageResampling
    Friend Class ImageResampling
        Private Shared numreplaced As Integer
        Private Shared Sub ResampleImages(content As Content)
            Dim i As Integer = 0
            While i < content.NumElements
                Dim e As Element = content.GetElement(i)
                Console.WriteLine(i & " / " & content.NumElements & " = " & e.[GetType]().ToString())
                If TypeOf e Is Datalogics.PDFL.Image Then
                    Dim img As Datalogics.PDFL.Image = DirectCast(e, Datalogics.PDFL.Image)
                    Try
                        Dim newimg As Datalogics.PDFL.Image = img.ChangeResolution(400)
                        Console.WriteLine("Replacing an image...")
                        content.AddElement(newimg, i)
                        content.RemoveElement(i)
                        Console.WriteLine("Replaced.")
                        numreplaced += 1
                    Catch ex As ApplicationException
                        Console.WriteLine(ex.Message)
                    End Try
                ElseIf TypeOf e Is Container Then
                    Console.WriteLine("Recursing through a Container")
                    ResampleImages(DirectCast(e, Container).Content)
                ElseIf TypeOf e Is Group Then
                    Console.WriteLine("Recursing through a Group")
                    ResampleImages(DirectCast(e, Group).Content)
                ElseIf TypeOf e Is Form Then
                    Console.WriteLine("Recursing through a Form")
                    Dim formcontent As Content = DirectCast(e, Form).Content
                    ResampleImages(formcontent)
                    DirectCast(e, Form).Content = formcontent
                End If

                i += 1
            End While
        End Sub

        Shared Sub Main(args As String())
            Console.WriteLine("ImageResampling Sample:")
            ' ReSharper disable once UnusedVariable
            Using library As New Library()
                Console.WriteLine("Initialized the library.")
                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/ducky.pdf"
                Dim sOutput As String = "ImageResampling-out.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                If args.Length > 1 Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Input file " & sInput & ". Writing to output file " & sOutput)

                Dim doc As New Document(sInput)

                Console.WriteLine("Opened a document.")
                Try
                    For pgno As Integer = 0 To doc.NumPages - 1
                        numreplaced = 0
                        Dim pg As Page = doc.GetPage(pgno)
                        Dim content As Content = pg.Content
                        ResampleImages(content)
                        If numreplaced <> 0 Then
                            pg.UpdateContent()
                        End If
                    Next

                    doc.Save(SaveFlags.Full Or SaveFlags.CollectGarbage, sOutput)
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            End Using
        End Sub
    End Class
End Namespace
