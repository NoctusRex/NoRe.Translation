using NoRe.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoRe.Translation
{
    public class TranslatorConfiguration : Configuration
    {
        public string Language { get; set; }

        public TranslatorConfiguration() : base(System.IO.Path.Combine(Pathmanager.ConfigurationDirectory, "TranslatorConfiguration.xml")) { }

        public override void Read()
        {
            TranslatorConfiguration temp = Read<TranslatorConfiguration>();

            Language = temp.Language;
        }
    }
}
