using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardsGenerator
{
    internal class FlashcardFactory
    {
        public static FlashcardItem FromCSV(string csv_row, string separator)
        {
            List<string> result_array = csv_row.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();
            return new FlashcardItem(result_array[0], result_array[1], result_array[2], result_array[3]);
        }
    }
}
