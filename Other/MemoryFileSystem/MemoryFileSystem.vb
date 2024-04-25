Imports Datalogics.PDFL

'
'
' This sample demonstrates how to initialize and use RAM memory instead of the local
' hard disk to save temporary files, in order to save processing time.
'
' The program sets the DefaultTempStore property of the Adobe PDF Library object to
' TempStoreType.Memory. The program can also set a maximum amount of RAM to use by
' applying a value to the DefaultTempStoreMemLimit property.
'
' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
'
'
Namespace MemoryFileSystem
    Module MemoryFileSystem
        Sub Main(args as String())
            Console.WriteLine("MemoryFileSystem Sample:")

            Using library As New Library()
                Console.WriteLine("Initialized the library.")

                Dim sOutput As String = "TempFileSystem.pdf"

                If (args.Length > 0) Then
                    sOutput = args(0)
                End If

                Console.WriteLine("Writing to output " + sOutput)

                Dim bounds As Rect = New Rect(0, 0, 612, 792)

                ' Set in-memory file system as temporary storage
                library.DefaultTempStore = TempStoreType.Memory

                ' Set memory limit up to 100 kB. When occupied memory
                ' exceeds the limit value, disk temporary storage will be used.
                library.DefaultTempStoreMemLimit = 100
                Using doc As Document = New Document()
                    doc.CreatePage(Document.BeforeFirstPage, bounds)
                    doc.Save(SaveFlags.Full, sOutput)
                End Using
            End Using
        End Sub
    End Module
End Namespace
