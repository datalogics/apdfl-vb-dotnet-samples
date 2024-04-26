Imports Datalogics.PDFL

''' This sample demonstrates working with masking in PDF documents. Masking an image allows you to remove or 
''' change a feature, while a soft mask allows you to place an image on a page and define the level of 
''' transparency for that image.
'''
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.

Namespace ImageSoftMask
    Class ImageSoftMask
        Shared Sub Main(args As String())
            Console.WriteLine("Image Soft Mask sample:")

            Using library As New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/ducky.jpg"
                Dim sMask As String = Library.ResourceDirectory & "Sample_Input/Mask.tif"
                Dim sOutput As String = "ImageSoftMask-out.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                If args.Length > 1 Then
                    sMask = args(1)
                End If

                If args.Length > 2 Then
                    sOutput = args(2)
                End If

                Console.WriteLine($"Input file: {sInput}, mask: {sMask}; will write to {sOutput}")

                Dim doc As New Document()
                Dim pageRect As New Rect(0, 0, 612, 792)
                Dim docpage As Page = doc.CreatePage(Document.BeforeFirstPage, pageRect)

                Dim baseImage As Datalogics.PDFL.Image
                baseImage = New Datalogics.PDFL.Image(sInput)
                Console.WriteLine("Created the image to mask.")

                Dim maskImage As Datalogics.PDFL.Image
                maskImage = New Datalogics.PDFL.Image(sMask)
                Console.WriteLine("Created the image to use as mask.")

                baseImage.SoftMask = maskImage
                Console.WriteLine("Set the SoftMask property.")

                docpage.Content.AddElement(baseImage)
                docpage.UpdateContent()
                Console.WriteLine("Got the content, added the element and updated the content.")

                doc.Save(SaveFlags.Full, sOutput)

                ' Dispose the doc object
                doc.Dispose()
                Console.WriteLine("Disposed document object.")
            End Using
        End Sub
    End Class
End Namespace
