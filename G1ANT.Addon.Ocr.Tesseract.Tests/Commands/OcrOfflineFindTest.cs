/**
*    Copyright(C) G1ANT Ltd, All rights reserved
*    Solution G1ANT.Addon, Project G1ANT.Addon.OCR.Tesseract
*    www.g1ant.com
*
*    Licensed under the G1ANT license.
*    See License.txt file in the project root for full license information.
*
*/
using G1ANT.Addon.Ocr.Tesseract.Tests.Properties;
using G1ANT.Engine;
using G1ANT.Language;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Threading;

namespace G1ANT.Addon.Ocr.Tesseract.Tests
{
    [TestFixture]
    public class OcrOfflineFindTest
    {
        private Scripter scripter;
        private Process proces;
        private static string filePath = string.Empty;

        [OneTimeSetUp]
        public static void OcrOfflineTestsInitialize()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            filePath = Assembly.GetExecutingAssembly().UnpackResourceToFile("Resources." + nameof(Resources.testimageDance), "png");
        }

        [SetUp]
        public void TestInit()
        {
            Language.Addon addon = Language.Addon.Load(@"G1ANT.Addon.Ocr.Tesseract.dll");
            scripter = new Scripter();
scripter.InitVariables.Clear();
            var processInfo = new ProcessStartInfo("mspaint.exe", $"\"{filePath}\"");
            processInfo.WindowStyle = ProcessWindowStyle.Maximized;
            proces = Process.Start(processInfo);
            proces.WaitForInputIdle();
            Thread.Sleep(3000);

        }

        [Test]
        [Timeout(30000)]
        public void OcrOfflineFindCommandTest()
        {
            Rectangle expectedRectangle = new Rectangle(51, 91, 161, 43);

            //            window {SpecialChars.Search}Paint{SpecialChars.Search} style maximize
            string script = $@"
            ocroffline.find search {SpecialChars.Text}Dance{SpecialChars.Text} area (rectangle)68{SpecialChars.Point}162{SpecialChars.Point}767{SpecialChars.Point}528
            ";
            scripter.Text = script;
            scripter.Run();
            var resultRectangle = scripter.Variables.GetVariableValue<Rectangle>("result");
            Assert.IsTrue(AreRectanglesSimilar(expectedRectangle, resultRectangle, 10));
        }

        [TearDown]
        public void TestCleanup()
        {
            if (proces != null && !proces.HasExited)
            {
                proces.Kill();
            }
        }

        private bool AreRectanglesSimilar(Rectangle r1, Rectangle r2, int tolerance)
        {
            if (Math.Abs(r1.X - r2.X) > tolerance)
            {
                return false;
            }
            if (Math.Abs(r1.Y - r2.Y) > tolerance)
            {
                return false;
            }
            if (Math.Abs(r1.Width - r2.Width) > tolerance)
            {
                return false;
            }
            if (Math.Abs(r1.Height - r2.Height) > tolerance)
            {
                return false;
            }
            return true;
        }
    }
}


