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
using System.IO;
using System.Reflection;
using System.Threading;

namespace G1ANT.Addon.Ocr.Tesseract.Tests
{
    [TestFixture]
    public class OcrOfflineFromScreenTests
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
        public void OcrFromScreenInitialize()
        {
            Language.Addon addon = Language.Addon.Load(@"G1ANT.Addon.Ocr.Tesseract.dll");
            scripter = new Scripter();
scripter.InitVariables.Clear();
            proces = Process.Start("mspaint.exe", $"\"{filePath}\"");
            proces.WaitForInputIdle();
            Thread.Sleep(3000);

        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [Timeout(12000)]
        public void OcfOfflineFromScreenCommandTest()
        {
            // window {SpecialChars.Search}Paint{SpecialChars.Search} style maximize
            string script = $@"
            ocroffline.fromscreen area (rectangle)68{SpecialChars.Point}162{SpecialChars.Point}767{SpecialChars.Point}528
            ";
            scripter.Text = script;
            scripter.Run();
            var resultOutput = scripter.Variables.GetVariableValue<string>("result");
            Assert.IsTrue(resultOutput.Contains("Dance"));
            Assert.IsTrue(resultOutput.Contains("till"));
            Assert.IsTrue(resultOutput.Contains("the"));
            Assert.IsTrue(resultOutput.Contains("DAWN"));
        }

        [TearDown]
        public void OcrFromScreenCleanup()
        {
            if (proces != null && !proces.HasExited)
            {
                proces.Kill();
            }
        }

    }
}