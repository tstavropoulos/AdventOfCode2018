﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day11
{
    class Program
    {

        static int[,] fuelCells;

        static void Main(string[] args)
        {
            Console.WriteLine("Day 11");
            Console.WriteLine("Star 1");
            Console.WriteLine("");

            //Find the 3x3 with largest total power

            fuelCells = new int[300, 300];

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
                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();


                int maxValue = -9999;
                int maxX = -1;
                int maxY = -1;
                int maxSize = -1;

                Console.WriteLine("Star 2");
                Console.WriteLine("");


                Task<(int, int, int, int)>[] searchTasks = new Task<(int, int, int, int)>[300];

                searchTasks[0] = Task.Run(() => UnoptimizedFindBest(1));
                for (int size = 2; size <= 300; size++)
                {
                    int tempSize = size;
                    searchTasks[size - 1] = Task.Run(() => Opt1FindBest(tempSize));
                }

                Task.WaitAll(searchTasks);

                (int, int, int, int)[] results = searchTasks.Select(x => x.Result).ToArray();

                (maxValue, maxSize, maxX, maxY) = results.OrderByDescending(x => x.Item1).First();

                ////Tired single-threaded brute-force

                //for (int size = 1; size <= 300; size++)
                //{
                //    for (int x = 0; x < 301 - size; x++)
                //    {
                //        for (int y = 0; y < 301 - size; y++)
                //        {
                //            int tempVal = 0;
                //            for (int dx = 0; dx < size; dx++)
                //            {
                //                for (int dy = 0; dy < size; dy++)
                //                {
                //                    tempVal += fuelCells[x + dx, y + dy];
                //                }
                //            }

                //            if (tempVal > maxValue)
                //            {
                //                maxValue = tempVal;
                //                maxX = x;
                //                maxY = y;
                //                maxSize = size;
                //            }
                //        }
                //    }
                //}

                stopWatch.Stop();

                Console.WriteLine($"Best Value: ({maxX + 1},{maxY + 1},{maxSize}): {maxValue}");
                Console.WriteLine("");
                Console.WriteLine($"Solution took {stopWatch.Elapsed.TotalSeconds} seconds");
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

        //34 seconds
        private static (int, int, int, int) UnoptimizedFindBest(int size)
        {
            int maxValue = int.MinValue;
            int maxX = 0;
            int maxY = 0;

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
                    }
                }
            }

            return (maxValue, size, maxX, maxY);
        }

        //0.801 Seconds
        private static (int, int, int, int) Opt1FindBest(int size)
        {
            int maxValue = int.MinValue;
            int maxX = 0;
            int maxY = 0;

            for (int x = 0; x < 301 - size; x++)
            {
                //First Square of Column
                int tempVal = 0;
                for (int dx = 0; dx < size; dx++)
                {
                    for (int dy = 0; dy < size; dy++)
                    {
                        tempVal += fuelCells[x + dx, dy];
                    }
                }

                //Compare
                if (tempVal > maxValue)
                {
                    maxValue = tempVal;
                    maxX = x;
                    maxY = 0;
                }

                //Acquire later squares by adding new row and subtracting old row
                for (int y = 0; y < 300 - size; y++)
                {
                    for (int dx = 0; dx < size; dx++)
                    {
                        tempVal -= fuelCells[x + dx, y];
                        tempVal += fuelCells[x + dx, y + size];
                    }

                    if (tempVal > maxValue)
                    {
                        maxValue = tempVal;
                        maxX = x;
                        maxY = y + 1;
                    }
                }
            }

            return (maxValue, size, maxX, maxY);
        }
    }
}
