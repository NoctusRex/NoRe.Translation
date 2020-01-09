using NoRe.Core;

namespace NoRe.Translation
{
    /// <summary>
    /// Contains all configurations of the Translator
    /// </summary>
    public class TranslatorConfiguration : Configuration
    {
        /// <summary>
        /// The target language the text will be translated to
        /// All text can be used as language
        /// Use 'textInDefaultLanguage' to get the original text and do no translation
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// Loads all translations into ram for faster access
        /// </summary>
        public bool LoadToRam { get; set; }
        /// <summary>
        /// Adds untranslated text to the database for later translation
        /// </summary>
        public bool CollectMissingTranslation { get; set; }

        public TranslatorConfiguration() : base(System.IO.Path.Combine(Pathmanager.ConfigurationDirectory, "TranslatorConfiguration.xml")) { }

        public override void Read()
        {
            TranslatorConfiguration temp = Read<TranslatorConfiguration>();

            Language = temp.Language;
            LoadToRam = temp.LoadToRam;
            CollectMissingTranslation = temp.CollectMissingTranslation;
        }
    }
}
