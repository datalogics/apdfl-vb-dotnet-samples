Imports Datalogics.PDFL

''' This sample adds Optional Content Groups (layers) to a PDF document and
''' then adds Content to those layers.
''' 
''' The related ChangeLayerConfiguration program makes layers visible or invisible.
''' 
''' You can toggle back and forth to make a layer visible or invisible
''' in a PDF Viewer.
'''
''' Copyright (c) 2007-2025, Datalogics, Inc. All rights reserved.

Namespace CreateLayer
    Class CreateLayer
        Shared Sub Main(args As String())
            Console.WriteLine("CreateLayer Sample:")

            Using library As New Library()
                Console.WriteLine("Initialized the library.")

                ' Define input and output file paths
                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/ducky.pdf"
                Dim sOutput As String = "CreateLayer-out.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                If args.Length > 1 Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Input file: " & sInput & ", writing to " & sOutput)

                Using doc As New Document(sInput)
                    Console.WriteLine("Opened a document.")

                    Dim pg As Page = doc.GetPage(0)
                    Dim element As Element = pg.Content.GetElement(0)

                    If TypeOf element Is Image Then
                        Dim image As Image = DirectCast(element, Image)
                        image.Matrix = New Matrix(image.Matrix.A * 0.5, 0, 0, image.Matrix.D * 0.5, image.Matrix.H, image.Matrix.V)

                        Dim image2 As Image = New Image(Library.ResourceDirectory & "Sample_Input/Image.png")

                        Dim text As Text = New Text()
                        Dim matrix As Matrix = New Matrix()
                        Dim font As Font = New Font("Helvetica")
                        Dim graphicState As GraphicState = New GraphicState()
                        Dim textState As TextState = New TextState()

                        matrix.A = 42
                        matrix.D = 22
                        matrix.H = 72
                        matrix.V = 72

                        Dim textRun As TextRun = New TextRun("sample text", font, graphicState, textState, matrix)
                        text.AddRun(TextRun)

                        Dim text2 As Text = New Text()

                        matrix.A = 30
                        matrix.D = 30
                        matrix.H = 72
                        matrix.V = 288

                        Dim textRun2 As TextRun = New TextRun("Text definition provided here", font, graphicState, textState, matrix)
                        text2.AddRun(textRun2)

                        ' Containers, Forms and Annotations can be attached to an
                        ' OptionalContentGroup; other content (like Image) can
                        ' be made optional by placing it inside a Container
                        Dim imageContainer As New Container()
                        imageContainer.Content = New Content()
                        imageContainer.Content.AddElement(image)

                        Dim imageContainer2 As New Container()
                        imageContainer2.Content = New Content()
                        imageContainer2.Content.AddElement(image2)

                        Dim textContainer As New Container()
                        textContainer.Content = New Content()
                        textContainer.Content.AddElement(text)

                        Dim textContainer2 As New Container()
                        textContainer2.Content = New Content()
                        textContainer2.Content.AddElement(text2)

                        Using newDoc As New Document()
                            Using newPage = newDoc.CreatePage(Document.BeforeFirstPage, pg.MediaBox)
                                newPage.Content.AddElement(imageContainer)
                                newPage.Content.AddElement(imageContainer2)
                                newPage.Content.AddElement(textContainer)
                                newPage.Content.AddElement(textContainer2)

                                ' We create new OptionalContentGroups and place them in the OptionalContentConfig.Order array
                                Dim theStrings As String() = {"Rubber Ducky", "PNG Logo", "Example Text", "Text Definition"}
                                Dim ocgs As List(Of OptionalContentGroup) = CreateNewOptionalContentGroups(newDoc, theStrings.ToList())

                                AssociateOCGWithContainer(newDoc, ocgs(0), imageContainer)
                                AssociateOCGWithContainer(newDoc, ocgs(1), imageContainer2)
                                AssociateOCGWithContainer(newDoc, ocgs(2), textContainer)
                                AssociateOCGWithContainer(newDoc, ocgs(3), textContainer2)

                                newPage.UpdateContent()

                                newDoc.Save(SaveFlags.Full, sOutput)
                            End Using
                        End Using
                    End If
                End Using
            End Using
        End Sub

        Public Shared Function CreateNewOptionalContentGroups(doc As Document, names As List(Of String)) As List(Of OptionalContentGroup)
            Dim ocgs As New List(Of OptionalContentGroup)

            Dim ocg As OptionalContentGroup = New OptionalContentGroup(doc, names(0))
            Dim ocg2 As OptionalContentGroup = New OptionalContentGroup(doc, names(1))
            Dim ocg3 As OptionalContentGroup = New OptionalContentGroup(doc, names(2))
            Dim ocg4 As OptionalContentGroup = New OptionalContentGroup(doc, names(3))

            ocgs.Add(ocg)
            ocgs.Add(ocg2)
            ocgs.Add(ocg3)
            ocgs.Add(ocg4)

            ' Add it to the Order array -- this Is required so that it will appear in the 'Layers' panel in a PDF Viewer.
            Dim order_list As OptionalContentOrderArray = doc.DefaultOptionalContentConfig.Order

            Dim grouping As OptionalContentOrderArray = New OptionalContentOrderArray(doc, "Image Grouping")
            grouping.Add(New OptionalContentOrderLeaf(ocg))
            grouping.Add(New OptionalContentOrderLeaf(ocg2))

            Dim grouping2 As OptionalContentOrderArray = New OptionalContentOrderArray(doc, "Text Grouping")
            grouping2.Add(New OptionalContentOrderLeaf(ocg3))
            grouping2.Add(New OptionalContentOrderLeaf(ocg4))

            order_list.Insert(order_list.Length, grouping)
            order_list.Insert(order_list.Length, grouping2)
            Return ocgs
        End Function

        ' Associate a Container with an OptionalContentGroup via an OptionalContentMembershipDict.
        ' This function associates a Container with a single OptionalContentGroup and uses
        ' a VisibilityPolicy of AnyOn.
        Public Shared Sub AssociateOCGWithContainer(doc As Document, ocg As OptionalContentGroup, cont As Container)
            ' Create an OptionalContentMembershipDict.  The options here are appropriate for a
            ' 'typical' usage; other options can be used to create an 'inverting' layer
            ' (i.e. 'Display this content when the layer is turned OFF'), or to make the
            ' Container's visibility depend on several OptionalContentGroups
            Dim ocmd As New OptionalContentMembershipDict(doc, New OptionalContentGroup() {ocg}, VisibilityPolicy.AnyOn)

            ' Associate the Container with the OptionalContentMembershipDict
            cont.OptionalContentMembershipDict = ocmd
        End Sub
    End Class
End Namespace
