namespace NoRe.Translation
{
    /// <summary>
    /// Contains extensions for faster translation access
    /// </summary>
    public static class TranslationExtension
    {

        /// <summary>
        /// Translates a string
        /// </summary>
        /// <param name="value">the original value</param>
        /// <returns>the translation in the language specified in the configuration</returns>
        public static string Translate(this string value)
        {
            return Translator.Translate(value);
        }

        /// <summary>
        /// Translates a string
        /// </summary>
        /// <param name="value">the original value</param>
        /// <returns>the translation in the language specified in the configuration</returns>
        public static string Translate(this string value, string targetLanguage)
        {
            return Translator.Translate(value, targetLanguage);
        }

        /// <summary>
        /// Translates any object by translating the result of the ToString() function
        /// </summary>
        /// <param name="value">the object to translate</param>
        /// <returns>the translation</returns>
        public static string Translate(this object value)
        {
            return Translator.Translate(value.ToString());
        }

        /// <summary>
        /// Translates any object by translating the result of the ToString() function
        /// </summary>
        /// <param name="value">the object to translate</param>
        /// <returns>the translation</returns>
        public static string Translate(this object value, string targetLanguage)
        {
            return Translator.Translate(value.ToString(), targetLanguage);
        }

    }
}
