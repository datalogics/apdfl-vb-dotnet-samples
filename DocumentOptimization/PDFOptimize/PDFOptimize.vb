Imports Datalogics.PDFL

'
'
' This sample program demonstrates the use of PDFOptimizer. This compresses a PDF document
' to make it smaller so it's easier to process and download.
'
' NOTE: Some documents can't be compressed because they're already well-compressed or contain
' content that can't be assumed is safe to be removed.  However you can fine tune the optimization
' to suit your applications needs and drop such content to achieve better compression if you already
' know it's unnecessary.
'
'
' Copyright (c) 2024, Datalogics, Inc. All rights reserved.
'
'
Namespace PDFOptimize
    Module Optimize
        Sub Main(args As String())
            System.Console.WriteLine("PDF Optimizer:")

            Using (New Library())
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/sample.pdf"
                Dim sOutput As String = "PDFOptimizer-out.pdf"

                If (args.Length > 0) Then
                    sInput = args(0)
                End If

                If (args.Length > 1) Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Input file: " + sInput)
                Console.WriteLine("Writing to output " + sOutput)

                Using doc As New Document(sInput)
                    Using optimizer As New Datalogics.PDFL.PDFOptimizer()
                        Dim beforeLength As Long = New System.IO.FileInfo(sInput).Length

                        optimizer.Optimize(doc, sOutput)

                        Dim afterLength As Long = New System.IO.FileInfo(sOutput).Length

                        Console.WriteLine()
                        Console.WriteLine("Optimized file: {0:P2} the size of the original.", CDbl(afterLength / beforeLength))
                    End Using
                End Using
            End Using
        End Sub
    End Module
End Namespace
