Imports Datalogics.PDFL
Imports System




''' This sample creates a PDF document with a single page, featuring a rectangle.
''' An action Is added to the rectangle in the form of a hyperlink; if the viewer
''' clicks on the rectangle, it opens a Datalogics web page.
'''
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.

Namespace [Action]
    Class [Action]
        Shared Sub Main(args As String())
            Console.WriteLine("Action Sample:")

            Using library As New Library()
                Dim sOutput As String = "Action-out.pdf"
                Console.WriteLine("Initialized the library.")
                Dim doc As New Document()

                Using New Datalogics.PDFL.Path()
                    ' Create a PDF page which is the same size of the image.
                    Dim pageRect As New Rect(0, 0, 100, 100)
                    Dim docpage As Page = doc.CreatePage(Document.BeforeFirstPage, pageRect)
                    Console.WriteLine("Created page.")

                    ' Create our first link with a URI action
                    Dim newLink As New LinkAnnotation(docpage, New Rect(1.0, 2.0, 3.0, 4.0))
                    Console.WriteLine(newLink.ToString())

                    doc.BaseURI = "http://www.datalogics.com"
                    Dim uri As New URIAction("/adobe-pdf-library/", False)
                    Console.WriteLine("Action data: " & uri.URI)

                    newLink.Action = uri

                    ' Create a second link with a GoTo action
                    Dim secondLink As New LinkAnnotation(docpage, New Rect(5.0, 6.0, 7.0, 8.0))

                    Dim r As New Rect(5, 5, 100, 100)
                    Dim gta As New GoToAction(New ViewDestination(doc, 0, "FitR", r, 1.0))
                    Console.WriteLine("Action data: " & gta.ToString())

                    secondLink.Action = gta

                    ' Read some URI properties
                    Console.WriteLine("Extracted URI: " & uri.URI)

                    If uri.IsMap Then
                        Console.WriteLine("Send mouse coordinates")
                    Else
                        Console.WriteLine("Don't send mouse coordinates")
                    End If

                    ' Change the URI properties
                    doc.BaseURI = "http://www.datalogics.com"
                    uri.URI = "/products/pdf/pdflibrary/"

                    uri.IsMap = True

                    Console.WriteLine("Complete changed URI:" & doc.BaseURI & uri.URI)

                    If uri.IsMap Then
                        Console.WriteLine("Send mouse coordinates")
                    Else
                        Console.WriteLine("Don't send mouse coordinates")
                    End If

                    Console.WriteLine("Fit type of destination: " & gta.Destination.FitType)
                    Console.WriteLine("Rectangle of destination: " & gta.Destination.DestRect.ToString())
                    Console.WriteLine("Zoom of destination: " & gta.Destination.Zoom)
                    Console.WriteLine("Page number of destination: " & gta.Destination.PageNumber)

                    doc.Save(SaveFlags.Full, sOutput)
                End Using
            End Using
        End Sub
    End Class
End Namespace

