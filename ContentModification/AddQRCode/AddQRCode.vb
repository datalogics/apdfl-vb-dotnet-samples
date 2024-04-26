Imports System
Imports Datalogics.PDFL

 ''' This sample shows how to add a QR barcode to a PDF page
 '''
 ''' Copyright (c) 2024, Datalogics, Inc. All rights reserved.
 
Namespace AddCollection
    Class AddQRCode
        Shared Sub Main(args As String())
            Console.WriteLine("AddQRCode Sample:")

            Using library As New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/sample_links.pdf"
                Dim sOutput As String = "../AddQRCode-out.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                Using doc As New Document(sInput)
                    Dim page As Page = doc.GetPage(0)

                    page.AddQRBarcode("Datalogics", 72.0, page.CropBox.Top - 1.5 * 72.0, 72.0, 72.0)

                    doc.Save(SaveFlags.Full, sOutput)
                End Using
            End Using
        End Sub
    End Class
End Namespace
