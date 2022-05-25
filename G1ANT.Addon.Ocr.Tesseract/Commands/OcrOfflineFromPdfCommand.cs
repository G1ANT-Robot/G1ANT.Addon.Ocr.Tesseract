/**
*    Copyright(C) G1ANT Ltd, All rights reserved
*    Solution G1ANT.Addon, Project G1ANT.Addon.OCR.Tesseract
*    www.g1ant.com
*
*    Licensed under the G1ANT license.
*    See License.txt file in the project root for full license information.
*
*/
using G1ANT.Language;
using System;
using System.IO;
using System.Drawing;
using System.Windows;
using Tesseract;

namespace G1ANT.Addon.Ocr.Tesseract
{
    [Command(Name = "ocrtesseract.frompdf", Tooltip = "This command allows to capture part of the PDF file and recognize text from it. \nThis command may often be less accurate than 'ocr' command. \nPlease be aware that command will unpack some necessary data to folder My Documents/G1ANT.Robot.")]
    public class OcrOfflineFromPdfCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Tooltip = "Area to OCR, example 0⫽0⫽652⫽138, default value: full image size.")]
            public RectangleStructure Area { get; set; }

            [Argument]
            public VariableStructure Result { get; set; } = new VariableStructure("result");

            [Argument(Tooltip = "Language to be used for text recognition")]
            public TextStructure Language { get; set; } = new TextStructure("eng");

            [Argument(Tooltip = "Factor of image zoom that allows better recognition of smaller text")]
            public FloatStructure Sensitivity { get; set; } = new FloatStructure(2.0);

            [Argument(Tooltip = "Name of PDF file to recognise")]
            public PathStructure PdfFileName { get; set; }

            [Argument(Tooltip = "Number of page to recognise (1..), default: 1")]
            public IntegerStructure PageNumber { get; set; } = new IntegerStructure(1);
        }

        public OcrOfflineFromPdfCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            if (!File.Exists(arguments.PdfFileName.Value))
                throw new ArgumentException($"PDF file {arguments.PdfFileName.Value} doesn't exists.");


            var loadedImage = new Bitmap(arguments.PdfFileName.Value);
            var rectangle = new Rectangle(new Point(0, 0), loadedImage.Size);

            if (arguments.Area != null)
            {
                if (rectangle.Contains(arguments.Area.Value))
                    rectangle = arguments.Area.Value;
                else
                    throw new ArgumentException("Image doesn't contains area argument");
            }

            if (!rectangle.IsValidRectangle())
                throw new ArgumentException("Argument Area is not a valid rectangle");

            var imageToParse = OcrOfflineHelper.RescaleImage(loadedImage, arguments.Sensitivity.Value);
            var language = arguments.Language.Value;
            var dataPath = OcrOfflineHelper.GetResourcesFolder(language);

            try
            {
                using (var engine = new TesseractEngine(dataPath, language, EngineMode.TesseractAndCube))
                using (var image = PixConverter.ToPix(imageToParse))
                using (var page = engine.Process(image))
                {
                    var text = page.GetText();
                    if (string.IsNullOrEmpty(text))
                        throw new NullReferenceException("Ocr was unable to find any text");
                    Scripter.Variables.SetVariableValue(arguments.Result.Value, new TextStructure(text));
                }
            }
            catch (TesseractException)
            {
                throw new ApplicationException("Ocr engine exception, possibly missing language data in folder: " + dataPath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

