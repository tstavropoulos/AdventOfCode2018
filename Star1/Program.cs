using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Star1
{
    class Program
    {
        static void Main(string[] args)
        {
            long value = 0;
            using (FileStream stream = File.OpenRead("..\\..\\..\\input1.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    value += long.Parse(line);
                }
            }

            Console.WriteLine(value);
            Console.ReadKey();
        }
    }
}
