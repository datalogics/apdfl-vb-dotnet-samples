Imports Datalogics.PDFL

'
' 
' This sample shows how to list metadata about a PDF document, And presents command prompts
' if you want to change these values, such as the title Or author. The results are exported
' to a PDF output document.
' 
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'

Namespace ListInfo
    Module ListInfo
        Sub Main(args As String())
            Console.WriteLine("ListInfo Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/sample.pdf"
                Dim sOutput As String = "ListInfo-out.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If
                If args.Length > 1 Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Input file: " + sInput + ". Writing to output " + sOutput)

                Dim doc As Document = New Document(sInput)

                Console.WriteLine("Document Title " + doc.Title)
                Dim newTitle As String = "The Sciences"
                Console.WriteLine("Changed document title to: " + newTitle)

                Console.WriteLine("Document Subject " + doc.Subject)
                Dim newSubject As String = "Biology"
                Console.WriteLine("Changed document subject to: " + newSubject)

                Console.WriteLine("Document Author " + doc.Author)
                Dim newAuthor As String = "John Smith"
                Console.WriteLine("Changed document author to: " + newAuthor)


                Console.WriteLine("Document Keywords '" + doc.Keywords + "'")
                Dim newKeywords As String = "Sciences"
                Console.WriteLine("Changed document keywords to: " + newKeywords)

                Console.WriteLine("Document Creator " + doc.Creator)
                Dim newCreator As String = "Charles Darwin"
                Console.WriteLine("Changed document creator to: " + newCreator)


                Console.WriteLine("Document Producer " + doc.Producer)
                Dim newProducer As String = "Datalogics, Inc"
                Console.WriteLine("Changed document producer to: " + newProducer)

                doc.Title = newTitle
                doc.Subject = newSubject
                doc.Author = newAuthor
                doc.Keywords = newKeywords
                doc.Creator = newCreator
                doc.Producer = newProducer

                doc.Save(SaveFlags.Full Or SaveFlags.Linearized, sOutput)
            End Using
        End Sub
    End Module
End Namespace
