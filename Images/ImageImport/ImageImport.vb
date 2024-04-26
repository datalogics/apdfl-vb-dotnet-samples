Imports Datalogics.PDFL


''' This program demonstrates how to import images into a PDF file. The program runs without
''' prompting you, And creates two PDF files, demonstrating how to import graphics from image files
''' into a PDF file. One of the PDF output files Is the result of graphics imported from a multi-page TIF file.
'''
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.

Namespace ImageImport
    ' In this scenario the Image object is used alone to create a
    ' new PDF page with the image as the content.
    Class ImageImport
        Shared Sub Main(args As String())
            Console.WriteLine("Import Images Sample:")

            Using (New Library())
                Console.WriteLine("Initialized the library.")
                Dim doc As New Document()

                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/ducky.jpg"
                Dim sOutput As String = "ImageImport-out1.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                If args.Length > 1 Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Reading image file " & sInput & " and writing " & sOutput)

                Using newimage As New Image(sInput, doc)
                    ' Create a PDF page which is one inch larger all around than this image
                    ' The design width and height for the image are carried in the
                    ' Matrix.A and Matrix.D fields, respectively.
                    ' There are 72 PDF user space units in one inch.
                    Dim pageRect As New Rect(0, 0, newimage.Matrix.A + 144, newimage.Matrix.D + 144)
                    Dim docpage As Page = doc.CreatePage(Document.BeforeFirstPage, pageRect)
                    ' Center the image on the page
                    newimage.Translate(72, 72)
                    docpage.Content.AddElement(newimage)
                    docpage.UpdateContent()
                End Using

                doc.Save(SaveFlags.Full, sOutput)
            End Using
        End Sub
    End Class
End Namespace
