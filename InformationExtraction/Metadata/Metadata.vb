Imports System.Text
Imports System.Xml
Imports Datalogics.PDFL

'
'
' This sample shows how to view And edit metadata for a PDF document. The metadata values appear on the Properties
' window in a PDF Viewer.
'
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'

Namespace Metadata
    Module Metadata
        Sub Main(args As String())
            Using New Library()
                Dim sInput1 As String = Library.ResourceDirectory + "Sample_Input/sample.pdf"
                Dim sInput2 As String = Library.ResourceDirectory + "Sample_Input/Ducky_with_metadata.pdf"
                Dim sOutput As String = "sample-metadata-out.pdf"

                If args.Length > 0 Then
                    sInput1 = args(0)
                End If

                If (args.Length > 1) Then
                    sInput2 = args(1)
                End If

                If (args.Length > 2) Then
                    sOutput = args(2)
                End If

                Console.WriteLine("Input files " + sInput1 + " and  " + sInput2 + ". Writing to output file " +
                                  sOutput)

                DisplayDocumentMetadata(sInput1, sOutput)
                DisplayImageMetadata(sInput2)
            End Using
        End Sub

        Private Sub DisplayDocumentMetadata(input As String, output As String)
            Using doc As New Document(input)
                ' Parse some data out of the document metadata string
                Dim metadata As String = doc.XMPMetadata
                Console.WriteLine("Title: {0}", GetTitle(metadata))
                Console.WriteLine("CreatorTool: {0}",
                    doc.GetXMPMetadataProperty("http://ns.adobe.com/xap/1.0/", "CreatorTool"))
                Console.WriteLine("format: {0}",
                    doc.GetXMPMetadataProperty("http://purl.org/dc/elements/1.1/", "format"))
                Dim numAuthors As Integer = doc.CountXMPMetadataArrayItems("http://ns.adobe.com/xap/1.0/", "Authors")
                Console.WriteLine("Number of authors: {0}", numAuthors)
                For i As Integer = 1 To numAuthors
                    Console.WriteLine("Author: {0}",
                        doc.GetXMPMetadataArrayItem("http://ns.adobe.com/xap/1.0/", "Authors", i))
                Next

                ' Demonstrate setting a property
                doc.SetXMPMetadataArrayItem("http://ns.adobe.com/xap/1.0/", "tetractys", "Authors", 2,
                    "Metadata Tester")
                doc.Save(SaveFlags.Full, output)
            End Using
        End Sub

        Private Sub DisplayImageMetadata(input As String)
            Using doc As Document = New Document(input)
                ' Demonstrate getting data from an image
                Dim content As Content = doc.GetPage(0).Content
                Dim container As Container = DirectCast(content.GetElement(0), Container)
                Dim image As Datalogics.PDFL.Image = DirectCast(container.Content.GetElement(0), Datalogics.PDFL.Image)
                Dim metadata As String = image.Stream.Dict.XMPMetadata
                Console.WriteLine("Ducky CreatorTool: {0}\n", GetCreatorToolAttribute(metadata))
            End Using
        End Sub

        Function GetTitle(xmlstring As String)
            Dim title As String = ""

            Dim xmldoc As XmlDocument = New XmlDocument()
            xmldoc.LoadXml(xmlstring)
            Dim element As XmlElement = DirectCast(xmldoc.GetElementsByTagName("dc:title")(0), XmlElement)
            If element IsNot Nothing Then
                Dim titleNode As XmlNode = element.GetElementsByTagName("rdf:li")(0)
                If titleNode IsNot Nothing Then
                    title = GetText(titleNode.ChildNodes)
                End If
            End If

            Return title
        End Function

        Function GetCreatorTool(xmlstring As String)
            Dim creatorTool As String = ""
            Dim xmldoc As XmlDocument = New XmlDocument()
            xmldoc.LoadXml(xmlstring)
            Dim element As XmlElement = DirectCast(xmldoc.GetElementsByTagName("xap:CreatorTool")(0), XmlElement)
            If element IsNot Nothing Then
                creatorTool = GetText(element.ChildNodes)
            End If

            Return creatorTool
        End Function

        Function GetCreatorToolAttribute(xmlstring As String)
            Dim creatorToolAttribute As String = ""

            Dim xmldoc As XmlDocument = New XmlDocument()
            xmldoc.LoadXml(xmlstring)
            For Each node As XmlNode In xmldoc.GetElementsByTagName("rdf:Description")
                Dim e As XmlElement = DirectCast(node, XmlElement)
                If e.HasAttribute("xap:CreatorTool") Then
                    creatorToolAttribute = e.GetAttribute("xap:CreatorTool")
                End If
            Next

            Return creatorToolAttribute
        End Function

        Function GetText(nodeList As XmlNodeList)
            Dim sb As StringBuilder = New StringBuilder()

            For Each node As XmlNode In nodeList
                If node.NodeType = XmlNodeType.Text Then
                    sb.Append(node.Value)
                End If
            Next

            Return sb.ToString()
        End Function
    End Module
End Namespace
