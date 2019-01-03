using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day2.Star1
{
    class Program
    {
        private const string inputFile = "..\\..\\..\\input2.txt";

        static void Main(string[] args)
        {
            List<string> values = File.ReadAllLines(inputFile).ToList();

            int twos = 0;
            int threes = 0;

            foreach(string id in values)
            {
                (bool two, bool three) = Characterize(id);

                if (two)
                {
                    twos++;
                }

                if (three)
                {
                    threes++;
                }
            }

            Console.WriteLine("Day 2 - Star 1");
            Console.WriteLine("");
            Console.WriteLine(twos * threes);

            Console.ReadKey();
        }

        static (bool two, bool three) Characterize(string id)
        {
            Dictionary<char, int> characters = new Dictionary<char, int>();

            foreach(char c in id)
            {
                if (characters.ContainsKey(c))
                {
                    characters[c] = characters[c] + 1;
                }
                else
                {
                    characters[c] = 1;
                }
            }

            bool two = false;
            bool three = false;
            foreach(int i in characters.Values)
            {
                switch (i)
                {
                    case 2:
                        two = true;
                        break;
                    case 3:
                        three = true;
                        break;
                    default:
                        break;
                }
            }

            return (two, three);
        }
    }
}
