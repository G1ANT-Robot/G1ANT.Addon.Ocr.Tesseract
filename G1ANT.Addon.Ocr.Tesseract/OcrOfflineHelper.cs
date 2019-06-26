/**
*    Copyright(C) G1ANT Ltd, All rights reserved
*    Solution G1ANT.Addon, Project G1ANT.Addon.OCR.Tesseract
*    www.g1ant.com
*
*    Licensed under the G1ANT license.
*    See License.txt file in the project root for full license information.
*
*/
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace G1ANT.Addon.Ocr.Tesseract
{
    public class OcrOfflineHelper
    {
        public static Bitmap RescaleImage(Bitmap partOfScreen, double v)
        {
            var width = (int)Math.Round(partOfScreen.Width * v, 0);
            var height = (int)Math.Round(partOfScreen.Height * v, 0);
            Bitmap bmp = new Bitmap(width, height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.DrawImage(partOfScreen, new Rectangle(0, 0, bmp.Width, bmp.Height), new Rectangle(0, 0, partOfScreen.Width, partOfScreen.Height), GraphicsUnit.Pixel);
            }
            return bmp;
        }
        public static string SaveImageToTemporaryFolder(Bitmap bmp)
        {
            string dirPath = Path.Combine(Path.GetTempPath(),
                                    @"G1ANT.Robot\OcrTempFolder\");
            Directory.CreateDirectory(dirPath);
            string filename = Guid.NewGuid().ToString() + ".bmp";
            bmp.Save(dirPath + filename, System.Drawing.Imaging.ImageFormat.Bmp);

            return dirPath + filename;
        }
        public static string GetResourcesFolder(string language)
        {
            string dirPath = Path.Combine(Path.GetTempPath(),
                                 @"G1ANT.Robot\OcrTempFolder\tessdata\");

            if (!Directory.Exists(dirPath))
            {
                var Str = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
                var filesToCopy = Str.Where(x => x.Contains(language)).ToList();
                string defaultNamespace = System.Reflection.Assembly.GetExecutingAssembly().FullName.Split(',')[0];
                Directory.CreateDirectory(dirPath);

                foreach (var langFile in filesToCopy)
                {

                    using (var resource = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(langFile))
                    {
                        var fileName = langFile.Replace(defaultNamespace + ".Resources.tessdata.", "");
                        using (var file = new FileStream(dirPath + fileName, FileMode.Create, FileAccess.Write))
                        {
                            resource.CopyTo(file);
                        }
                    }
                }
            }
            return dirPath;
        }
        public static void UnpackNeededAssemblies()
        {
            UnpackNeededAssemblies("x86");
            UnpackNeededAssemblies("x64");
            UnpackTesseract();
        }

        private static void UnpackNeededAssemblies(string version)
        {
            var executingPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var dirPath = executingPath + $@"\{version}\";
            Directory.CreateDirectory(dirPath);
            var l = Directory.EnumerateFiles(dirPath);
            var needCopy = (l.Where(x => x.Contains("libtesseract")).SingleOrDefault() == null) || l.Where(x => x.Contains("liblept")).SingleOrDefault() == null;

            if (needCopy)
            {
                var Str = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
                var filesToCopy = Str.Where(x => x.Contains($"{version}dlls")).ToList();
                string defaultNamespace = System.Reflection.Assembly.GetExecutingAssembly().FullName.Split(',')[0];
                foreach (var langFile in filesToCopy)
                {

                    using (var resource = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(langFile))
                    {
                        var fileName = langFile.Replace(defaultNamespace + $".Resources.{version}dlls.", "");
                        using (var file = new FileStream(dirPath + fileName, FileMode.Create, FileAccess.Write))
                        {
                            resource.CopyTo(file);
                        }
                    }
                }
            }
        }

        private static void UnpackTesseract()
        {
            var executingPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var fileName = "Tesseract.dll";
            var fullPath = Path.Combine(executingPath, fileName);
            var resourceName = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(r => r.Contains(fileName)).FirstOrDefault();
            if (!File.Exists(fullPath))
            {
                using (var resource = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                using (var file = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    resource.CopyTo(file);
                }
            }
        }
    }
}