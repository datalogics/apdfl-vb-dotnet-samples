Imports System
Imports System.Collections.Generic
Imports Datalogics.PDFL

''' This sample demonstrates how to convert a PDF document into a series of graphic image files,
''' one per page. You can also create a multi-page TIFF file. This program requires that you enter 
''' formatting values manually at the command line. 
'''
''' Copyright (c) 2024, Datalogics, Inc. All rights reserved.

Namespace DocToImages

    Public Class DocToImagesOptions
        Private outputformat As ImageType = ImageType.Invalid
        Private color As ColorSpace = ColorSpace.DeviceRGB
        Private grayhalftone As Boolean
        Private firstpageonly As Boolean
        Private quality As Integer
        Private hres As Double = 300.0
        Private vres As Double = 300.0
        Private fontdirs As New List(Of String)(0)
        Private hpixelsize As Integer
        Private vpixelsize As Integer
        Private compress As CompressionCode = CompressionCode.Default
        Private pageregion As String = "crop"
        Private PageList As New List(Of Integer)(0)
        Private evenoddpages As Integer ' 1 = all odd pages, 2 = all even pages.
        Private outputfilename As String = ""
        Private outputdirname As String = ""
        Private smoothingflags As SmoothFlags = SmoothFlags.None
        Private reversegray As Boolean
        Private blackisone As Boolean
        Private multipage As Boolean
        Private ZeroSuffix As Integer
        Private asprinted As Boolean

        Public Sub setoutputformat(format As ImageType)
            outputformat = format
        End Sub

        Public Function getoutputformat() As ImageType
            Return outputformat
        End Function

        Public Sub setcolor(outputcolor As ColorSpace)
            color = outputcolor
        End Sub

        Public Function getcolor() As ColorSpace
            Return color
        End Function

        Public Sub setgrayhalftone(halftone As Boolean)
            grayhalftone = halftone
        End Sub

        Public Function getgrayhalftone() As Boolean
            Return grayhalftone
        End Function

        Public Sub setfirst(firstonly As Boolean)
            firstpageonly = firstonly
        End Sub

        Public Function getfirst() As Boolean
            Return firstpageonly
        End Function

        Public Sub setquality(q As Integer)
            quality = q
        End Sub

        Public Function getquality() As Integer
            Return quality
        End Function

        Public Sub sethres(h As Double)
            hres = h
        End Sub

        Public Function gethres() As Double
            Return hres
        End Function

        Public Sub setvres(v As Double)
            vres = v
        End Sub

        Public Function getvres() As Double
            Return vres
        End Function

        Public Sub setfontdirs(fd As String)
            Dim fontdirarray As String() = fd.Split(";"c)
            For i As Integer = 0 To fontdirarray.Length - 1
                fontdirs.Add(fontdirarray(i))
            Next
        End Sub

        Public Function getfontdirs() As List(Of String)
            Return fontdirs
        End Function

        Public Sub sethpixels(h As Integer)
            hpixelsize = h
        End Sub

        Public Function gethpixels() As Integer
            Return hpixelsize
        End Function

        Public Sub setvpixels(v As Integer)
            vpixelsize = v
        End Sub

        Public Function getvpixels() As Integer
            Return vpixelsize
        End Function

        Public Sub setcompress(cc As CompressionCode)
            compress = cc
        End Sub

        Public Function getcompress() As CompressionCode
            Return compress
        End Function

        Public Sub setpageregion(region As String)
            pageregion = region
        End Sub

        Public Function getpageregion() As String
            Return pageregion
        End Function

        Public Sub appendpagelist(pageno As Integer)
            PageList.Add(pageno)
        End Sub

        ' If this list is empty and the evenoddpages flag is set to 0 then the entire document
        ' will be used.

        Public Function getpagelist() As List(Of Integer)
            Return PageList
        End Function

        ' Once the PDF page is opened the pagecount will be derived and then the evenoddpages flag will be used
        ' to determine which pages are to be used.  If this flag is 0, and the Pagelist is empty, then the entire
        ' document will be used.

        Public Sub setevenpages()
            evenoddpages = 2
        End Sub

        Public Sub setoddpages()
            evenoddpages = 1
        End Sub

        Public Function onlyodd() As Boolean
            Return evenoddpages = 1
        End Function

        Public Function onlyeven() As Boolean
            Return evenoddpages = 2
        End Function

        Public Sub setoutputfile(outputfile As String)
            outputfilename = outputfile
        End Sub

        Public Function getoutputfile() As String
            Return outputfilename
        End Function

        Public Sub setoutputdir(outputdir As String)
            outputdirname = outputdir
        End Sub

        Public Function getoutputdir() As String
            Return outputdirname
        End Function

        Public Sub setsmooth(flags As SmoothFlags)
            smoothingflags = flags
        End Sub

        Public Function getsmooth() As SmoothFlags
            Return smoothingflags
        End Function

        Public Sub setreversegray(reverse As Boolean)
            reversegray = reverse
        End Sub

        Public Function getreversegray() As Boolean
            Return reversegray
        End Function

        Public Sub setblackisone(isblackone As Boolean)
            blackisone = isblackone
        End Sub

        Public Function getblackisone() As Boolean
            Return blackisone
        End Function

        Public Sub setmultipage(multi As Boolean)
            multipage = multi
        End Sub

        Public Function getmultipage() As Boolean
            Return multipage
        End Function

        Public Sub setZeroSuffix(numdigs As Integer)
            ZeroSuffix = numdigs
        End Sub

        Public Function getZeroSuffix() As Integer
            Return ZeroSuffix
        End Function

        Public Sub setasprinted(asp As Boolean)
            asprinted = asp
        End Sub

        Public Function getasprinted() As Boolean
            Return asprinted
        End Function

        Public Function checkformattype() As Boolean
            Return outputformat <> ImageType.Invalid
        End Function

        Public Function checkcolorspacegrayhalftone() As Boolean
            If grayhalftone Then
                Return color = ColorSpace.DeviceGray
            Else
                Return True
            End If
        End Function

        Public Function checkformatcompressiontiff() As Boolean
            If compress = CompressionCode.G3 OrElse compress = CompressionCode.G4 OrElse compress = CompressionCode.LZW Then
                Return outputformat = ImageType.TIFF
            Else
                Return True
            End If
        End Function

        Public Function checkformatcompressionpng() As Boolean
            If compress = CompressionCode.FLATE Then
                Return outputformat = ImageType.PNG
            Else
                Return True
            End If
        End Function

        Public Function checkformatcompressionjpg() As Boolean
            If compress = CompressionCode.DCT Then
                Return outputformat = ImageType.JPEG
            Else
                Return True
            End If
        End Function

        Public Function checkcompressionnone() As Boolean
            If compress = CompressionCode.NONE Then
                Return outputformat = ImageType.BMP OrElse outputformat = ImageType.PNG OrElse outputformat = ImageType.TIFF
            Else
                Return True
            End If
        End Function

        Public Function checkreversegray() As Boolean
            If reversegray Then
                Return color = ColorSpace.DeviceGray
            Else
                Return True
            End If
        End Function

        Public Function checkblackisoneformat() As Boolean
            If blackisone Then
                Return outputformat = ImageType.TIFF
            Else
                Return True
            End If
        End Function

        Public Function checktiffcompressgrayhalftone() As Boolean
            If compress = CompressionCode.G3 OrElse compress = CompressionCode.G4 Then
                Return color = ColorSpace.DeviceGray AndAlso grayhalftone
            ElseIf color = ColorSpace.DeviceGray AndAlso grayhalftone AndAlso outputformat = ImageType.TIFF Then
                Return compress = CompressionCode.G3 OrElse compress = CompressionCode.G4
            Else
                Return True
            End If
        End Function

        Public Function checkqualityimagetype() As Boolean
            If quality <> 0 Then
                Return outputformat = ImageType.JPEG
            Else
                Return True
            End If
        End Function

        Public Function checkgrayhalftoneformat() As Boolean
            If grayhalftone Then
                Return outputformat = ImageType.TIFF
            Else
                Return True
            End If
        End Function

        Public Function checkfirstonlypagerange() As Boolean
            If firstpageonly Then
                Return PageList.Count = 0
            Else
                Return True
            End If
        End Function

        Public Function checkfirstonlyevenodd() As Boolean
            If firstpageonly Then
                Return evenoddpages = 0
            Else
                Return True
            End If
        End Function

        Public Function checkmultiformat() As Boolean
            If multipage Then
                Return outputformat = ImageType.TIFF
            Else
                Return True
            End If
        End Function

        Public Function checkpagelist(pagemax As Integer) As Boolean
            Dim rc As Boolean = True
            If PageList.Count <> 0 Then
                For i As Integer = 0 To PageList.Count - 1
                    If PageList(i) > pagemax Then
                        rc = False
                        Exit For
                    End If
                Next
            End If

            Return rc
        End Function

        Public Function checkfontdirs() As Boolean
            Dim rc As Boolean = True
            For i As Integer = 0 To fontdirs.Count - 1
                If Not System.IO.Directory.Exists(fontdirs(i)) Then
                    Console.WriteLine("The directory path " & fontdirs(i) & " listed in the fontlist options is not a valid directory.")
                    rc = False
                End If
            Next

            Return rc
        End Function
    End Class

    Class DocToImages
        Private Shared Sub Usage()
            Console.WriteLine("DocToImages Usage:")
            Console.WriteLine("DocToImages [options] inputPDF")
            Console.WriteLine("inputPDF is the name of the PDF file to open")
            Console.WriteLine("Options are one or more of:")
            Console.WriteLine("-format=[tif, jpg, bmp, png, gif], No default")
            Console.WriteLine("-color=[gray|cmyk|rgb], default=rgb")
            Console.WriteLine("-grayhalftone = [n|y] - is a grayscale image halftone? Only valid for format=tif and color=gray.")
            Console.WriteLine("-first=[y|n] Only convert the first PDFL page, default=n")
            Console.WriteLine("-quality=1-100. Only valid for an output type of jpg, default for jpg is 75")
            Console.WriteLine("-resolution=[horiz x vert] ( target DPI, [12-1200], default=300")
            Console.WriteLine("    A single value sets both horizontal and vertical the same.")
            Console.WriteLine("    ex: resolution=300 -or - resolution=480x640")
            Console.WriteLine("-fontlist=""dir1;dir2;dirN""")
            Console.WriteLine("    (A quoted list of directories for font resources, semicolon delimited, 16 dirs max.")
            Console.WriteLine("-pixels=[width x height] (absolute picture size, no default)")
            Console.WriteLine("    (If set, output image will be 'width' pixels wide, 'height' pixels tall)")
            Console.WriteLine("    (Otherwise it will be autoscaled from the PDF page.")
            Console.WriteLine("-compression=[default|no|flatelzw|g3|g4|dct]")
            Console.WriteLine("    g3 and g4 are only valid for gray images with grayhalftone=y")
            Console.WriteLine("    flate is only valid for an output format of png")
            Console.WriteLine("    dct is only valid for an output format of jpg")
            Console.WriteLine("    none is only valid for an output format of bmp, png, or tif")
            Console.WriteLine("    lzw is only valid for an output format of tif")
            Console.WriteLine("-region=[crop|media|art|trim|bleed|bounding]")
            Console.WriteLine("    (region of PDF page to rasterize, default=crop")
            Console.WriteLine("-pages=[comma separated list or range, or even or odd], i.e., pages=2,4,7-9,14. Default is all.")
            Console.WriteLine("-output=[filename], (default=input filename)")
            Console.WriteLine("-smoothing=[none|text|all], (default=none")
            Console.WriteLine("-reverse=[y|n], Reverse black/white, for gray images only.")
            Console.WriteLine("-blackisone=[y|n], Reverse black/white, for gray tif images only.")
            Console.WriteLine("-multi=[y|n],create one multipage tif file. Only valid for format=tif.")
            Console.WriteLine("-digits=[0-9], size of the fixed digit suffix added to the file name.")
            Console.WriteLine("-asprinted=[y|n], default=n, Renders only printable annotations.")
        End Sub


        Shared Function CreateFileSuffix(filename As String, imagetype As ImageType) As String
            Dim outputfile As String = ""
            Select Case imagetype
                Case ImageType.BMP
                    outputfile = filename & ".bmp"
                Case ImageType.GIF
                    outputfile = filename & ".gif"
                Case ImageType.JPEG
                    outputfile = filename & ".jpg"
                Case ImageType.PNG
                    outputfile = filename & ".png"
                Case ImageType.TIFF
                    outputfile = filename & ".tif"
            End Select

            Return outputfile
        End Function

        Private Shared Function formatdigits(numdigits As Integer, counter As Integer) As String
            Dim counterstring As String = counter.ToString()
            Dim ZeroSuffix As String = ""
            If counterstring.Length >= numdigits Then
                Return counterstring
            End If

            For x As Integer = 0 To numdigits - counterstring.Length - 1
                ZeroSuffix &= "0"
            Next

            counterstring = ZeroSuffix & counterstring
            Return counterstring
        End Function

        Private Shared Sub saveTheImage(ByVal saveimage As Image, ByVal imageIndex As Integer, ByVal options As DocToImagesOptions, ByVal isp As ImageSaveParams)
            Dim outputfilepath As String
            If options.getoutputdir() <> "" Then
                outputfilepath = options.getoutputdir() & "/" & options.getoutputfile() & formatdigits(options.getZeroSuffix(), imageIndex)
            Else
                outputfilepath = options.getoutputfile() & formatdigits(options.getZeroSuffix(), imageIndex)
            End If

            outputfilepath = CreateFileSuffix(outputfilepath, options.getoutputformat())
            Try
                saveimage.Save(outputfilepath, options.getoutputformat(), isp)
                saveimage.Dispose()
            Catch ex As Exception
                Console.WriteLine("Cannot write an image to a file: " & ex.Message)
            End Try
        End Sub

        Shared Sub Main(args As String())
            Dim docpath As String
            Console.WriteLine("PDF Document to Images Sample:")
            If args.Length < 2 Then
                Usage()
                Environment.Exit(1)
            End If

            Dim options As New DocToImagesOptions()
            If args(args.Length - 1).StartsWith("-") Or args(args.Length - 1).Contains("=") Then
                Console.WriteLine("The last option must be the path to a PDF file.")
                Usage()
                Environment.Exit(1)
            End If

            For i As Integer = 0 To args.Length - 2
                Dim arg As String = args(i)
                If arg.StartsWith("-") AndAlso arg.Contains("=") Then
                    Dim opt As String = arg.Substring(arg.IndexOf("=") + 1)
                    If arg.StartsWith("-format=") Then
                        ' Process output format option
                        Select Case opt
                            Case "jpg"
                                options.setoutputformat(ImageType.JPEG)
                            Case "tif"
                                options.setoutputformat(ImageType.TIFF)
                            Case "bmp"
                                options.setoutputformat(ImageType.BMP)
                            Case "png"
                                options.setoutputformat(ImageType.PNG)
                            Case "gif"
                                options.setoutputformat(ImageType.GIF)
                            Case Else
                                Console.WriteLine("Invalid value for the format option. Valid values are jpg, tif, bmp, png, gif")
                                Environment.Exit(1)
                        End Select
                        Continue For
                    End If

                    If arg.StartsWith("-color=") Then
                        ' Process output color option
                        Select Case opt
                            Case "gray"
                                options.setcolor(ColorSpace.DeviceGray)
                            Case "rgb"
                                options.setcolor(ColorSpace.DeviceRGB)
                            Case "cmyk"
                                options.setcolor(ColorSpace.DeviceCMYK)
                            Case Else
                                Console.WriteLine("Invalid value for the color option. Valid values are gray, rgb, cmyk")
                                Environment.Exit(1)
                        End Select
                        Continue For
                    End If

                    If arg.StartsWith("-grayhalftone=") Then
                        ' Process grayscale half tone
                        If opt.Equals("y") Then
                            options.setgrayhalftone(True)
                        ElseIf opt.Equals("n") Then
                            options.setgrayhalftone(False)
                        Else
                            Console.WriteLine("Invalid value for the grayhalftone option.  Valid values are n or y")
                            Environment.Exit(1)
                        End If
                        Continue For
                    End If

                    If arg.StartsWith("-first=") Then
                        ' Process first page only option
                        If opt.Equals("y") Then
                            options.setfirst(True)
                        ElseIf opt.Equals("n") Then
                            options.setfirst(False)
                        Else
                            Console.WriteLine("Invalid value for the first option.  Valid values are 'y' and 'n'.")
                            Environment.Exit(1)
                        End If
                        Continue For
                    End If

                    If arg.StartsWith("-quality=") Then
                        ' Process jpeg quality option
                        Dim quality As Integer = Integer.Parse(opt)
                        If quality < 1 Then
                            Console.WriteLine("Invalid value for the quality option.  Valid values are 1 through 100.")
                            Environment.Exit(1)
                        End If

                        options.setquality(quality)
                        Continue For
                    End If

                    If arg.Contains("resolution=") Then
                        ' Process horizontal and/or vertical resolution option
                        If opt.Contains("x") Then
                            Dim hopt As String = opt.Substring(0, opt.IndexOf("x"))
                            Dim vopt As String = opt.Substring(opt.IndexOf("x") + 1)
                            options.sethres(Double.Parse(hopt))
                            options.setvres(Double.Parse(vopt))
                        Else
                            Dim res As Double = Double.Parse(opt)
                            options.sethres(res)
                            options.setvres(res)
                        End If

                        Continue For
                    End If

                    If arg.StartsWith("-fontlist=") Then
                        ' process font search directory list option
                        options.setfontdirs(opt)
                        Continue For
                    End If

                    If arg.StartsWith("-pixels=") Then
                        ' process size in pixels option
                        If opt.Contains("x") Then
                            Dim hopt As String = opt.Substring(0, opt.IndexOf("x"))
                            Dim vopt As String = opt.Substring(opt.IndexOf("x") + 1)
                            options.sethpixels(Integer.Parse(hopt))
                            options.setvpixels(Integer.Parse(vopt))
                        Else
                            Dim pixels As Integer = Integer.Parse(opt)
                            options.sethpixels(pixels)
                            options.setvpixels(pixels)
                        End If

                        Continue For
                    End If

                    If arg.StartsWith("-compression=") Then
                        ' process TIFF compression option
                        If opt.Equals("no") Then
                            options.setcompress(CompressionCode.NONE)
                        ElseIf opt.Equals("lzw") Then
                            options.setcompress(CompressionCode.LZW)
                        ElseIf opt.Equals("g3") Then
                            options.setcompress(CompressionCode.G3)
                        ElseIf opt.Equals("g4") Then
                            options.setcompress(CompressionCode.G4)
                        ElseIf opt.Equals("flate") Then
                            options.setcompress(CompressionCode.FLATE)
                        ElseIf opt.Equals("dct") Then
                            options.setcompress(CompressionCode.DCT)
                        ElseIf opt.Equals("default") Then
                            options.setcompress(CompressionCode.Default)
                        ElseIf opt.Equals("none") Then
                            options.setcompress(CompressionCode.NONE)
                        Else
                            Console.WriteLine(
                                "Invalid value for the compression option.  Valid values are: no|lzw|g3|g4|jpg")
                            Environment.Exit(1)
                        End If

                        Continue For
                    End If

                    If arg.StartsWith("-region=") Then
                        ' process page output region option
                        If Not (opt.Equals("crop") OrElse opt.Equals("media") OrElse opt.Equals("art") _
                                OrElse opt.Equals("trim") OrElse opt.Equals("bleed") OrElse opt.Equals("bounding")) Then
                            Console.WriteLine(
                                "Invalid value for the region option.  Value values are: crop|media|art|trim|bleed|bounding")
                            Environment.Exit(1)
                        End If

                        options.setpageregion(opt)
                        Continue For
                    End If

                    If arg.Contains("pages=") Then
                        ' process pages to be rasterized list option
                        If opt.Equals("even") Then
                            options.setevenpages()
                        ElseIf opt.Equals("odd") Then
                            options.setoddpages()
                        Else
                            ' Get the comma separated page groups and check each
                            ' for page ranges.  If page ranges exist then create the
                            ' range using the page numbers on either side of the '-' as
                            ' the lower and upper bound.
                            Dim pagegroups As String() = opt.Split(","c)
                            For ix As Integer = 0 To pagegroups.Length - 1
                                Dim pagegroup As String = pagegroups(ix)
                                If pagegroup.Contains("-") Then
                                    Dim pagerange As String() = pagegroup.Split("-"c)
                                    If pagerange.Length <> 2 OrElse pagerange(0).Equals("") OrElse pagerange(1).Equals("") Then
                                        Console.WriteLine("Misformatted page range: " & pagegroup)
                                        Environment.Exit(1)
                                    Else
                                        For z As Integer = Integer.Parse(pagerange(0)) To Integer.Parse(pagerange(1))
                                            options.appendpagelist(z)
                                        Next
                                    End If
                                Else
                                    Dim printpage As Integer = Integer.Parse(pagegroup)
                                    options.appendpagelist(printpage)
                                End If
                            Next
                        End If

                        Continue For
                    End If

                    If arg.StartsWith("-output=") Then
                        ' process output filename option
                        options.setoutputfile(opt)
                        Continue For
                    End If

                    If arg.StartsWith("-smoothing=") Then
                        ' process smoothing option none|text|all
                        If opt.Equals("none") Then
                            options.setsmooth(SmoothFlags.None)
                        ElseIf opt.Equals("text") Then
                            options.setsmooth(SmoothFlags.Text)
                        ElseIf opt.Equals("all") Then
                            options.setsmooth(SmoothFlags.Image Or SmoothFlags.LineArt Or SmoothFlags.Text)
                        Else
                            Console.WriteLine(
                                "Invalid value for the smoothing option.  Valid values are none|text|all")
                            Environment.Exit(1)
                        End If

                        Continue For
                    End If

                    If arg.StartsWith("-reverse=") Then
                        ' process gray reverse option
                        If opt.Equals("y") Then
                            options.setreversegray(True)
                        ElseIf opt.Equals("n") Then
                            options.setreversegray(False)
                        Else
                            Console.WriteLine("Invalid value for the reverse option.  Valid values are 'y' and 'n'.")
                            Environment.Exit(1)
                        End If

                        Continue For
                    End If

                    If arg.StartsWith("-blackisone=") Then
                        ' Photometrically reverse Tiff  option
                        If opt.Equals("y") Then
                            options.setblackisone(True)
                        ElseIf opt.Equals("n") Then
                            options.setblackisone(False)
                        Else
                            Console.WriteLine(
                                "Invalid value for the blackisone option.  Valid values are 'y' and 'n'.")
                            Environment.Exit(1)
                        End If

                        Continue For
                    End If

                    If arg.StartsWith("-multi=") Then
                        ' process multipage tif option
                        If opt.Equals("y") Then
                            options.setmultipage(True)
                        ElseIf opt.Equals("n") Then
                            options.setmultipage(False)
                        Else
                            Console.WriteLine("Invalid value for the multi option.  Valid values are 'y' and 'n'.")
                            Environment.Exit(1)
                        End If

                        Continue For
                    End If

                    If arg.StartsWith("-digits=") Then
                        ' process fixed digit suffix length option
                        Dim numdigits As Integer = Integer.Parse(opt)
                        If numdigits < 0 OrElse numdigits > 9 Then
                            Console.WriteLine("Invalid value for the digits option. Valid values are 0-9.")
                        End If

                        options.setZeroSuffix(numdigits)
                        Continue For
                    End If

                    If arg.StartsWith("-asprinted=") Then
                        ' process render only printable annotations option
                        If opt.Equals("y") Then
                            options.setasprinted(True)
                        ElseIf opt.Equals("n") Then
                            options.setasprinted(False)
                        Else
                            Console.WriteLine("Invalid value for the asprinted option.  Valid values are 'y' and 'n'.")
                            Environment.Exit(1)
                        End If

                        Continue For
                    End If

                    ' Output invalid option and usage in case of unrecognized option
                    Console.WriteLine("Invalid option: " & arg)
                    Usage()
                    Environment.Exit(1)
                Else
                    Console.WriteLine("Invalid option: " & arg)
                    Usage()
                    Environment.Exit(1)
                End If
            Next

            ' Process the PDF file path
            docpath = args(args.Length - 1)
            ' Now crosscheck and verify the combinations of the options we have entered.
            Dim errorcount As Integer = 0

            If options.checkformattype() = False Then
                Console.WriteLine("format must be set to tif, jpg, bmp, png, or gif")
                errorcount += 1
            End If

            If options.checkcolorspacegrayhalftone() = False Then
                Console.WriteLine("grayhalftone can only be set to 1 for a value of 'gray' for color")
                errorcount += 1
            End If

            If options.checkformatcompressiontiff() = False Then
                Console.WriteLine("Compression can only be this value for a format value of 'tif'")
                errorcount += 1
            End If

            If options.checkformatcompressionpng() = False Then
                Console.WriteLine("Compression can only be this value for a format value of 'png'")
                errorcount += 1
            End If

            If options.checkformatcompressionjpg() = False Then
                Console.WriteLine("Compression can only be this value for a format value of 'jpg'")
                errorcount += 1
            End If

            If options.checkcompressionnone() = False Then
                Console.WriteLine("Compression can only be this value for a format value of 'bmp' or 'png' or 'tif'")
                errorcount += 1
            End If

            If options.checkreversegray() = False Then
                Console.WriteLine("reverse can only be set to 'y' for a value of 'gray' for color")
                errorcount += 1
            End If

            If options.checkblackisoneformat() = False Then
                Console.WriteLine("blackisone can only be set to 'y' for a format value of 'tif'")
                errorcount += 1
            End If

            If options.checktiffcompressgrayhalftone() = False Then
                Console.WriteLine("compression can only be set to 'g3' or 'g4' if grayhalftone is set to 'y' and color is set to 'gray'")
                errorcount += 1
            End If

            If options.checkgrayhalftoneformat() = False Then
                Console.WriteLine("grayhalftone can only be set to 'y' for format set to tif")
                errorcount += 1
            End If

            If options.checkqualityimagetype() = False Then
                Console.WriteLine("quality can only be set to a value other than 0 for a format value of 'jpg'")
                errorcount += 1
            End If

            If options.checkfirstonlypagerange() = False Then
                Console.WriteLine("The pages option cannot be specified if firstonly is set to 'y'")
                errorcount += 1
            End If

            If options.checkfirstonlyevenodd() = False Then
                Console.WriteLine("The pages option cannot be set to 'even' or 'odd' if firstonly is set to 'y'")
                errorcount += 1
            End If

            If options.checkmultiformat() = False Then
                Console.WriteLine("The multi option can only be set to 'y' if format is set to 'tif'")
                errorcount += 1
            End If

            If options.checkfontdirs() <> True Then
                errorcount += 1
            End If

            If errorcount > 0 Then
                Console.WriteLine("Because of command line option errors, no image processing will be performed.")
                Environment.Exit(1)
            End If



            ' ReSharper disable once UnusedVariable
            Using library As New Library(options.getfontdirs())
                Dim pdfdocument As Document = Nothing
                Dim numpages As Integer = 0
                Try
                    pdfdocument = New Document(docpath)
                    numpages = pdfdocument.NumPages
                Catch ex As Exception
                    Console.WriteLine("Error opening PDF document " & docpath & ":" & ex.Message)
                    Environment.Exit(1)
                End Try

                ' Now that we know we can open the PDF file, use the filename to create what will be the basis
                ' of the output filename and directory name.

                Dim outputfilename As String
                If options.getoutputfile() = "" Then
                    outputfilename = docpath
                Else
                    outputfilename = options.getoutputfile()
                End If

                options.setoutputdir(System.IO.Path.GetDirectoryName(outputfilename))
                Dim basefilename As String() = System.IO.Path.GetFileName(outputfilename).Split("."c) ' split off the .pdf suffix.
                options.setoutputfile(basefilename(0))


                If options.checkpagelist(numpages) = False Then
                    Console.WriteLine("The list of pages given in the 'pages' option is outside the number of pages in this PDF document: " &
                          pdfdocument.NumPages)
                    Environment.Exit(1)
                End If

                ' If the list of pages were not populated from the command line options, or if the options have specified
                ' all odd or all even, then populate the list here and then reget the list.

                Dim pagelist As List(Of Integer) = options.getpagelist()
                If pagelist.Count = 0 Then
                    If options.getfirst() Then ' First page only.
                        numpages = 1 ' Will modify the operation of the loop below.
                    End If

                    For i As Integer = 0 To numpages - 1
                        ' Remember in the Doc object page #'s start with 0, but physically they start with 1.
                        If (options.onlyodd() = False AndAlso options.onlyeven() = False) OrElse ' all pages
                (options.onlyodd() AndAlso (((i + 1) Mod 2) = 1)) OrElse ' this is an odd page
                (options.onlyeven() AndAlso (((i + 1) Mod 2) = 0)) Then ' this is an even page
                            options.appendpagelist(i)
                        End If
                    Next

                    pagelist = options.getpagelist()
                End If

                Dim pip As New PageImageParams(options.getcolor(), options.getsmooth(), options.gethpixels(),
                                    options.getvpixels(), options.gethres(), options.getvres(),
                                    (DrawFlags.UseAnnotFaces Or DrawFlags.DoLazyErase))
                If options.getasprinted() Then
                    pip.PageDrawFlags = pip.PageDrawFlags Or DrawFlags.IsPrinting
                End If

                Dim Pageimagecollection As New ImageCollection()

                If options.getoutputformat() = ImageType.TIFF AndAlso options.getcompress() = CompressionCode.NONE Then
                    options.setcompress(CompressionCode.LZW) ' Default for TIF
                End If

                Dim isp As New ImageSaveParams()
                isp.HalftoneGrayImages = options.getgrayhalftone()
                isp.Compression = options.getcompress()
                If options.getoutputformat() = ImageType.JPEG Then
                    isp.JPEGQuality = options.getquality()
                End If

                isp.ReverseGray = options.getreversegray()
                isp.TIFFBlackIsOne = options.getblackisone()

                ' Get the images of the PDF pages to create an image collection.
                For i As Integer = 0 To pagelist.Count - 1
                    Dim docpage As Page = pdfdocument.GetPage(pagelist(i))
                    Dim PageRect As Rect = Nothing
                    If options.getpageregion().Equals("crop") Then
                        PageRect = docpage.CropBox
                    ElseIf options.getpageregion().Equals("media") Then
                        PageRect = docpage.MediaBox
                    ElseIf options.getpageregion().Equals("art") Then
                        PageRect = docpage.ArtBox
                    ElseIf options.getpageregion().Equals("bounding") Then
                        PageRect = docpage.BBox
                    ElseIf options.getpageregion().Equals("bleed") Then
                        PageRect = docpage.BleedBox
                    ElseIf options.getpageregion().Equals("trim") Then
                        PageRect = docpage.TrimBox
                    Else
                        Console.WriteLine("Unknown page region option.")
                        Environment.Exit(1)
                    End If

                    Try
                        Dim pageimage As Image = docpage.GetImage(PageRect, pip)
                        If options.getmultipage() Then
                            Pageimagecollection.Append(pageimage)
                        Else
                            Dim x As Integer = i + 1
                            saveTheImage(pageimage, x, options, isp)
                        End If
                    Catch ex As Exception
                        Console.WriteLine("Cannot rasterize page to an image: " & ex.Message)
                        Environment.Exit(1)
                    End Try
                Next

                ' Pageimagecollection now Contains, as the name states,
                ' a collection of images created from the PDF pages according
                ' to the user's options. Now to post process them to the image.
                ' type according to the user's options.
                If options.getmultipage() Then
                    Dim outputfilepath As String
                    If options.getoutputdir() <> "" Then
                        outputfilepath = options.getoutputdir() & "/" & options.getoutputfile()
                    Else
                        outputfilepath = options.getoutputfile()
                    End If

                    outputfilepath = CreateFileSuffix(outputfilepath, options.getoutputformat())
                    Try
                        Pageimagecollection.Save(outputfilepath, options.getoutputformat(), isp)
                    Catch ex As Exception
                        Console.WriteLine("Cannot save images to a multi-page TIF file: " & ex.Message)
                    End Try
                End If
            End Using



        End Sub






    End Class


End Namespace

