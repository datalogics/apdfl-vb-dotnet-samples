Imports System
Imports System.Collections.Generic
Imports Datalogics.PDFL

 '
 '
 ' The ColorConvertDocument sample program demonstrates working with color conversion in PDF documents.
 ' The color conversion process allows you to apply a different color profile to an object in a PDF document,
 ' and thus effectively change the colors found in that object. This process applies a set of colors to an
 ' image or other object within a PDF and embeds that information in the document, so that the right set of
 ' colors will be rendered when the PDF document is sent to a printer or to another output device.
 ' 
 ' Note that the color profile is not embedded by default rather, the default is not to embed the color profile.
 ' The user must set the option to embed to True.
 ' 
 ' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.
 '
 '
Namespace ColorConvertDocument
    Module ColorConvertDocument
        Sub Main(args As String())
            Console.WriteLine("ColorConvertDocument Sample:")

            Dim paths As List(Of String) = New List(Of String)
            paths.Add(Library.ResourceDirectory + "Fonts/")

            Using New Library(paths, Library.ResourceDirectory + "CMap/",
                Library.ResourceDirectory + "Unicode/", Library.ResourceDirectory + "Color",
                LibraryFlags.DisableMemorySuballocator)

                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/ducky.pdf"
                Dim sOutput As String = "ColorConvertDocument-out.pdf"

                If (args.Length > 0) Then
                    sInput = args(0)
                End If
                If (args.Length > 1) Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Input file: " + sInput + ", writing to " + sOutput)

                Dim doc As Document = New Document(sInput)

                ' Create the list of color conversion actions to be applied to the document. Each object in the document is compared
                ' against the selection criteria for each of the actions until a matching action is found. Actions do not chain,
                ' except in the case of aliased ink definitions
                '
                Dim colorConvActions As List(Of ColorConvertActions) = New List(Of ColorConvertActions)
                Dim action As ColorConvertActions = New ColorConvertActions()

                ' In this example, make any object in the document a candidate for color conversion. Also allow for any kind of Color Space.
                ' The ColorConvertObjAttrs values can be combined together for more specific matching patterns Using the | operator.
                ' This is also true for Color Spaces.
                '
                action.MustMatchAnyAttrs = ColorConvertObjAttrs.ColorConvAnyObject
                action.MustMatchAnyCSAttrs = ColorConvertCSpaceType.ColorConvAnySpace
                action.IntentToMatch = RenderIntent.UseProfileIntent
                action.ConvertIntent = RenderIntent.Perceptual
                action.Action = ColorConvertActionType.ConvertColor
                action.ConvertProfile = ColorProfile.DotGain10

                'Actions that should be applied to inks need to have its IsInkAction property set to true before being added to the list'
                colorConvActions.Add(action)

                'Once all the actions to be performed are on the list, a ColorConvertParams object is created to hold them. Defaults for 
                'Render Intent and Device Color Profiles can also be set here. 
                Dim parms As ColorConvertParams = New ColorConvertParams(colorConvActions)

                Dim success As Boolean = doc.ColorConvertPages(parms)

                doc.Save(SaveFlags.Full Or SaveFlags.Compressed, sOutput)
            End Using
        End Sub
    End Module
End Namespace
