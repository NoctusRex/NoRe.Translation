using NoRe.Core;
using NoRe.Database.SqLite;
using NoRe.Database.Core.Models;
using System;

namespace NoRe.Translation
{
    internal static class DatabaseQueries
    {
        private static string TranslatorConfigurationPath { get; } = System.IO.Path.Combine(Pathmanager.ConfigurationDirectory, "TranslatorSqLiteConfiguration.xml");

        public static void CreateTable()
        {
            using (SqLiteWrapper wrapper = new SqLiteWrapper(TranslatorConfigurationPath))
            {
                wrapper.ExecuteNonQuery("CREATE TABLE translations (id INTEGER PRIMARY KEY NOT NULL, textInDefaultLanguage TEXT NOT NULL)");
            }
        }

        public static void AddLanguage(string language)
        {
            using (SqLiteWrapper wrapper = new SqLiteWrapper(TranslatorConfigurationPath))
            {
                wrapper.ExecuteNonQuery($"ALTER TABLE translations ADD {language} TEXT");
            }
        }

        public static bool ExistsLanguage(string language)
        {
            using (SqLiteWrapper wrapper = new SqLiteWrapper(TranslatorConfigurationPath))
            {
                return wrapper.ExecuteScalar<Int64>("SELECT COUNT(*) AS CNTREC FROM pragma_table_info(@0) WHERE name = @1", "translations", language) > 0;
            }
        }

        public static bool ExistsTable()
        {
            using (SqLiteWrapper wrapper = new SqLiteWrapper(TranslatorConfigurationPath))
            {
                return wrapper.ExecuteScalar<Int64>("SELECT COUNT(name) FROM sqlite_master WHERE type=@0 AND name=@1", "table", "translations") > 0;
            }
        }

        public static bool ExistsText(string text)
        {
            using (SqLiteWrapper wrapper = new SqLiteWrapper(TranslatorConfigurationPath))
            {
                return wrapper.ExecuteScalar<Int64>("SELECT COUNT(textInDefaultLanguage) FROM translations WHERE textInDefaultLanguage=@0", text) > 0;
            }
        }

        public static bool TryGetTranslation(string text, string language, out string translation)
        {
            translation = text;

            using (SqLiteWrapper wrapper = new SqLiteWrapper(TranslatorConfigurationPath))
            {
                Table Table = wrapper.ExecuteReader("SELECT * FROM translations WHERE textInDefaultLanguage = @0", text);

                if (Table.Rows.Count == 0) return false;
                if (string.IsNullOrEmpty(Table.GetValue<string>(0, language))) return false;

                translation = Table.GetValue<string>(0, language);
                return true;
            }

        }

        public static void AddUntranslatedText(string text)
        {
            using (SqLiteWrapper wrapper = new SqLiteWrapper(TranslatorConfigurationPath))
            {
                wrapper.ExecuteNonQuery("INSERT INTO translations (textInDefaultLanguage) VALUES(@0)", text);
            }
        }

    }
}
