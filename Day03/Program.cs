using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day03
{
    class Program
    {
        private const string inputFile = "..\\..\\..\\input3.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Day 3");
            Console.WriteLine("Star 1");
            Console.WriteLine("");

            List<string> values = System.IO.File.ReadAllLines(inputFile).ToList();
            List<Claim> claims = new List<Claim>();

            foreach (string value in values)
            {
                claims.Add(new Claim(value));
            }

            values = null;

            //Find the size:

            int w = 0;
            int h = 0;

            foreach (Claim claim in claims)
            {
                if (claim.X1 > w)
                {
                    w = claim.X1;
                }

                if (claim.Y1 > h)
                {
                    h = claim.Y1;
                }
            }

            int[,] claimMap = new int[w, h];

            foreach (Claim claim in claims)
            {
                for (int x = claim.x0; x < claim.X1; x++)
                {
                    for (int y = claim.y0; y < claim.Y1; y++)
                    {
                        claimMap[x, y]++;
                    }
                }
            }

            int count = 0;
            {
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        if (claimMap[x, y] > 1)
                        {
                            count++;
                        }
                    }
                }
            }

            Console.WriteLine($"Square inches within two claims: {count}");
            Console.WriteLine("");


            Console.WriteLine("Star 2");
            Console.WriteLine("");

            int claimID = 0;

            foreach (Claim claim in claims)
            {
                bool valid = true;
                for (int x = claim.x0; x < claim.X1; x++)
                {
                    for (int y = claim.y0; y < claim.Y1; y++)
                    {
                        valid = claimMap[x, y] == 1;
                        if (!valid)
                        {
                            break;
                        }
                    }

                    if (!valid)
                    {
                        break;
                    }
                }

                if (!valid)
                {
                    continue;
                }

                claimID = claim.num;
                break;
            }

            Console.WriteLine($"Claim with no other overlapping claim: {claimID}");
            Console.WriteLine("");

            Console.ReadKey();
        }
        class Claim
        {
            public int num;
            public int x0;
            public int y0;
            public int w;
            public int h;

            public int X1 => x0 + w;
            public int Y1 => y0 + h;

            private static readonly char[] separators = new char[] { '#', '@', ' ', ',', ':', 'x' };
            public Claim(string serialized)
            {
                string[] splitStr =
                    serialized.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                num = int.Parse(splitStr[0]);
                x0 = int.Parse(splitStr[1]);
                y0 = int.Parse(splitStr[2]);
                w = int.Parse(splitStr[3]);
                h = int.Parse(splitStr[4]);
            }

        }
    }
}
