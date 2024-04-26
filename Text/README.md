## ***ExtractTextFromAnnotations***
Extracts text from the annotations in a PDF document and saves the text to a JSON file indicating what type of annotation and the text within it. If you need consolidate all the notes and feedback that were added as annotations in your PDFs, this function can help you pull out that data for easy viewing and/or printing.

## ***ExtractTextFromMultiRegions***
Processes example invoice PDF files in a folder that share the same page layout and extracts text from specific regions of its pages and saves the text to a CSV file.  All the invoice numbers, dates, order numbers, customer IDs, and totals from the invoices in the folder are saved in convenient CSV format.

## ***ExtractTextPreservingStyleAndPositionInfo***
Extracts text along with details about the text such as quads, font, font-size, color, and styles from a PDF document and saves the text to a JSON file.

## ***ListWords***
Extracts text from a PDF document and displays the extracted words.

## ***RegexExtractText***
Uses a regular expression to find a specified phrase of text in a PDF document.

## ***RegexTextSearch***
Searches for phrases or text patterns in a PDF input document. It supplies sample regular expressions to use in searching for phone numbers, email addresses, or URLs, and you can use them or create your own. You can search the entire PDF document or provide a page range for your search. The program generates an output PDF document that matches the input file except that the search content appears highlighted.  You can enter the name of the input file you plan to use, and the name of the output file. The sample uses PDDocTextFinder to find instances of a phrase or pattern in a PDF input document.

The sample normally highlights search text with a box that surrounds the entire phrase found. But if the search text is on multiple lines, or if the font changes within the phrase, the content appears in multiple boxes.

## ***TextExtract***
Extracts text from a tagged and untagged PDF document.
