Imports System
Imports Datalogics.PDFL

Namespace Annotations
    Class Annotations
        Shared Sub Main(args As String())
            Console.WriteLine("Annotations Sample:")

            ' ReSharper disable once UnusedVariable
            Using (New Library())
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/sample_annotations.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                Console.WriteLine("Input file: " & sInput)

                Dim doc As New Document(sInput)

                Dim pg As Page = doc.GetPage(0)
                Dim ann As Annotation = pg.GetAnnotation(0)

                Console.WriteLine(ann.Title)
                Console.WriteLine(ann.GetType().Name)
            End Using
        End Sub
    End Class
End Namespace
