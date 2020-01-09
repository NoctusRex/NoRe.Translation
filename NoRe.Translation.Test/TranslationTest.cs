using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NoRe.Translation.Test
{
    [TestClass]
    public class TranslationTest
    {
        [TestMethod]
        public void TestNewWord()
        {
            // Test if new words with out translations are returned as inputted
            string newText = DateTime.Now.Ticks.ToString();
            Assert.AreEqual(newText, Translator.Translate(newText));
        }

        [TestMethod]
        public void TestExistingWord()
        {
            // Test if a word gets translated right
            Assert.AreEqual("Auto", Translator.Translate("Car"));
            Assert.AreEqual("Auto", "Car".Translate());
            Assert.AreEqual("Fünf", 5.Translate());
        }
    }
}
