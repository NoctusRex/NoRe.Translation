using NoRe.Core;
using NoRe.Database.Core.Models;
using NoRe.Database.SqLite;
using System;
using System.Collections.Generic;

namespace NoRe.Translation
{
    /// <summary>
    /// Contains all database statements used to manage the translations
    /// </summary>
    internal static class DatabaseQueries
    {
        /// <summary>
        /// The path where the database file is stored - TODO: Add to configuration
        /// </summary>
        private static string DatabasePath { get; } = System.IO.Path.Combine(Pathmanager.ConfigurationDirectory, "TranslatorSqLiteConfiguration.xml");

        /// <summary>
        /// Creates the table in the database where the translations will be stored
        /// </summary>
        public static void CreateTable()
        {
            using (SqLiteWrapper wrapper = new SqLiteWrapper(DatabasePath))
            {
                wrapper.ExecuteNonQuery("CREATE TABLE translations (id INTEGER PRIMARY KEY NOT NULL, textInDefaultLanguage TEXT NOT NULL)");
            }
        }

        /// <summary>
        /// Alters the translation table and adds a new column to save translations for the specified language
        /// </summary>
        /// <param name="language">Will be added as new column</param>
        public static void AddLanguage(string language)
        {
            using (SqLiteWrapper wrapper = new SqLiteWrapper(DatabasePath))
            {
                wrapper.ExecuteNonQuery($"ALTER TABLE translations ADD {language} TEXT");
            }
        }

        /// <summary>
        /// Checks if a language is already added as a column in the table
        /// </summary>
        /// <param name="language">the language to check</param>
        /// <returns>true if exists</returns>
        public static bool ExistsLanguage(string language)
        {
            using (SqLiteWrapper wrapper = new SqLiteWrapper(DatabasePath))
            {
                return wrapper.ExecuteScalar<Int64>("SELECT COUNT(*) AS CNTREC FROM pragma_table_info(@0) WHERE name = @1", "translations", language) > 0;
            }
        }

        /// <summary>
        /// Checks if the translations table already exists
        /// </summary>
        /// <returns>true if exists</returns>
        public static bool ExistsTable()
        {
            using (SqLiteWrapper wrapper = new SqLiteWrapper(DatabasePath))
            {
                return wrapper.ExecuteScalar<Int64>("SELECT COUNT(name) FROM sqlite_master WHERE type=@0 AND name=@1", "table", "translations") > 0;
            }
        }

        /// <summary>
        /// Checks if a default translation already exists
        /// </summary>
        /// <param name="text">text to check</param>
        /// <returns>true if exists</returns>
        public static bool ExistsText(string text)
        {
            using (SqLiteWrapper wrapper = new SqLiteWrapper(DatabasePath))
            {
                return wrapper.ExecuteScalar<Int64>("SELECT COUNT(textInDefaultLanguage) FROM translations WHERE textInDefaultLanguage=@0", text) > 0;
            }
        }

        /// <summary>
        /// Tries to get the translation of the text in the specified language
        /// </summary>
        /// <param name="text">the text that is meant to be translated</param>
        /// <param name="language">the target language</param>
        /// <param name="translation">the translation of the text, returns the text if it could not be translated</param>
        /// <returns>true if the text was translated</returns>
        public static bool TryGetTranslation(string text, string language, out string translation)
        {
            translation = text;

            using (SqLiteWrapper wrapper = new SqLiteWrapper(DatabasePath))
            {
                Table Table = wrapper.ExecuteReader("SELECT * FROM translations WHERE textInDefaultLanguage = @0", text);

                if (Table.Rows.Count == 0) return false;
                if (string.IsNullOrEmpty(Table.GetValue<string>(0, language))) return false;

                translation = Table.GetValue<string>(0, language);
                return true;
            }

        }

        /// <summary>
        /// Returns all text and its translation in the target language
        /// </summary>
        /// <param name="targetLanguage">the translations in the specified language</param>
        /// <returns>a dictionary with the original text as key and the translation as value, string is empty of the translation is missing</returns>
        public static Dictionary<string, string> GetTranslations(string targetLanguage)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            using (SqLiteWrapper wrapper = new SqLiteWrapper(DatabasePath))
            {
                wrapper.ExecuteReader("SELECT * FROM translations").Rows.ForEach(row => 
                {
                    result.Add(row.GetValue<string>("textInDefaultLanguage"), TryGetString(row.GetValue<object>(targetLanguage)));
                });
            }

            return result;
        }

        /// <summary>
        /// Adds a not translated text to the translations table
        /// </summary>
        /// <param name="text">The text that will be added into 'textInDefaultLanguage' column</param>
        public static void AddUntranslatedText(string text)
        {
            using (SqLiteWrapper wrapper = new SqLiteWrapper(DatabasePath))
            {
                wrapper.ExecuteNonQuery("INSERT INTO translations (textInDefaultLanguage) VALUES(@0)", text);
            }
        }

        /// <summary>
        /// Tries to convert an object to string and returns emtpy string of not possible
        /// used for DBNull strings
        /// </summary>
        /// <param name="value">the value to be converted</param>
        /// <returns></returns>
        private static string TryGetString(object value)
        {
            try
            {
                return value.ToString();
            }
            catch
            {
                return "";
            }
        }

    }
}
