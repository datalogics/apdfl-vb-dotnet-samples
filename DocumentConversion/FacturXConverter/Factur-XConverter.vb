Imports System
Imports Datalogics.PDFL

 '
 '
 ' This sample demonstrates converting the input PDF with the input Invoice XML to a Factur-X compliant PDF.
 '
 ' For more detail see the description of the Factur-XConverter sample program on our Developer’s site, 
 ' http:'dev.datalogics.com/adobe-pdf-library/sample-program-descriptions/net-core-sample-programs/converting-and-merging-pdf-content/#facturxconverter
 '
 ' Copyright (c) 2022-2024, Datalogics, Inc. All rights reserved.
 '
 '
Namespace FacturXConverter

Module FacturXConverter
        ' The input XML must be named this way for it to be compliant. If you replace this with an absolute path to the file, then that would be a violation of this conformity.
        Dim sInputInoviceXML As String = "factur-x.xml"

        ' The type of Associated File Relationship specIfied, it's Alternative in Germany, while Data or Source in France
        Dim sRelationship As String = "Alternative"

        Sub Main(args As String())
            Console.WriteLine("Factur-XConverter Sample:")

            ' Initialize the Library
            Using New Library()
                Console.WriteLine("Initialized the library.")

                If (args.Length < 1) Then
                    Console.WriteLine("You must specIfy an input PDF, e.g.:")
                    Console.WriteLine()
                    Console.WriteLine("Factur-XConverter input-file.pdf")

                    return
                End If

                Dim sInputPDF As String = args(0)

                Dim sOutput As String = "Factur-XConverter-out.pdf"

                Console.WriteLine("Converting " + sInputPDF + " with " + sInputInoviceXML + ", output file is " + sOutput)

                ' Step 1) Open the input PDF
                Using doc = New Document(sInputPDF)
                    ' Step 2) Open the input Invoice XML and attach it to the PDF
                    Using New FileAttachment(doc, sInputInoviceXML)
                        ' Make a conversion parameters object
                        Dim pdfAParams As PDFAConvertParams = New PDFAConvertParams()
                        pdfAParams.IgnoreFontErrors = False
                        pdfAParams.NoValidationErrors = False
                        pdfAParams.ValidateImplementationLimitsOfDocument = True

                        ' Step 3) Convert the input PDF to be a PDF/A-3 document.
                        Dim pdfaResult As PDFAConvertResult = doc.CloneAsPDFADocument(PDFAConvertType.RGB3b, pdfAParams)

                        ' The conversion may have failed: we must check If the result has a valid Document
                        If (pdfaResult.PDFADocument Is Nothing) Then
                            Console.WriteLine("ERROR: Could not convert " + sInputPDF + " to PDF/A.")
                        Else
                            Console.WriteLine("Successfully converted " + sInputPDF + " to PDF/A.")
                        End If

                        Dim pdfaDoc As Document = pdfaResult.PDFADocument

                        Dim rootObj As PDFDict = pdfaDoc.Root
                        Dim associatedFilesArray As PDFArray = CType(rootObj.Get("AF"), PDFArray)
                        Dim associatedFile As PDFDict = CType(associatedFilesArray.Get(0), PDFDict)

                        associatedFile.Put("AFRelationship", New PDFName(sRelationship, pdfaDoc, False))

                        'Step 4) Add the required XMP metadata entries
                        AddMetadataAndExtensionSchema(pdfaDoc)

                        'Step 5) Save the document
                        pdfaDoc.Save(pdfaResult.PDFASaveFlags, sOutput)
                    End Using
                End Using
            End Using
        End Sub

        Sub AddMetadataAndExtensionSchema(document as Document)
            Dim NamespaceURI As String = "urn:factur-x:pdfa:CrossIndustryDocument:invoice:1p0#"
            Dim NamespacePrefix As String = "fx"

            Dim pathDocumentType As String = "DocumentType"
            Dim pathDocumentTypeValue As String = "INVOICE"

            Dim pathDocumentFileName As String = "DocumentFileName"
            Dim pathDocumentFileNameValue As String = sInputInoviceXML

            Dim pathConformanceLevel As String = "ConformanceLevel"
            Dim pathConformanceLevelValue As String = "BASIC"

            Dim pathVersion As String = "Version"
            Dim pathVersionValue As String = "1.0"

            'Set the XMP Factur-X properties
            document.SetXMPMetadataProperty(NamespaceURI, NamespacePrefix, pathDocumentType, pathDocumentTypeValue)
            document.SetXMPMetadataProperty(NamespaceURI, NamespacePrefix, pathDocumentFileName,
                pathDocumentFileNameValue)
            document.SetXMPMetadataProperty(NamespaceURI, NamespacePrefix, pathConformanceLevel,
                pathConformanceLevelValue)
            document.SetXMPMetadataProperty(NamespaceURI, NamespacePrefix, pathVersion, pathVersionValue)

            'Create the PDF/A Extension Schema for Factur-X since it's not part of the PDF/A standard.
            Dim extensionSchema as String
            extensionSchema = "<rdf:Description rdf:about=""""" + Environment.NewLine +
                                                                "xmlns:pdfaExtension=""http://www.aiim.org/pdfa/ns/extension/""" +
                                                                Environment.NewLine +
                                                                "xmlns:pdfaSchema=""http://www.aiim.org/pdfa/ns/schema#""" +
                                                                Environment.NewLine +
                                                                "xmlns:pdfaProperty=""http://www.aiim.org/pdfa/ns/property#"">" +
                                                                Environment.NewLine +
                                                                "<pdfaExtension:schemas>" + Environment.NewLine +
                                                                "<rdf:Bag>" + Environment.NewLine +
                                                                "<rdf:li rdf:parseType=""Resource"">" +
                                                                Environment.NewLine +
                                                                "<pdfaSchema:schema>Factur-X PDFA Extension Schema</pdfaSchema:schema>" +
                                                                Environment.NewLine +
                                                                "<pdfaSchema:NamespaceURI>urn:factur-x:pdfa:CrossIndustryDocument:invoice:1p0#</pdfaSchema:NamespaceURI>" +
                                                                Environment.NewLine +
                                                                "<pdfaSchema:prefix>fx</pdfaSchema:prefix>" +
                                                                Environment.NewLine +
                                                                "<pdfaSchema:property>" + Environment.NewLine +
                                                                "<rdf:Seq>" + Environment.NewLine +
                                                                "<rdf:li rdf:parseType=""Resource"">" +
                                                                Environment.NewLine +
                                                                "<pdfaProperty:name>DocumentFileName</pdfaProperty:name>" +
                                                                Environment.NewLine +
                                                                "<pdfaProperty:valueType>Text</pdfaProperty:valueType>" +
                                                                Environment.NewLine +
                                                                "<pdfaProperty:category>external</pdfaProperty:category>" +
                                                                Environment.NewLine +
                                                                "<pdfaProperty:description>name of the embedded XML invoice file</pdfaProperty:description>" +
                                                                Environment.NewLine +
                                                                "</rdf:li>" + Environment.NewLine +
                                                                 "<rdf:li rdf:parseType=""Resource"">" +
                                                                Environment.NewLine +
                                                                "<pdfaProperty:name>DocumentType</pdfaProperty:name>" +
                                                                Environment.NewLine +
                                                                "<pdfaProperty:valueType>Text</pdfaProperty:valueType>" +
                                                                Environment.NewLine +
                                                                "<pdfaProperty:category>external</pdfaProperty:category>" +
                                                                Environment.NewLine +
                                                                "<pdfaProperty:description>INVOICE</pdfaProperty:description>" +
                                                                Environment.NewLine +
                                                                "</rdf:li>" + Environment.NewLine +
                                                                "<rdf:li rdf:parseType=""Resource"">" +
                                                                Environment.NewLine +
                                                                "<pdfaProperty:name>Version</pdfaProperty:name>" +
                                                                Environment.NewLine +
                                                                "<pdfaProperty:valueType>Text</pdfaProperty:valueType>" +
                                                                Environment.NewLine +
                                                                "<pdfaProperty:category>external</pdfaProperty:category>" +
                                                                Environment.NewLine +
                                                                "<pdfaProperty:description>The actual version of the ZUGFeRD XML schema</pdfaProperty:description>" +
                                                                Environment.NewLine +
                                                                "</rdf:li>" + Environment.NewLine +
                                                                "<rdf:li rdf:parseType=""Resource"">" +
                                                                Environment.NewLine +
                                                                "<pdfaProperty:name>ConformanceLevel</pdfaProperty:name>" +
                                                                Environment.NewLine +
                                                                "<pdfaProperty:valueType>Text</pdfaProperty:valueType>" +
                                                                Environment.NewLine +
                                                                "<pdfaProperty:category>external</pdfaProperty:category>" +
                                                                Environment.NewLine +
                                                                "<pdfaProperty:description>The conformance level of the embedded ZUGFeRD data</pdfaProperty:description>" +
                                                                Environment.NewLine +
                                                                "</rdf:li>" + Environment.NewLine +
                                                                "</rdf:Seq>" + Environment.NewLine +
                                                                "</pdfaSchema:property>" + Environment.NewLine +
                                                                "</rdf:li>" + Environment.NewLine +
                                                                "</rdf:Bag>" + Environment.NewLine +
                                                                "</pdfaExtension:schemas>" + Environment.NewLine +
                                                                "</rdf:Description>" + Environment.NewLine +
                                                                "</rdf:RDF>" + Environment.NewLine

            Dim xmpMetadata As String = document.XMPMetadata

            'We're going to look for the ending of our typical XMP Metadata to replace it with our Extension Schema
            Dim newXMPMetadata As String = xmpMetadata.Replace("</rdf:RDF>", extensionSchema)

            'Update with our new XMP Metadata
            document.XMPMetadata = newXMPMetadata
        End Sub
    End Module
End Namespace
