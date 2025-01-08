using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardsGenerator
{
    public class FlashcardItem: IExportable
    {
        public string Original { get; set; }
        public string Translated { get; set; }
        public string Example { get; set; }
        public string ExampleTranslated { get; set; }

        public FlashcardItem(string original, string translated, string example, string exampleTranslated)
        {
            Original = original;
            Translated = translated;
            Example = example;
            ExampleTranslated = exampleTranslated;
        }   

        public string ToCSV(string separator)
        {
            return string.Join(separator, [this.Original, this.Translated, this.Example, this.ExampleTranslated]);
        }
    }
}
