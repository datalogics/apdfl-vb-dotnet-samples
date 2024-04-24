Imports System
Imports Datalogics.PDFL

Namespace LinkAnnotations
    Class LinkAnnotations
        Shared Sub Main(args As String())
            Console.WriteLine("LinkAnnotations Sample:")

            ' ReSharper disable once UnusedVariable
            Using (New Library())
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/sample.pdf"
                Dim sOutput As String = "LinkAnnotation-out.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                If args.Length > 1 Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Input file: " & sInput & ". Writing to output " & sOutput)

                Dim doc As New Document(sInput)

                Dim docpage As Page = doc.GetPage(0)

                Dim newLink As New Datalogics.PDFL.LinkAnnotation(docpage, New Rect(100, docpage.CropBox.Top - 25, 200, docpage.CropBox.Top - 50))

                ' Test some link features
                newLink.NormalAppearance = newLink.GenerateAppearance()

                Console.WriteLine("Current Link Annotation version = " & newLink.AnnotationFeatureLevel)
                newLink.AnnotationFeatureLevel = 1.0
                Console.WriteLine("New Link Annotation version = " & newLink.AnnotationFeatureLevel)

                ' Test the destination setting
                Dim dest As New ViewDestination(doc, 0, "XYZ", doc.GetPage(0).MediaBox, 1.5)

                dest.DestRect = New Rect(0.0, 0.0, 200.0, 200.0)
                Console.WriteLine("The new destination rectangle: " & dest.DestRect.ToString())

                dest.FitType = "FitV"
                Console.WriteLine("The new fit type: " & dest.FitType)

                dest.Zoom = 2.5
                Console.WriteLine("The new zoom level: " & dest.Zoom)

                dest.PageNumber = 1
                Console.WriteLine("The new page number: " & dest.PageNumber)

                newLink.Destination = dest

                newLink.Highlight = HighlightStyle.Invert

                If newLink.Highlight = HighlightStyle.Invert Then
                    Console.WriteLine("Invert highlighting.")
                End If

                doc.Save(SaveFlags.Full, sOutput)
            End Using
        End Sub
    End Class
End Namespace
