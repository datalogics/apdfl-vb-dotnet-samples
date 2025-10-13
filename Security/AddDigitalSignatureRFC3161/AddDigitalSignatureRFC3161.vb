Imports Datalogics.PDFL

'
'
' This sample shows how to add an RFC3161 Timestamp Digital Signature
'
' Copyright (c) 2025, Datalogics, Inc. All rights reserved.
'
'
Namespace AddDigitalSignatureRFC3161
    Module AddDigitalSignatureRFC3161
        Sub Main(args As String())
            Console.WriteLine("AddDigitalSignatureRFC3161 Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")
                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/CreateAcroForm2h.pdf"

                Dim sOutput As String = "DigSigRFC3161-out.pdf"

                If (args.Length > 0) Then
                    sInput = args(0)
                End If
                If (args.Length > 1) Then
                    sInput = args(1)
                End If

                Console.WriteLine("Input file: " + sInput)

                Using doc As New Document(sInput)
                    Using sigDoc As New SignDoc()
                        ' Setup Sign params
                        sigDoc.FieldID = SignatureFieldID.SearchForFirstUnsignedField

                        ' Set credential related attributes
                        sigDoc.DigestCategory = DigestCategory.Sha256

                        ' Set the signature type to be used, RFC3161/TimeStamp.
                        ' The available types are defined in the SignatureType enum. Default CMS.
                        sigDoc.DocSignType = SignatureType.RFC3161

                        ' Setup Save params
                        sigDoc.OutputPath = sOutput

                        ' Finally, sign and save the document
                        sigDoc.AddDigitalSignature(doc)
                    End Using
                End Using
            End Using
        End Sub
    End Module
End Namespace
