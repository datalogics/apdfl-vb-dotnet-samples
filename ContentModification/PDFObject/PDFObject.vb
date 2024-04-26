Imports System
Imports Datalogics.PDFL


''' This sample demonstrates working with data objects in a PDF document. It examines the Objects and displays
''' information about them.  The sample extracts the dictionary for an object called URIAction and updates it using PDFObjects.
''' 
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'''
''' 
''' 
''' Suggested input file: Library.ResourceDirectory + "Sample_Input/sample_links.pdf"
''' Input file properties: First page must have an annotation with a URI link

Namespace PDFObject
    Class PDFObjectDemo
        Shared Sub Main(args As String())
            Console.WriteLine("PDFObject Sample:")

            Using library As New Library()
                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/sample_links.pdf"
                Dim sOutput As String = "PDFObject-out.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                If args.Length > 1 Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Input file: " & sInput & ". Writing to output " & sOutput)

                Dim doc As New Document(sInput)
                Dim page As Page = doc.GetPage(0)

                Dim annot As LinkAnnotation = CType(page.GetAnnotation(1), LinkAnnotation)
                Dim uri As URIAction = CType(annot.Action, URIAction)

                ' Print some info about the URI action, before we modify it
                Console.WriteLine("Initial URL: " & uri.URI)
                Console.WriteLine("Is Map property: " & uri.IsMap)

                ' Modify the URIAction
                '
                ' A URI action is a dictionary containing:
                '    Key: S     Contents: a name object with the value "URI" (required)
                '    Key: URI   Contents: a string object for the uniform resource locator (required)
                '    Key: IsMap Contents: a boolean for whether the link is part of a map (optional)
                '    (see section 8.5.3, "Action Types", of the PDF Reference)
                '
                ' We will change the URI entry and delete the IsMap entry for this dictionary

                Dim uri_dict As PDFDict = uri.PDFDict ' Extract the dictionary

                ' Create a new string object
                Dim uri_string As New PDFString("http://www.google.com", doc, False, False)

                uri_dict.Put("URI", uri_string) ' Change the URI (replaces the old one)
                uri_dict.Remove("IsMap") ' Remove the IsMap entry

                ' Check that we deleted the IsMap entry
                Console.WriteLine("Does this dictionary have an IsMap entry? " & uri_dict.Contains("IsMap"))

                doc.Save(SaveFlags.Full, sOutput)
                doc.Close()

                ' Check the modified contents of the link
                doc = New Document(sOutput)
                page = doc.GetPage(0)
                annot = CType(page.GetAnnotation(1), LinkAnnotation)
                uri = CType(annot.Action, URIAction)

                Console.WriteLine("Modified URL: " & uri.URI)
                Console.WriteLine("Is Map property (if not present, defaults to false): " & uri.IsMap)
            End Using
        End Sub
    End Class
End Namespace
