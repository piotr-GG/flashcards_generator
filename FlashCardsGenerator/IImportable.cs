using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardsGenerator
{
    public interface IImportable
    {
        public FlashcardItem FromCSV(string csv_row, string separator);
    }
}
