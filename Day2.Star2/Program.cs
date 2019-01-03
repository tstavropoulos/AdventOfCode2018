using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day2.Star2
{
    class Program
    {
        private const string inputFile = "..\\..\\..\\input2.txt";

        static void Main(string[] args)
        {
            List<string> values = File.ReadAllLines(inputFile).ToList();

            Console.WriteLine("Day 2 - Star 2");
            Console.WriteLine("");
            for (int i = 0; i < values.Count - 1; i++)
            {
                for (int j = i + 1; j < values.Count; j++)
                {
                    string idA = values[i];
                    string idB = values[j];

                    if (CompareStrings(idA, idB) != null)
                    {
                        Console.WriteLine(CompareStrings(idA, idB));
                    }
                }
            }

            Console.ReadKey();
        }

        static char[] CompareStrings(string idA, string idB)
        {
            int errorCount = 0;
            char[] output = new char[idA.Length - 1];
            for (int i = 0; i < idA.Length; i++)
            {
                if (idA[i] != idB[i])
                {
                    if (++errorCount >= 2)
                    {
                        return null;
                    }
                }
                else
                {
                    output[i - errorCount] = idA[i];
                }
            }


            return output;
        }
    }
}
