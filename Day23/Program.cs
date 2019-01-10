using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day23
{
    class Program
    {
        private const string inputFile = "..\\..\\..\\input23.txt";

        private static List<Nanobot> nanobots;

        static void Main(string[] args)
        {
            Console.WriteLine("Day 23");
            Console.WriteLine("Star 1");
            Console.WriteLine("");

            nanobots = new List<Nanobot>();
            Nanobot biggestRadius = null;

            foreach (string line in System.IO.File.ReadAllLines(inputFile))
            {
                nanobots.Add(new Nanobot(line));

                if (biggestRadius == null || nanobots[nanobots.Count - 1].r > biggestRadius.r)
                {
                    biggestRadius = nanobots[nanobots.Count - 1];
                }
            }

            int interiorCount = 0;
            foreach (Nanobot bot in nanobots)
            {
                if (bot.Distance(biggestRadius) <= biggestRadius.r)
                {
                    interiorCount++;
                }
            }


            Console.WriteLine($"There are {interiorCount} bots inside radius.");

            Console.WriteLine("");
            Console.WriteLine("Star 2");
            Console.WriteLine("");

            //Find the coordinate in range of most


            //Let's try a binary search
            //Set up bounds in the laziest way
            long minX = nanobots.Min(x => x.x);
            long maxX = nanobots.Max(x => x.x) + 1;
            long minY = nanobots.Min(x => x.y);
            long maxY = nanobots.Max(x => x.y) + 1;
            long minZ = nanobots.Min(x => x.z);
            long maxZ = nanobots.Max(x => x.z) + 1;

            List<(long x0, long x1, long y0, long y1, long z0, long z1)> boxes = new List<(long x0, long x1, long y0, long y1, long z0, long z1)>();

            while (true)
            {
                boxes.Clear();

                long pivotX = (minX + maxX + 1) / 2;
                long pivotY = (minY + maxY + 1) / 2;
                long pivotZ = (minZ + maxZ + 1) / 2;

                boxes.Add((minX, pivotX, minY, pivotY, minZ, pivotZ));

                if (pivotX != maxX)
                {
                    boxes.Add((pivotX, maxX, minY, pivotY, minZ, pivotZ));

                    if (pivotY != maxY)
                    {
                        boxes.Add((pivotX, maxX, pivotY, maxY, minZ, pivotZ));

                        if (pivotZ != maxZ)
                        {
                            boxes.Add((pivotX, maxX, pivotY, maxY, pivotZ, maxZ));
                        }
                    }

                    if (pivotZ != maxZ)
                    {
                        boxes.Add((pivotX, maxX, minY, pivotY, pivotZ, maxZ));
                    }
                }

                if (pivotY != maxY)
                {
                    boxes.Add((minX, pivotX, pivotY, maxY, minZ, pivotZ));

                    if (pivotZ != maxZ)
                    {
                        boxes.Add((minX, pivotX, pivotY, maxY, pivotZ, maxZ));
                    }
                }

                if (pivotZ != maxZ)
                {
                    boxes.Add((minX, pivotX, minY, pivotY, pivotZ, maxZ));
                }



                int bestBox = 0;
                int bestBotCount = CountBots(boxes[0]);

                for (int i = 1; i < boxes.Count; i++)
                {
                    int count = CountBots(boxes[i]);
                    if (count > bestBotCount)
                    {
                        bestBox = i;
                        bestBotCount = count;
                    }
                    else if (count == bestBotCount)
                    {
                        //Break ties with box closest to origin
                        if (DistanceFromOrigin(boxes[i]) < DistanceFromOrigin(boxes[bestBox]))
                        {
                            bestBox = i;
                        }
                    }
                }

                minX = boxes[bestBox].x0;
                maxX = boxes[bestBox].x1;
                minY = boxes[bestBox].y0;
                maxY = boxes[bestBox].y1;
                minZ = boxes[bestBox].z0;
                maxZ = boxes[bestBox].z1;

                if ((minX + 1 == maxX) && (minY + 1 == maxY) && (minZ + 1 == maxZ))
                {
                    break;
                }
            }


            Console.WriteLine($"Best Position: {minX},{minY},{minZ}");
            Console.WriteLine($"Manhattan distance from origin: {Math.Abs(minX) + Math.Abs(minY) + Math.Abs(minZ)}");


            Console.WriteLine("");
            Console.ReadKey();
        }

        public static int CountBots((long x0, long x1, long y0, long y1, long z0, long z1) box)
        {
            int count = 0;

            foreach (Nanobot bot in nanobots)
            {
                if (bot.IsWithinRange(box.x0, box.x1, box.y0, box.y1, box.z0, box.z1))
                {
                    count++;
                }
            }

            return count;
        }

        private static long DistanceFromOrigin((long x0, long x1, long y0, long y1, long z0, long z1) box)
        {
            if ((0 >= box.x0 && 0 < box.x1) && (0 >= box.y0 && 0 < box.y1) && (0 >= box.z0 && 0 < box.z1))
            {
                return 0;
            }

            return Math.Abs(Math.Max(box.x0, Math.Min(box.x1, 0))) + Math.Abs(Math.Max(box.y0, Math.Min(box.y1, 0))) + Math.Abs(Math.Max(box.z0, Math.Min(box.z1, 0)));
        }

        public class Nanobot
        {
            public readonly long r;

            public readonly long x;
            public readonly long y;
            public readonly long z;

            public (long x, long y, long z) Position => (x, y, z);

            public Nanobot(string line)
            {
                int endOfPos = line.IndexOf('>');
                long[] values = line.Substring(5, endOfPos - 5).Split(',').Select(long.Parse).ToArray();

                x = values[0];
                y = values[1];
                z = values[2];


                int startOfRadius = line.IndexOf('r') + 2;

                r = long.Parse(line.Substring(startOfRadius));
            }

            public long Distance(Nanobot source)
            {
                return Math.Abs(x - source.x) + Math.Abs(y - source.y) + Math.Abs(z - source.z);
            }

            public long Distance((long x, long y, long z) source)
            {
                return Math.Abs(x - source.x) + Math.Abs(y - source.y) + Math.Abs(z - source.z);
            }

            public bool IsWithinRange(long x0, long x1, long y0, long y1, long z0, long z1)
            {
                if ((x >= x0 && x < x1) && (y >= y0 && y < y1) && (z >= z0 && z < z1))
                {
                    return true;
                }

                (long x, long y, long z) closest = (Math.Max(x0, Math.Min(x1, x)), Math.Max(y0, Math.Min(y1, y)), Math.Max(z0, Math.Min(z1, z)));

                return Distance(closest) <= r;
            }
        }
    }
}
