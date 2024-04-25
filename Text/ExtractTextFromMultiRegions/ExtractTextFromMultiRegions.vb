Imports System.IO
Imports Datalogics.PDFL

'
' This sample processes PDF files in a folder and extracts text from specific regions
' of its pages and saves the text to a CSV file.
' 
' Copyright (c) 2022-2024, Datalogics, Inc. All rights reserved.
'
'
Namespace ExtractTextFromMultiRegions
    Module ExtractTextFromMultiRegions
        ' Set Defaults
        Dim sInput As String = Library.ResourceDirectory + "Sample_Input/ExtractTextFromMultiRegions"
        Dim sOutput As String = "ExtractTextFromMultiRegions-out.csv"

        ' Rectangular regions to extract text in points (origin of the page is bottom left)
        ' (Left, Right, Bottom, Top)
        Dim invoice_number As Double() = {500, 590, 692, 710}
        Dim order_date As Double() = {500, 590, 680, 700}
        Dim order_number As Double() = {500, 590, 672, 688}
        Dim cust_id As Double() = {500, 590, 636, 654}
        Dim total As Double() = {500, 590, 52, 73}

        Dim regions As IList(Of Double()) = New List(Of Double()) From {invoice_number, order_date, order_number, cust_id, total}

        Enum quadSide
            left
            right
            bottom
            top
        End Enum

        Sub Main(args As String())
            Console.WriteLine("ExtractTextFromMultiRegions Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")
                Using outfile As System.IO.StreamWriter = New System.IO.StreamWriter(sOutput)
                    Console.WriteLine("Writing to: " + sOutput)
                    ' Print out a heading for CSV file
                    outfile.WriteLine("Filename,Invoice Number,Date,Order Number,Customer ID,Total")

                    Dim pdfFiles As String() = Directory.GetFiles(sInput)

                    For Each fileName As String in pdfFiles
                        Using doc As Document = New Document(fileName)
                            outfile.Write(System.IO.Path.GetFileName(fileName))
                            Console.WriteLine("Input file: " + System.IO.Path.GetFileName(fileName))

                            Using docText As ExtractTextNameSpace.ExtractText = New ExtractTextNameSpace.ExtractText(doc)
                                For pageNum As Integer = 0 To doc.NumPages - 1
                                    Dim result As List(Of ExtractTextNameSpace.TextAndDetailsObject) = docText.GetTextAndDetails(pageNum)
                                    For Each region As Double() In regions
                                        outfile.Write(",")
                                        For Each textInfo As ExtractTextNameSpace.TextAndDetailsObject In result
                                            Dim allQuadsWithinRegion As Boolean = True
                                            ' A Word typically has only 1 quad, but can have more than one
                                            ' for hyphenated words, words on a curve, etc.
                                            For Each quad As Quad In textInfo.Quads
                                                If quad Is Nothing Then
                                                    quad = New Quad()
                                                End If
                                                If (Not CheckWithinRegion(quad, region)) Then
                                                    allQuadsWithinRegion = False
                                                    Exit For
                                                End If
                                            Next
                                            If (allQuadsWithinRegion) Then
                                                ' Put this Word that meets our criteria in the output document
                                                outfile.Write(textInfo.Text)
                                            End If
                                        Next
                                    Next
                                    outfile.WriteLine()
                                Next
                            End Using
                        End Using
                    Next
                End Using
            End Using
        End Sub

        ' For this sample, we will consider a Word to be in the region of interest if the
        ' complete Word fits within the specified rectangular box
        Function CheckWithinRegion( wordQuad As Quad, region As Double()) As Boolean
            If (wordQuad.BottomLeft.H >= region(quadSide.left) And wordQuad.BottomRight.H <= region(quadSide.right) And
                wordQuad.TopLeft.H >= region(quadSide.left) And wordQuad.TopRight.H <= region(quadSide.right) And
                wordQuad.BottomLeft.V >= region(quadSide.bottom) And wordQuad.TopLeft.V <= region(quadSide.top) And
                wordQuad.BottomRight.V >= region(quadSide.bottom) And wordQuad.TopRight.V <= region(quadSide.top)) Then
                Return True
            End If
            Return false
        End Function
    End Module
End Namespace
