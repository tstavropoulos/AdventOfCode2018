using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Day01
{
    class Program
    {
        private const string inputFile = "..\\..\\..\\input1.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Day 1");
            Console.WriteLine("Star 1");
            Console.WriteLine("");

            List<long> values = File.ReadAllLines(inputFile).Select(long.Parse).ToList();

            long cumulativeValue = values.Sum();

            Console.WriteLine($"The cumulative value: {cumulativeValue}");
            Console.WriteLine("");


            Console.WriteLine("Star 2");
            Console.WriteLine("");

            long cyclicalValue = 0;
            HashSet<long> visited = new HashSet<long>();


            bool done = false;

            while (!done)
            {
                foreach (long adj in values)
                {
                    cyclicalValue += adj;
                    if (!visited.Add(cyclicalValue))
                    {
                        done = true;
                        break;
                    }
                }
            }

            Console.WriteLine($"The first repeated value: {cyclicalValue}");
            Console.WriteLine("");
            Console.ReadKey();
        }
    }
}
