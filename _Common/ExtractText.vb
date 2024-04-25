Imports System.Text.Json.Serialization
Imports Datalogics.PDFL

'
' ===============================================================================
' This class is intended to assist with operations common to
' text extraction samples. The class contains methods to control types of words
' found and what information is returned to the user.
' ===============================================================================
'
' Copyright (c) 2022-2024, Datalogics, Inc. All rights reserved.
'
'

Namespace ExtractTextNameSpace
    ' This class represents the text info.
    Public Class TextObject
        Public Property Text As String
    End Class

    Public Class DLColorSpace
        Public Property Name As String
        Public Property NumComponents As Integer
    End Class

    Public Class DLColor
        Public Property Value As IList(Of Double)
        Public Property Space As DLColorSpace
    End Class

    Public Class DLStyle
        Public Property Color As DLColor
        Public Property FontSize As Double
        Public Property FontName As String
    End Class

    Public Class DLStyleTransition
        Public Property CharIndex As Integer
        Public Property Style As DLStyle
    End Class

    ' This class represents the text and details info.
    Public Class TextAndDetailsObject
        Public Property Text As String
        Public Property CharQuads As IList(Of Quad)
        Public Property Quads As IList(Of Quad)
        Public Property StyleList As IList(Of DLStyleTransition)
    End Class

    ' This class represents the AcroForm text info.
    Public Class AcroFormTextFieldObject
        <JsonPropertyName("field-name")>
        Public Property AcroFormFieldName As String
        <JsonPropertyName("field-text")>
        Public Property AcroFormFieldText As String
    End Class

    ' This class represents the Annotation text info.
    Public Class AnnotationTextObject
        <JsonPropertyName("annotation-type")>
        Public Property AnnotationType As String
        <JsonPropertyName("annotation-text")>
        Public Property AnnotationText As String
    End Class

    Public Class ExtractText
        Implements IDisposable
        Property doc As Document

        Property wordFinder As WordFinder
        Property pageWords As IList(Of Word) = New List(Of Word)

        Sub New(inputDoc As Document)
            Me.doc = inputDoc
            ' Use WordFinder with default settings to find words
            Dim wordConfig As WordFinderConfig = New WordFinderConfig()
            wordFinder = New WordFinder(doc, WordFinderVersion.Latest, wordConfig)
        End Sub

        Sub Dispose() Implements IDisposable.Dispose
            wordFinder.Dispose()
        End Sub

        '===============================================================================
        ' GetText() - Gets the text for the entire document.
        '===============================================================================
        Public Function GetText()
            Dim resultText As List(Of TextObject) = New List(Of TextObject)

            For pageNum As Integer = 0 To doc.NumPages - 1
                Dim pageText As List(Of TextObject) = GetText(pageNum)
                resultText.AddRange(pageText)
            Next
            Return resultText
        End Function

        '===============================================================================
        ' GetText() - Gets the text on a specified page.
        '===============================================================================
        Public Function GetText(pageNum As Integer)
            Dim pageText As List(Of TextObject) = New List(Of TextObject)

            pageWords = wordFinder.GetWordList(pageNum)

            For Each wordInfo As Word In pageWords
                Dim textObject As TextObject = New TextObject()
                textObject.Text = wordInfo.Text
                pageText.Add(textObject)
            Next
            ' Release requested WordList
            For wordnum As Integer = 0 To pageWords.Count - 1
                pageWords(wordnum).Dispose()
            Next

            Return pageText
        End Function

        '===============================================================================
        ' GetTextAndDetails() - Gets the text And detail info for the entire document.
        '===============================================================================
        Public Function GetTextAndDetails()
            Dim resultTextAndDetails As List(Of TextAndDetailsObject) = New List(Of TextAndDetailsObject)

            For pageNum As Integer = 0 To doc.NumPages - 1
                Dim pageText As List(Of TextAndDetailsObject) = GetTextAndDetails(pageNum)
                resultTextAndDetails.AddRange(pageText)
            Next
            Return resultTextAndDetails
        End Function

        '===============================================================================
        ' GetTextAndDetails() - Gets the text and detail info for a specific page.
        '===============================================================================
        Public Function GetTextAndDetails(pageNum As Integer)
            Dim resultTextAndDetails As List(Of TextAndDetailsObject) = New List(Of TextAndDetailsObject)

            pageWords = wordFinder.GetWordList(pageNum)

            For Each wordInfo As Word In pageWords
                Dim textObject As TextAndDetailsObject = New TextAndDetailsObject()
                textObject.Text = wordInfo.Text
                textObject.CharQuads = wordInfo.CharQuads
                textObject.Quads = wordInfo.Quads
                Dim trans As List(Of DLStyleTransition) = New List(Of DLStyleTransition)
                For Each st As StyleTransition In wordInfo.StyleTransitions
                    Dim dlStyleTrans As DLStyleTransition = New DLStyleTransition()
                    dlStyleTrans.CharIndex = st.CharIndex
                    Dim dlStyle As DLStyle = New DLStyle()
                    dlStyle.FontSize = st.Style.FontSize
                    dlStyle.FontName = st.Style.FontName
                    dlStyle.Color = New DLColor()
                    dlStyle.Color.Space = New DLColorSpace()
                    dlStyle.Color.Space.Name = st.Style.Color.Space.Name
                    dlStyle.Color.Space.NumComponents = st.Style.Color.Space.NumComponents
                    dlStyle.Color.Value = New List(Of Double)
                    For Each val As Double In st.Style.Color.Value
                        dlStyle.Color.Value.Add(val)
                    Next
                    dlStyleTrans.Style = dlStyle
                    trans.Add(dlStyleTrans)
                Next
                textObject.StyleList = trans
                resultTextAndDetails.Add(textObject)
            Next
            ' Release requested WordList
            For wordnum As Integer = 0 To pageWords.Count - 1
                pageWords(wordnum).Dispose()
            Next
            Return resultTextAndDetails
        End Function

        '===============================================================================
        ' GetAcroFormFieldData() - Gets the AcroForm field data.
        '===============================================================================
        Public Function GetAcroFormFieldData()
            Dim resultAcroFormText As List(Of AcroFormTextFieldObject) = New List(Of AcroFormTextFieldObject)

            Dim form_entry As PDFObject = doc.Root.Get("AcroForm")
            If form_entry.GetType() Is GetType(PDFDict) Then
                Dim form_root As PDFDict = DirectCast(form_entry, PDFDict)

                Dim fields_entry As PDFObject = form_root.Get("Fields")
                If fields_entry.GetType() Is GetType(PDFArray) Then
                    Dim fields As PDFArray = DirectCast(form_entry, PDFArray)
                    For fieldIndex As Integer = 0 To fields.Length - 1
                        Dim field_entry As PDFObject = fields.Get(fieldIndex)
                        EnumerateAcroFormField(field_entry, "", resultAcroFormText)
                    Next
                End If
            End If
            Return resultAcroFormText
        End Function

        Function GetAcroFormFieldText(field As PDFDict)
            Dim entry As PDFObject = field.Get("V")
            Dim svalue As String = ""
            If (entry.GetType() Is GetType(PDFString)) Then
                Dim value As PDFString = DirectCast(entry, PDFString)
                svalue = value.Value
            End If
            Return svalue
        End Function

        Sub EnumerateAcroFormField(field_entry As PDFObject, prefix As String, result As List(Of AcroFormTextFieldObject))
            Dim name_part As String
            Dim field_name As String
            Dim field_text As String
            Dim entry As PDFObject

            If field_entry.GetType() Is GetType(PDFDict) Then
                Dim field As PDFDict = DirectCast(field_entry, PDFDict)
                entry = DirectCast(field.Get("T"), PDFString)
                If entry.GetType() Is GetType(PDFString) Then
                    Dim t As PDFString = DirectCast(entry, PDFString)
                    name_part = t.Value
                Else
                    Return
                End If
                If (prefix = "") Then
                    field_name = name_part
                Else
                    ' Concatenate field name for 'Kids'
                    field_name = String.Format("{0}.{1}", prefix, name_part)
                End If

                ' Recursively handle 'Kids'
                entry = field.Get("Kids")
                If entry.GetType() Is GetType(PDFArray) Then
                    Dim kids As PDFArray = DirectCast(entry, PDFArray)
                    For kidIndex As Integer = 0 To kids.Length - 1
                        Dim kid_entry As PDFObject = kids.Get(kidIndex)
                        EnumerateAcroFormField(kid_entry, field_name, result)
                    Next
                End If

                ' We are at an end-node
                entry = field.Get("FT")
                If entry.GetType() Is GetType(PDFName) Then
                    Dim field_type_name As PDFName = DirectCast(entry, PDFName)
                    If field_type_name.Value = "Tx" Then
                        field_text = GetAcroFormFieldText(field)
                        Dim textObject As AcroFormTextFieldObject = New AcroFormTextFieldObject()
                        textObject.AcroFormFieldName = field_name
                        textObject.AcroFormFieldText = field_text
                        result.Add(textObject)
                    End If
                End If
            End If
        End Sub

        '===============================================================================
        ' GetAnnotationText() - Gets the Annotation text.
        '===============================================================================
        Public Function GetAnnotationText()
            Dim resultAnnotationText As List(Of AnnotationTextObject) = New List(Of AnnotationTextObject)

            For pageNum As Integer = 0 To pageNum < doc.NumPages - 1
                Dim page As Page = doc.GetPage(pageNum)
                For annotNum As Integer = 0 To page.NumAnnotations - 1
                    Dim annot As Annotation = page.GetAnnotation(annotNum)
                    If annot.Subtype = "Text" Or annot.Subtype = "FreeText" Then
                        Dim textObject As AnnotationTextObject = New AnnotationTextObject()
                        textObject.AnnotationType = annot.GetType().Name
                        textObject.AnnotationText = annot.Contents
                        resultAnnotationText.Add(textObject)
                    End If
                Next
            Next
            Return resultAnnotationText
        End Function
    End Class
End Namespace
