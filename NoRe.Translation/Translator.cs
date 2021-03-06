﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace NoRe.Translation
{
    /// <summary>
    /// Translates text
    /// Use the .Translate() extension for faster access
    /// </summary>
    public static class Translator
    {
        private static TranslatorConfiguration Configuration { get; set; } = null;

        /// <summary>
        /// Contains all translations of the configuration language if loaded into ram is true
        /// </summary>
        private static Dictionary<string, string> Translations { get; set; }

        /// <summary>
        /// Translates a string and returns the original value when no translation is found
        /// </summary>
        /// <param name="text">the text to translate</param>
        /// <returns>the translation</returns>
        public static string Translate(string text)
        {
            if (Configuration is null) Initialize();

            if (TryGetTranslation(text, out string translation)) return translation;

            if (!Configuration.CollectMissingTranslation) return text;
            if (!DatabaseQueries.ExistsText(text)) DatabaseQueries.AddUntranslatedText(text);

            return text;
        }

        /// <summary>
        /// Translates a string and returns the original value when no translation is found
        /// </summary>
        /// <param name="text">the text to translate</param>
        /// <param name="targetLanguage">the target language</param>
        /// <returns>the translation</returns>
        public static string Translate(string text, string targetLanguage)
        {
            string oldLanguage = Configuration.Language;

            try
            {
                SetLanguage(targetLanguage, false);
                return Translate(text);
            }
            finally
            {
                Configuration.Language = oldLanguage;
            }
        }

        /// <summary>
        /// Loads the configuration, creates neccessary tables and loads translations into ram
        /// </summary>
        public static void Initialize(string translatorConfigurationPath = "", string databaseConfigurationPath = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(databaseConfigurationPath)) DatabaseQueries.DatabaseConfigurationPath = databaseConfigurationPath;

                if (string.IsNullOrEmpty(translatorConfigurationPath))
                {
                    Configuration = new TranslatorConfiguration();
                }
                else
                {
                    Configuration = new TranslatorConfiguration(translatorConfigurationPath);
                }
                Configuration.Read();

                if (!DatabaseQueries.ExistsTable()) DatabaseQueries.CreateTable();
                SetLanguage(Configuration.Language, false);
            }
            catch (Exception ex)
            {
                Configuration = null;
                throw ex;
            }

        }

        /// <summary>
        /// Changes the currently selected language
        /// </summary>
        /// <param name="language"></param>
        /// <param name="doWrite">Changes the config file if true</param>
        public static void SetLanguage(string language, bool doWrite = true)
        {
            Configuration.Language = language;
            if (doWrite) Configuration.Write();

            if (!DatabaseQueries.ExistsLanguage(language)) DatabaseQueries.AddLanguage(language);
            if (Configuration.LoadToRam) Translations = DatabaseQueries.GetTranslations(language);
        }

        /// <summary>
        /// Tries to get a translation and returns the original value if no translation is found
        /// Method of acquiring depends on configuration
        /// Loads from ram or database
        /// </summary>
        /// <param name="text">the text to translate</param>
        /// <param name="translation">the translated text</param>
        /// <returns>true if translation is successful</returns>
        private static bool TryGetTranslation(string text, out string translation)
        {
            if (Configuration.LoadToRam)
            {
                translation = Translations.FirstOrDefault(x => x.Key == text).Value;
                if (string.IsNullOrEmpty(translation)) { translation = text; return false; }

                return true;
            }
            else
            {
                return DatabaseQueries.TryGetTranslation(text, Configuration.Language, out translation);
            }
        }

    }
}
