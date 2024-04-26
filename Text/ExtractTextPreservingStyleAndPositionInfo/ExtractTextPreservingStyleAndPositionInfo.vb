Imports System.IO
Imports System.Text.Json
Imports Datalogics.PDFL
'
'
' This sample extracts text and details of that text in a PDF
' document, prints to console, and saves the text to a JSON file.
'
' Copyright (c) 2022-2024, Datalogics, Inc. All rights reserved.
'
'
Namespace ExtractTextPreservingStyleAndPositionInfo
    Module ExtractTextPreservingStyleAndPositionInfo
        ' Set Defaults
        Dim sInput As String = Library.ResourceDirectory + "Sample_Input/sample.pdf"
        Dim sOutput As String = "ExtractTextPreservingStyleAndPositionInfo-out.json"

        Sub Main(args As String())
            Console.WriteLine("ExtractTextPreservingStyleAndPositionInfo Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")

                Console.WriteLine("Input file: " + sInput)

                Using doc As Document = New Document(sInput)
                    Using docText As ExtractTextNameSpace.ExtractText = New ExtractTextNameSpace.ExtractText(doc)
                        Dim result As List(Of ExtractTextNameSpace.TextAndDetailsObject) = docText.GetTextAndDetails()
                        ' Save the output to a JSON file.
                        SaveJson(result)
                    End Using
                End Using
            End Using
        End Sub

        Sub SaveJson(result As List(Of ExtractTextNameSpace.TextAndDetailsObject))
            Using fs As FileStream = System.IO.File.Create(sOutput)
                Dim writerOptions As JsonWriterOptions = New JsonWriterOptions
                writerOptions.Indented = True
                Using writer As Utf8JsonWriter = New Utf8JsonWriter(fs, writerOptions)
                    writer.WriteStartArray()
                    For Each resultText As ExtractTextNameSpace.TextAndDetailsObject In result
                        writer.WriteStartObject()
                        writer.WriteString("text", resultText.Text)
                        writer.WriteStartArray("quads")
                        For Each quad As Quad In resultText.Quads
                            If quad Is Nothing Then
                                quad = New Quad()
                            End If
                            writer.WriteStartObject()
                            writer.WriteString("top-left", quad.TopLeft.ToString())
                            writer.WriteString("top-right", quad.TopRight.ToString())
                            writer.WriteString("bottom-left", quad.BottomLeft.ToString())
                            writer.WriteString("bottom-right", quad.BottomRight.ToString())
                            writer.WriteEndObject()
                        Next
                        writer.WriteEndArray()
                        writer.WriteStartArray("styles")
                        For Each st As ExtractTextNameSpace.DLStyleTransition In resultText.StyleList
                            If st Is Nothing Then
                                st = New ExtractTextNameSpace.DLStyleTransition()
                            End If
                            writer.WriteStartObject()
                            writer.WriteString("char-index", st.CharIndex.ToString())
                            Dim fontName As String = ""
                            If Not (st.Style Is Nothing) And Not (st.Style.FontName Is Nothing) Then
                                fontName = st.Style.FontName
                            End If
                            writer.WriteString("font-name", fontName)
                            Dim fontSize As Double = 0
                            If Not (st.Style Is Nothing) Then
                                fontSize = st.Style.FontSize
                            End If
                            writer.WriteString("font-size", Math.Round(fontSize, 2).ToString())
                            Dim colorSpaceName As String = ""
                            If Not (st.Style Is Nothing) And Not (st.Style.Color Is Nothing) And Not (st.Style.Color.Space Is Nothing) And Not (st.Style.Color.Space.Name Is Nothing) Then
                                colorSpaceName = st.Style.Color.Space.Name
                            End If
                            writer.WriteString("color-space", colorSpaceName)
                            writer.WriteStartArray("color-values")
                            If Not (st.Style Is Nothing) Then
                                Dim _color As ExtractTextNameSpace.DLColor = st.Style.Color
                                If Not (_color Is Nothing) Then
                                    For Each cv As Double In _color.Value
                                        writer.WriteStringValue(Math.Round(cv, 3).ToString())
                                    Next
                                End If
                            End If
                            writer.WriteEndArray()
                            writer.WriteEndObject()
                        Next
                        writer.WriteEndArray()
                        writer.WriteEndObject()
                    Next
                    writer.WriteEndArray()
                    writer.Flush()
                End Using
            End Using
        End Sub
    End Module
End Namespace
