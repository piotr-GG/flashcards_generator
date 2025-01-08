using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardsGenerator
{
    public interface IExportable
    {
        public string ToCSV(string separator);
    }
}
