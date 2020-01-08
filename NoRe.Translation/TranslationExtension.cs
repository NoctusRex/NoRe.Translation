using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoRe.Translation
{
    public static class TranslationExtension
    {
        public static string Translate(this string text)
        {
            return Translator.Translate(text);
        }
    }
}
