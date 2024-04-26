Imports System
Imports System.Collections.Generic
Imports Datalogics.PDFL




''' This sample changes the On/Off configuration for a set of Optional Content Groups,
''' or layers, within a PDF document. By changing the On or Off state in the default
''' configuration, the sample makes the layers visible or invisible when opened in a 
''' PDF viewer.
''' 
''' The sample changes the states of the layers in the document called Layers.pdf and
''' saves the result to a new PDF document.
''' 
''' 
''' Copyright (c) 2007-2024, Datalogics, Inc. All rights reserved.


Namespace ChangeLayerConfiguration
    Class ChangeLayerConfiguration
        Shared Sub Main(args As String())
            Console.WriteLine("ChangeLayerConfiguration Sample:")

            Using library As New Library()
                Console.WriteLine("Initialized the library.")

                ' Define input and output file paths
                Dim sInput As String = Library.ResourceDirectory & "Sample_Input/Layers.pdf"
                Dim sOutput As String = "ChangeLayerConfiguration-out.pdf"

                If args.Length > 0 Then
                    sInput = args(0)
                End If

                If args.Length > 1 Then
                    sOutput = args(1)
                End If

                Console.WriteLine("Input file: " & sInput & ", writing to " & sOutput)

                Using doc As New Document(sInput)
                    ' Get the default OptionalContentConfig and OptionalContentGroups
                    Dim config As OptionalContentConfig = doc.DefaultOptionalContentConfig
                    Dim ocgs As IList(Of OptionalContentGroup) = doc.OptionalContentGroups

                    ' Create a 'working' array
                    Dim worklist As New List(Of OptionalContentGroup)()

                    ' Set the BaseState to Off
                    config.BaseState = OptionalContentBaseState.BaseStateOff

                    ' Add 'Layer 3' and 'Guides and Grids' to the ON array
                    For Each layer As OptionalContentGroup In ocgs
                        If layer.Name = "Layer 3" OrElse layer.Name = "Guides and Grids" Then
                            worklist.Add(layer)
                        End If
                    Next

                    ' Now set the ON array to our new set of layers, and the OFF array to empty
                    ' Clearing the OFF array is IMPORTANT! When we opened the Layers.pdf, 
                    ' it had the 'Guides and Grids' layer in the OFF array; if we leave it in there, 
                    ' it will appear in BOTH the ON and OFF arrays and cause unpredictable behavior
                    config.OnArray = worklist
                    config.OffArray = New List(Of OptionalContentGroup)()

                    ' Save the new document
                    doc.Save(SaveFlags.Full, sOutput)
                End Using
            End Using
        End Sub
    End Class
End Namespace

