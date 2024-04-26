Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports Datalogics.PDFL

'
 ' This sample demonstrates Using DocTextFinder to find instances of a phrase
 ' that matches a user-supplied regular expression. The output is a JSON file that
 ' has the match information.
 '
 ' Copyright (c) 2021-2024, Datalogics, Inc. All rights reserved.
 '
 '

Namespace RegexExtractText
    ' This class represents a match quad's bottom left coordinate.
    Class BottomLeft
        Property x As Double
        Property y As Double
    End Class

    ' This class represents a match quad's bottom right coordinate.
    Class BottomRight
        Property x As Double
        Property y As Double
    End Class

    ' This class represents a match quad's top left coordinate.
    Class TopLeft
        Property x As Double
        Property y As Double
    End Class

    ' This class represents a match quad's top right coordinate.
    Class TopRight
        Property x As Double
        Property y As Double
    End Class

    ' This class represents the 4 coordinates that make up a match quad.
    Class QuadLocation
        <JsonPropertyName("bottom-left")>
        Property bottomLeft As BottomLeft

        <JsonPropertyName("bottom-right")>
        Property bottomRight As BottomRight

        <JsonPropertyName("top-left")>
        Property topLeft As TopLeft

        <JsonPropertyName("top-right")>
        Property topRight As TopRight
    End Class

    ' This class represents a match quad's location (the quad coordinates and page number that quad is located on).
    Class MatchQuadInformation
        <JsonPropertyName("page-number")>
        Property PageNumber As Integer

        <JsonPropertyName("quad-location")>
        Property quadLocation As QuadLocation
    End Class

    ' This class represents the information that is associated with a match (match phrase and match quads).
    Class MatchObject
        <JsonPropertyName("match-phrase")>
        Property matchPhrase As String

        <JsonPropertyName("match-quads")>
        Property matchQuads As List(Of MatchQuadInformation)
    End Class

    ' This class represents the final JSON that will be written to the output JSON file.
    Class DocTextFinderJson
        Property documentJson As List(Of MatchObject)
    End Class

    Module RegexExtractText
        Sub Main(args As String())
            Console.WriteLine("RegexExtractText Sample:")

            Using New Library()
                Console.WriteLine("Initialized the library.")

                Dim sInput As String = Library.ResourceDirectory + "Sample_Input/RegexExtractText.pdf"
                Dim sOutput As String = "RegexExtractText-out.json"

                ' Uncomment only one regular expression you are interested in seeing the match information of (as a JSON file).
                ' Phone numbers
                Dim sRegex As String = "((1-)?(\()?\d{3}(\))?(\s)?(-)?\d{3}-\d{4})"
                ' Email addresses
                'Dim sRegex As String = "(\b[\w.!#$%&'*+\/=?^`{|}~-]+@[\w-]+(?:\.[\w-]+)*\b)";
                ' URLs
                ' Dim sRegex As String = "((https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,}))"

                If (args.Length > 0) Then
                    sInput = args(0)
                End If

                Using doc As Document = New Document(sInput)
                    Dim nPages As Integer = doc.NumPages

                    Console.WriteLine("Input file:  " + sInput)

                    ' This will hold the JSON stream that we will print to the output JSON file.
                    Dim result As DocTextFinderJson = New DocTextFinderJson()
                    result.documentJson = New List(Of MatchObject)

                    Dim wordConfig As WordFinderConfig = New WordFinderConfig()

                    ' Need to set this to True so phrases will be concatenated properly.
                    wordConfig.NoHyphenDetection = True

                    ' Create a DocTextFinder with the default wordfinder parameters.
                    Using docTextFinder As DocTextFinder = New DocTextFinder(doc, wordConfig)
                        ' Retrieve the phrases matching a regular expression.
                        Dim docMatches As IList(Of DocTextFinderMatch) = docTextFinder.GetMatchList(0, nPages - 1, sRegex)

                        ' Iterate through the matches and add match information to the DocTextFinderJson object.
                        For Each wInfo As DocTextFinderMatch In docMatches
                            ' This object will store the match phrase and an array of quads for the match.
                            Dim matchObject As MatchObject = New MatchObject()

                            ' This list will store the page number and quad location for each match quad.
                            Dim matchQuadInformationList As List(Of MatchQuadInformation) = New List(Of MatchQuadInformation)

                            ' Set the match phrase in the matchObject.
                            matchObject.matchPhrase = wInfo.MatchString

                            ' Get the quads.
                            Dim QuadInfo As IList(Of DocTextFinderQuadInfo) = wInfo.QuadInfo

                            For Each qInfo As DocTextFinderQuadInfo In QuadInfo
                                Dim temp As MatchQuadInformation = New MatchQuadInformation()
                                temp.PageNumber = qInfo.PageNum

                                ' Iterate through the quads and insert the quad information into the matchQuadInformation object.
                                For Each quad As Quad In qInfo.Quads
                                    Dim quadLocation As QuadLocation = New QuadLocation()
                                    quadLocation.topLeft = New TopLeft()
                                    quadLocation.bottomLeft = New BottomLeft()
                                    quadLocation.topRight = New TopRight()
                                    quadLocation.bottomRight = New BottomRight()

                                    quadLocation.topLeft.x = quad.TopLeft.H
                                    quadLocation.topLeft.y = quad.TopLeft.V

                                    quadLocation.bottomLeft.x = quad.BottomLeft.H
                                    quadLocation.bottomLeft.y = quad.BottomLeft.V

                                    quadLocation.topRight.x = quad.TopRight.H
                                    quadLocation.topRight.y = quad.TopRight.V

                                    quadLocation.bottomRight.x = quad.BottomRight.H
                                    quadLocation.bottomRight.y = quad.BottomRight.V

                                    temp.quadLocation = quadLocation
                                    matchQuadInformationList.Add(temp)
                                Next
                            Next
                            matchObject.matchQuads = matchQuadInformationList
                            result.documentJson.Add(matchObject)
                        Next
                        ' Save the output JSON file.
                        Console.WriteLine("Writing JSON to " + sOutput)
                        Dim options As JsonSerializerOptions = New JsonSerializerOptions()
                        options.WriteIndented = True
                        Dim json As String = JsonSerializer.Serialize(result.documentJson, options)
                        System.IO.File.WriteAllText(sOutput, json)
                    End Using
                End Using
            End Using
        End Sub
    End Module
End Namespace
