using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NoRe.Translation
{
    public static class Translator
    {
        private static TranslatorConfiguration TranslatorConfiguration { get; set; } = null;

        /// <summary>
        /// Returns a copy of the current configuration without object reference so it can not be changed
        /// </summary>
        public static TranslatorConfiguration Configuration
        {
            get
            {
                if (TranslatorConfiguration is null) return null;

                return new TranslatorConfiguration()
                {
                    Language = TranslatorConfiguration.Language,
                    Path = TranslatorConfiguration.Path
                };
            }
        }

        public static string Translate(string text)
        {
            if (Configuration is null) Initialize();

            if (DatabaseQueries.TryGetTranslation(text, Configuration.Language, out string translation)) return translation;

            if (!DatabaseQueries.ExistsLanguage(Configuration.Language)) DatabaseQueries.AddLanguage(Configuration.Language);

            if (!DatabaseQueries.ExistsText(text)) DatabaseQueries.AddUntranslatedText(text);

            return text;
        }

        private static void Initialize()
        {
            try
            {
                TranslatorConfiguration = new TranslatorConfiguration();
                TranslatorConfiguration.Read();

                if (!DatabaseQueries.ExistsTable()) DatabaseQueries.CreateTable();
            }
            catch (Exception ex)
            {
                TranslatorConfiguration = null;
                throw ex;
            }

        }

    }
}
