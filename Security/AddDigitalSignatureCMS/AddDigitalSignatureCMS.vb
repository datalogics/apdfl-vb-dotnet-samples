Imports Datalogics.PDFL

'
'
' This sample shows how to add a CMS Digital Signature
'
' Copyright (c) 2025, Datalogics, Inc. All rights reserved.
'
'
Namespace AddDigitalSignatureCMS
    Module AddDigitalSignatureCMS
        Sub Main(args As String())
            Console.WriteLine("AddDigitalSignatureCMS Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")
                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/SixPages.pdf"
                Dim sLogo As String = Library.ResourceDirectory & "Sample_Input/ducky_alpha.tif"
                Dim sOutput As String = "DigSigCMS-out.pdf"

                Dim sDERCert As String = Library.ResourceDirectory & "Sample_Input/Credentials/DER/RSA_certificate.der"
                Dim sDERKey As String = Library.ResourceDirectory & "Sample_Input/Credentials/DER/RSA_privKey.der"

                If (args.Length > 0) Then
                    sInput = args(0)
                End If
                If (args.Length > 1) Then
                    sInput = args(1)
                End If
                If (args.Length > 2) Then
                    sInput = args(2)
                End If

                Console.WriteLine("Input file: " + sInput)

                Using doc As New Document(sInput)
                    Using sigDoc As New SignDoc()
                        ' Setup Sign params
                        sigDoc.FieldID = SignatureFieldID.CreateFieldWithQualifiedName
                        sigDoc.FieldName = "Signature_es_:signatureblock"

                        ' Set credential related attributes
                        sigDoc.DigestCategory = DigestCategory.Sha256
                        sigDoc.CredentialDataFormat = CredentialDataFmt.NonPFX
                        sigDoc.SetNonPfxSignerCert(sDERCert, 0, CredentialStorageFmt.OnDisk)
                        sigDoc.SetNonPfxPrivateKey(sDERKey, 0, CredentialStorageFmt.OnDisk)

                        ' Set the signature type to be used.
                        ' The available types are defined in the SignatureType enum. Default CMS.
                        sigDoc.DocSignType = SignatureType.CMS

                        ' Setup the signer information
                        ' (Logo image is optional)
                        sigDoc.SetSignerInfo(sLogo, 0.5F, "John Doe", "Chicago, IL", "Approval", "Datalogics, Inc.", DisplayTraits.KDisplayAll)

                        ' Set the size and location of the signature box (optional)
                        ' If not set, invisible signature will be placed on first page
                        sigDoc.SignatureBoxPageNumber = 0
                        sigDoc.SignatureBoxRectangle = New Rect(100, 300, 400, 400)

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
