using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day6
{
    class Program
    {
        private const string inputFile = "..\\..\\..\\input6.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Day 6");
            Console.WriteLine("Star 1");
            Console.WriteLine("");

            List<Coordinate> coordinates = File.ReadAllLines(inputFile)
                .Select(x => new Coordinate(x))
                .ToList();

            Coordinate min = coordinates[0];
            Coordinate max = coordinates[0];

            foreach (Coordinate coord in coordinates)
            {
                if (coord.x < min.x)
                {
                    min.x = coord.x;
                }

                if (coord.y < min.y)
                {
                    min.y = coord.y;
                }

                if (coord.x >= max.x)
                {
                    max.x = coord.x + 1;
                }

                if (coord.y >= max.y)
                {
                    max.y = coord.y + 1;
                }
            }

            int width = max.x - min.x;
            int height = max.y - min.y;

            int[,] gridAssignment = new int[width, height];
            int[,] gridDistance = new int[width, height];
            long[,] totalGridDistance = new long[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gridAssignment[x, y] = -1;
                    gridDistance[x, y] = int.MaxValue;
                }
            }

            HashSet<int> validTargets = new HashSet<int>();

            for (int c = 0; c < coordinates.Count; c++)
            {
                //Just accumulate the valid targets here
                validTargets.Add(c);
                Coordinate coord = coordinates[c];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int distance = coord.Distance(min.x + x, min.y + y);
                        totalGridDistance[x, y] += distance;
                        if (distance < gridDistance[x, y])
                        {
                            gridDistance[x, y] = distance;
                            gridAssignment[x, y] = c;
                        }
                        else if (distance == gridDistance[x, y])
                        {
                            gridAssignment[x, y] = -1;
                        }
                    }
                }
            }
            
            for (int x = 0; x < width; x++)
            {
                validTargets.Remove(gridAssignment[x, 0]);
                validTargets.Remove(gridAssignment[x, height - 1]);
            }

            for (int y = 0; y < height; y++)
            {
                validTargets.Remove(gridAssignment[0, y]);
                validTargets.Remove(gridAssignment[width - 1, y]);
            }


            Console.WriteLine($"{validTargets.Count} different non-infinite zones");

            int maxArea = 0;
            int maxTarget = -1;

            foreach(int target in validTargets)
            {
                int area = 0;
                for (int x = 1; x < width-1; x++)
                {
                    for (int y = 1; y < height-1; y++)
                    {
                        if (gridAssignment[x,y] == target)
                        {
                            area++;
                        }
                    }
                }

                if (area > maxArea)
                {
                    maxArea = area;
                    maxTarget = target;
                }

            }


            Console.WriteLine("");
            Console.WriteLine($"Zone {maxTarget} has an area of {maxArea}");


            int zone2Count = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (totalGridDistance[x, y] < 10_000)
                    {
                        zone2Count++;
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine("Star 2");
            Console.WriteLine("");
            Console.WriteLine($"{zone2Count} spaces with distance less than 10,000.");
            Console.WriteLine("");

            Console.ReadKey();
        }

        public struct Coordinate
        {
            public int x;
            public int y;

            public Coordinate(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public Coordinate(string serialized)
            {
                int[] parsed = serialized.Split(',').Select(int.Parse).ToArray();

                x = parsed[0];
                y = parsed[1];
            }

            public int Distance(int x, int y)
            {
                return Math.Abs(this.x - x) + Math.Abs(this.y - y);
            }
        }
    }
}
