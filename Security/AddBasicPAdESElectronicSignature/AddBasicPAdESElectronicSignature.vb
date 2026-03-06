Imports Datalogics.PDFL

'
'
' This sample program demonstrates the use of AddDigitalSignature for PAdES
' (PDF Advanced Electronic Signatures) baseline signature type without a
' signature policy. PAdES signatures conform to the ETSI standard and use
' the ETSI.CAdES.detached SubFilter.
'
' Copyright (c) 2026, Datalogics, Inc. All rights reserved.
'
'
Namespace AddBasicPAdESElectronicSignature
    Module AddBasicPAdESElectronicSignature
        Sub Main(args As String())
            Console.WriteLine("AddBasicPAdESElectronicSignature Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")
                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/SixPages.pdf"
                Dim sLogo As String = Library.ResourceDirectory & "Sample_Input/ducky_alpha.tif"
                Dim sOutput As String = "PAdESBaselineSignature-out.pdf"

                Dim sPEMCert As String = Library.ResourceDirectory & "Sample_Input/Credentials/PEM/ecSecP521r1Cert.pem"
                Dim sPEMKey As String = Library.ResourceDirectory & "Sample_Input/Credentials/PEM/ecSecP521r1Key.pem"

                If (args.Length > 0) Then
                    sInput = args(0)
                End If
                If (args.Length > 1) Then
                    sOutput = args(1)
                End If
                If (args.Length > 2) Then
                    sLogo = args(2)
                End If

                Console.WriteLine("Input file: " & sInput)
                Console.WriteLine("Writing to output: " & sOutput)

                Using doc As New Document(sInput)
                    Using sigDoc As New SignDoc()
                        ' Setup Sign params
                        sigDoc.FieldID = SignatureFieldID.CreateFieldWithQualifiedName
                        sigDoc.FieldName = "Signature_es_:signatureblock"

                        ' Set credential related attributes
                        sigDoc.DigestCategory = DigestCategory.Sha384
                        sigDoc.CredentialDataFormat = CredentialDataFmt.NonPFX
                        sigDoc.SetNonPfxSignerCert(sPEMCert, 0, CredentialStorageFmt.OnDisk)
                        sigDoc.SetNonPfxPrivateKey(sPEMKey, 0, CredentialStorageFmt.OnDisk)

                        ' Set the signature type to PAdES (PDF Advanced Electronic Signatures).
                        ' This produces an ETSI.CAdES.detached signature conforming to the
                        ' PAdES baseline profile without a signature policy.
                        sigDoc.DocSignType = SignatureType.PADES

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
