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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Tesseract;

namespace G1ANT.Addon.Ocr.Tesseract
{
    [Command(Name = "ocrtesseract.find", Tooltip = "This command allows to find the text on the active screen and return it's position as a 'rectangle' format. \nIf the text will not be found, the result will be Rectangle(-1,-1,-2,-2).  \nPlease be aware that using this command result in unpacking the necessary data to directory My Documents/G1ANT.Robot.")]
    public class OcrOfflineFindCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Text to be found on the screen (the fewer words, the better results)")]
            public TextStructure Search { get; set; }

            [Argument]
            public RectangleStructure Area { get; set; } = new RectangleStructure(SystemInformation.VirtualScreen);

            [Argument]
            public BooleanStructure Relative { get; set; } = new BooleanStructure(true);

            [Argument]
            public VariableStructure Result { get; set; } = new VariableStructure("result");

            [Argument(Tooltip = "Language to be used for text recognition")]
            public TextStructure Language { get; set; } = new TextStructure("eng");

            [Argument(Tooltip = "Factor of image zoom that allows better recognition of smaller text")]
            public FloatStructure Sensitivity { get; set; } = new FloatStructure(2.0);
        }
        public OcrOfflineFindCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            var rectangle = arguments.Area.Value;
            if (!rectangle.IsValidRectangle())
                throw new ArgumentException("Argument Area is not a valid rectangle");
            if (arguments.Relative.Value)
            {
                var foregroundWindowRect = new RobotWin32.Rect();
                RobotWin32.GetWindowRectangle(RobotWin32.GetForegroundWindow(), ref foregroundWindowRect);
                rectangle = new Rectangle(rectangle.X + foregroundWindowRect.Left,
                    rectangle.Y + foregroundWindowRect.Top,
                    Math.Min(rectangle.Width, foregroundWindowRect.Right - foregroundWindowRect.Left - rectangle.X),
                    Math.Min(rectangle.Height, foregroundWindowRect.Bottom - foregroundWindowRect.Top - rectangle.Y));
            }
            var partOfScreen = RobotWin32.GetPartOfScreen(rectangle);
            var language = arguments.Language.Value;
            var imgToParse = OcrOfflineHelper.RescaleImage(partOfScreen, arguments.Sensitivity.Value);
            var search = arguments.Search.Value.ToLower().Trim();
            var dataPath = OcrOfflineHelper.GetResourcesFolder(language);

            try
            {
                using (var tEngine = new TesseractEngine(dataPath, language, EngineMode.TesseractAndCube))
                using (var img = PixConverter.ToPix(imgToParse))
                using (var page = tEngine.Process(img))
                {
                    var rectResult = new Rectangle(-1, -1, -1, -1);
                    var rectanglesWithWords = GetWords(page.GetHOCRText(0), arguments.Sensitivity.Value);
                    var searchWords = search.Split(' ');
                    List<Rectangle> rectangleList = new List<Rectangle>();
                    if (searchWords.Length > 1)
                    {
                        rectangleList = rectanglesWithWords.Where(x => searchWords.Contains(x.Value)).Select(a => a.Key).ToList();
                        if (rectangleList.Count() == searchWords.Length)
                        {
                            rectResult = UniteRectangles(rectangleList);
                        }
                    }
                    else if (rectanglesWithWords.ContainsValue(search))
                    {
                        rectResult = rectanglesWithWords.Where(x => x.Value == search).First().Key;
                    }
                    if (Equals(rectResult, new Rectangle(-1, -1, -1, -1)) || (searchWords.Length > 1 && rectangleList.Count() != searchWords.Length))
                        throw new NullReferenceException("Ocr was unable to find text");
                    Scripter.Variables.SetVariableValue(arguments.Result.Value, new RectangleStructure(rectResult));
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

        private Rectangle UniteRectangles(IEnumerable<Rectangle> rectangleList)
        {
            int xMin = rectangleList.Min(s => s.X);
            int yMin = rectangleList.Min(s => s.Y);
            int xMax = rectangleList.Max(s => s.X + s.Width);
            int yMax = rectangleList.Max(s => s.Y + s.Height);
            return new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        public Dictionary<Rectangle, string> GetWords(string tesseractHtml, double imgRescaleRatio)
        {
            var xml = XDocument.Parse(tesseractHtml);

            var rectsWords = new Dictionary<Rectangle, string>();
            var Ocr_words = xml.Descendants("span").ToList().Where(element => element.Attribute("class").Value == "ocrx_word").ToList();
            foreach (var Ocr_word in Ocr_words)
            {
                var strs = Ocr_word.Attribute("title").Value.Split(' ');
                strs[4] = strs[4].Replace(";", "");
                int left = (int)(double.Parse(strs[1]) / imgRescaleRatio);
                int top = (int)(double.Parse(strs[2]) / imgRescaleRatio);
                int width = (int)((double.Parse(strs[3]) - double.Parse(strs[1]) + 1) / imgRescaleRatio);
                int height = (int)((double.Parse(strs[4]) - double.Parse(strs[2]) + 1) / imgRescaleRatio);
                var rectange = new Rectangle(left, top, width, height);
                if (!rectsWords.ContainsKey(rectange))
                    rectsWords.Add(new Rectangle(left, top, width, height), Ocr_word.Value.ToLower().Trim());
            }

            return rectsWords;
        }

    }
}
