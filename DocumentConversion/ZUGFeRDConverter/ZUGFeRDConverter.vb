Imports System
Imports Datalogics.PDFL

 '
 '
 ' This sample demonstrates converting the input PDF with the input Invoice ZUGFeRD XML to a ZUGFeRD compliant PDF.
 '
 ' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
 '
 '
Namespace ZUGFeRDConverter
    Module ZUGFeRDConverter
        Sub Main(args As String())
            Console.WriteLine("ZUGFeRDConverter Sample:")

            'Initialize the Library

            Using New Library()
                Console.WriteLine("Initialized the library.")

                If (args.Length < 2) Then
                    Console.WriteLine("You must specify an input PDF and an input ZUGFeRD Invoice XML file, e.g.:")
                    Console.WriteLine()
                    Console.WriteLine("ConvertToZUGFeRD input-file.pdf input-ZUGFeRD-invoice.xml")

                    return
                End If

                Dim sInputPDF As String = args(0)
                Dim sInputInoviceXML As String = args(1)
                Dim sOutput As String = "ZUGFeRDConverter-out.pdf"

                Console.WriteLine("Converting " + sInputPDF + " with " + sInputInoviceXML + ", output file is " + sOutput)

                ' Step 1) Open the input PDF
                Using doc As Document = New Document(sInputPDF)
                    ' Step 2) Open the input Invoice XML and attach it to the PDF
                    Using (New FileAttachment(doc, sInputInoviceXML))
                        ' Make a conversion parameters object
                        Dim pdfaParams As PDFAConvertParams = New PDFAConvertParams()
                        pdfaParams.IgnoreFontErrors = false
                        pdfaParams.NoValidationErrors = false
                        pdfaParams.ValidateImplementationLimitsOfDocument = true

                        ' Step 3) Convert the input PDF to be a PDF/A-3 document.
                        Dim pdfaResult As PDFAConvertResult = doc.CloneAsPDFADocument(PDFAConvertType.RGB3b, pdfaParams)

                        ' The conversion may have failed: we must check if the result has a valid Document
                        If (pdfaResult.PDFADocument Is Nothing)
                            Console.WriteLine("ERROR: Could not convert " + sInputPDF + " to PDF/A.")
                        Else
                            Console.WriteLine("Successfully converted " + sInputPDF + " to PDF/A.")

                            Dim pdfaDoc As Document = pdfaResult.PDFADocument

                            'Step 4) Add the required XMP metadata entries
                            AddMetadataAndExtensionSchema(pdfaDoc, sInputInoviceXML)

                            'Step 5) Save the document
                            pdfaDoc.Save(pdfaResult.PDFASaveFlags, sOutput)
                        End If
                    End Using
                End Using
            End Using
        End Sub

        Sub AddMetadataAndExtensionSchema(document As Document, sInputInoviceXML As String)
            Dim namespaceURI As String = "urn:ferd:pdfa:CrossIndustryDocument:invoice:2p0#"
            Dim namespacePrefix As String = "zf"

            Dim pathDocumentType As String = "DocumentType"
            Dim pathDocumentTypeValue As String = "INVOICE"

            Dim pathDocumentFileName As String = "DocumentFileName"
            Dim pathDocumentFileNameValue As String = sInputInoviceXML

            Dim pathConformanceLevel As String = "ConformanceLevel"
            Dim pathConformanceLevelValue As String = "BASIC"

            Dim pathVersion As String = "Version"
            Dim pathVersionValue As String = "2p0"

            'Set the XMP ZUGFeRD properties
            document.SetXMPMetadataProperty(namespaceURI, namespacePrefix, pathDocumentType, pathDocumentTypeValue)
            document.SetXMPMetadataProperty(namespaceURI, namespacePrefix, pathDocumentFileName,
                pathDocumentFileNameValue)
            document.SetXMPMetadataProperty(namespaceURI, namespacePrefix, pathConformanceLevel,
                pathConformanceLevelValue)
            document.SetXMPMetadataProperty(namespaceURI, namespacePrefix, pathVersion, pathVersionValue)

            'Create the PDF/A Extension Schema for ZUGFeRD since it's not part of the PDF/A standard.
            Dim extensionSchema As String
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
                                                                "<pdfaSchema:schema>ZUGFeRD PDFA Extension Schema</pdfaSchema:schema>" +
                                                                Environment.NewLine +
                                                                "<pdfaSchema:namespaceURI>urn:ferd:pdfa:CrossIndustryDocument:invoice:2p0#</pdfaSchema:namespaceURI>" +
                                                                Environment.NewLine +
                                                                "<pdfaSchema:prefix>zf</pdfaSchema:prefix>" +
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
            Dim NewXMPMetadata As String = xmpMetadata.Replace("</rdf:RDF>", extensionSchema)

            'Update with our New XMP Metadata
            document.XMPMetadata = NewXMPMetadata
        End Sub
    End Module
End Namespace
