/**
*    Copyright(C) G1ANT Ltd, All rights reserved
*    Solution G1ANT.Addon, Project G1ANT.Addon.OCR.Tesseract
*    www.g1ant.com
*
*    Licensed under the G1ANT license.
*    See License.txt file in the project root for full license information.
*
*/
using NUnit.Framework;

namespace G1ANT.Addon.Ocr.Tesseract.Tests
{
    [TestFixture]
    public class LoadTesseractTests
    {
        [Test]
        public void LoadTesseractAddon()
        {
            Language.Addon addon = Language.Addon.Load(@"G1ANT.Addon.Ocr.Tesseract.dll");
            Assert.IsNotNull(addon);
            Assert.IsTrue(addon.Commands.Count > 0);
        }
    }
}
