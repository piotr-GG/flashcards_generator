using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardsGenerator
{
    internal class CSVWriter
    {
        public static void WriteToCSV<T>(string filePath, ObservableCollection<T> data, string separator = ";") where T:IExportable
        {
            List<string> outputArray = [];
            foreach (T item in data)
            {
                outputArray.Add(item.ToCSV(separator));
            }

            using(var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                foreach (string row in outputArray)
                {
                    writer.WriteLine(row);
                }
            }
       
        }
    }
}
