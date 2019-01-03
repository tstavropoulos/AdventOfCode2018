using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Star2
{
    class Program
    {
        private const string inputFile = "..\\..\\..\\input1.txt";

        static void Main(string[] args)
        {
            long value = 0;
            HashSet<long> visited = new HashSet<long>();

            List<long> values = File.ReadAllLines(inputFile).Select(long.Parse).ToList();

            bool done = false;

            while(!done)
            {
                foreach(long adj in values)
                {
                    value += adj;
                    if (!visited.Add(value))
                    {
                        done = true;
                        break;
                    }
                }
            }

            Console.WriteLine(value);
            Console.ReadKey();
        }
    }
}
