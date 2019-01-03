using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day11
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Day 11");
            Console.WriteLine("Star 1");
            Console.WriteLine("");

            //Find the 3x3 with largest total power

            int[,] fuelCells = new int[300, 300];

            for (int x = 0; x < 300; x++)
            {
                for (int y = 0; y < 300; y++)
                {
                    fuelCells[x, y] = GetValue(x, y);
                }
            }

            {

                int maxValue = -9999;
                int maxX = -1;
                int maxY = -1;

                for (int x = 0; x < 300 - 2; x++)
                {
                    for (int y = 0; y < 300 - 2; y++)
                    {
                        int tempVal = 0;
                        for (int dx = 0; dx < 3; dx++)
                        {
                            for (int dy = 0; dy < 3; dy++)
                            {
                                tempVal += fuelCells[x + dx, y + dy];
                            }
                        }

                        if (tempVal > maxValue)
                        {
                            maxValue = tempVal;
                            maxX = x;
                            maxY = y;
                        }
                    }
                }

                Console.WriteLine($"Best Value: ({maxX + 1},{maxY + 1}): {maxValue}");
                Console.WriteLine("");
            }

            {

                int maxValue = -9999;
                int maxX = -1;
                int maxY = -1;
                int maxSize = -1;

                Console.WriteLine("Star 2");
                Console.WriteLine("");

                for (int size = 1; size <= 300; size++)
                {
                    for (int x = 0; x < 301 - size; x++)
                    {
                        for (int y = 0; y < 301 - size; y++)
                        {
                            int tempVal = 0;
                            for (int dx = 0; dx < size; dx++)
                            {
                                for (int dy = 0; dy < size; dy++)
                                {
                                    tempVal += fuelCells[x + dx, y + dy];
                                }
                            }

                            if (tempVal > maxValue)
                            {
                                maxValue = tempVal;
                                maxX = x;
                                maxY = y;
                                maxSize = size;
                            }
                        }
                    }
                }

                Console.WriteLine($"Best Value: ({maxX + 1},{maxY + 1},{maxSize}): {maxValue}");
                Console.WriteLine("");
            }



            Console.ReadKey();
        }

        public static int GetValue(int x, int y)
        {
            //Fixing zero-indexing
            x++;
            y++;
            const long input = 7347;
            long RackID = x + 10;
            return (int)(((RackID * RackID * y + RackID * input) / 100) % 10) - 5;
        }
    }
}
