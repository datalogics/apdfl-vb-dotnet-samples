Imports System
Imports Datalogics.PDFL





''' The sample creates a PDF document with a single blank page, featuring a rectangle.
''' An action is added to the rectangle in the form of a hyperlink; if the reader clicks
''' on the rectangle, a different PDF file opens, showing an image.
''' 
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.

Namespace LaunchActions
    Class LaunchActions
        Shared Sub Main(args As String())

            Using library As New Library()
                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/ducky.pdf"
                Dim sOutput As String = "LaunchActions-out.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                If args.Length > 1 Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Input file: " & sInput & ". Writing to output " & sOutput)

                Dim doc As New Document()

                ' Standard letter size page (8.5" x 11")
                Dim pageRect As New Rect(0, 0, 612, 792)
                Dim docpage As Page = doc.CreatePage(Document.BeforeFirstPage, pageRect)
                Console.WriteLine("Created page.")

                Dim newLink As New LinkAnnotation(docpage, New Rect(153, 198, 306, 396))
                Console.WriteLine("Created new Link Annotation")

                ' LaunchActions need a FileSpecification object.
                '
                ' The FileSpecification specifies which file should be opened when the Link Annotation is "clicked".
                ' 
                ' Both FileSpecification objects and LaunchAction objects must be associated with a Document.
                ' The association happens at object creation time and cannot be changed.
                '
                ' FileSpecifications are associated with the Document that is passed in the constructor.
                ' LaunchActions are associated with the same Document as the FileSpecification used to create it.

                ' FileSpecifications can take either a relative or an absolute path. It is best to specify
                ' a relative path if the document is intended to work across multiple platforms (Windows, Linux, Mac)
                Dim fileSpec As New FileSpecification(doc, sInput)
                Console.WriteLine("Created a new FileSpecification with a path : " & fileSpec.Path)

                Dim launch As New LaunchAction(fileSpec)
                Console.WriteLine("Created a new Launch Action")

                ' setting NewWindow to true causes the document to open in a new window,
                ' by default this is set to false.
                launch.NewWindow = True

                newLink.Action = launch

                doc.Save(SaveFlags.Full, sOutput)
            End Using
        End Sub
    End Class
End Namespace
