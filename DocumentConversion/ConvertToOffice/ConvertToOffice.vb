Imports System
Imports Datalogics.PDFL

 '
 '
 ' ConvertToOffice converts sample PDF documents to Office Documents.
 '
 ' Copyright (c) 2023 - 2024, Datalogics, Inc. All rights reserved.
 '
 '
Namespace ConvertToOffice
    Module ConvertToOffice

        Enum OfficeType
            Word = 0
            Excel = 1
            PowerPoint = 2
        End Enum

        Sub Main(args As String())
            Console.WriteLine("ConvertToOffice Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")

                Dim inputPathWord As String = Library.ResourceDirectory + "Sample_Input/Word.pdf"
                Dim outputPathWord As String = "word-out.docx"
                Dim inputPathExcel As String = Library.ResourceDirectory + "Sample_Input/Excel.pdf"
                Dim outputPathExcel As String = "excel-out.xlsx"
                Dim inputPathPowerPoint As String = Library.ResourceDirectory + "Sample_Input/PowerPoint.pdf"
                Dim outputPathPowerPoint As String = "powerpoint-out.pptx"

                ConvertPDFToOffice(inputPathWord, outputPathWord, OfficeType.Word)
                ConvertPDFToOffice(inputPathExcel, outputPathExcel, OfficeType.Excel)
                ConvertPDFToOffice(inputPathPowerPoint, outputPathPowerPoint, OfficeType.PowerPoint)
            End Using
        End Sub

        Sub ConvertPDFToOffice(inputPath As String, outputPath As String, officeType As OfficeType)
            Console.WriteLine("Converting " + inputPath + ", output file is " + outputPath)

            Dim result As Boolean = False

            If (officeType = OfficeType.Word) Then
                result = Document.ConvertToWord(inputPath, outputPath)
            ElseIf (officeType = OfficeType.Excel) Then
                result = Document.ConvertToExcel(inputPath, outputPath)
            ElseIf (officeType = OfficeType.PowerPoint) Then
                result = Document.ConvertToPowerPoint(inputPath, outputPath)
            End If

            If (result) Then
                Console.WriteLine("Successfully converted " + inputPath + " to " + outputPath)
            Else
                Console.WriteLine("ERROR: Could not convert " + inputPath)
            End If
        End Sub
    End Module
End Namespace
