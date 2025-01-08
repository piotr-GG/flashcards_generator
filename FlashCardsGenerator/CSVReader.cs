using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace FlashCardsGenerator
{
    internal class CSVReader
    {
        public static List<string> ReadFromCSV(string filePath)
        {
            List<string> result = new List<string>();
            using (var reader = new StreamReader(filePath, encoding: Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    result.Add(line);
                }
            }
            return result;
        }
    }
}
