Imports Datalogics.PDFL

'
' This sample demonstrates converting an XPS file into a PDF document.
' 
' XML Paper Specification (XPS) is a standard document format that Microsoft created in 2006
' as an alternative to the PDF format.
'
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'
Namespace CreateDocFromXPS
    Module CreateDocFromXPS
        Sub Main(args as String())
            Console.WriteLine("CreateDocFromXPS sample:")

            Using New Library()
                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/brownfox.xps"
                Dim sOutput As String = "CreateDocFromXPS-out.pdf"

                If (args.Length > 0) Then
                    sInput = args(0)
                End If
                If (args.Length > 1) Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Input file: " + sInput + ", writing to " + sOutput)

                ' First, create an XPSConvertParams to specify conversion parameters 
                ' for creating the document.
                Dim xpsparams as XPSConvertParams = New XPSConvertParams()
                
                ' PDFL requires a .joboptions file to specify settings for XPS conversion.
                ' A default .joboptions file is provided in the Resources directory of 
                ' the PDFL distribution.  This file is used by default, but a custom file
                ' can be used instead by setting the pathToSettingsFile property.
                Console.WriteLine("Using settings file located at: " + xpsparams.PathToSettingsFile)

                ' Create the document.
                Console.WriteLine("Creating a document from an XPS file...")
                Dim doc As Document = New Document(sInput, xpsparams)

                ' Save the document.
                Console.WriteLine("Saving the document...")
                doc.Save(SaveFlags.Full, sOutput)
            End Using
        End Sub
    End Module
End Namespace
