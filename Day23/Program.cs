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
                Nanobot newNanoBot = new Nanobot(line);
                nanobots.Add(newNanoBot);

                if (biggestRadius == null || newNanoBot.r > biggestRadius.r)
                {
                    biggestRadius = newNanoBot;
                }
            }

            int interiorCount = 0;
            foreach (Nanobot bot in nanobots)
            {
                if (biggestRadius.IsWithinRange(bot))
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
            Vector min = new Vector(
                x: nanobots.Min(x => x.position.x),
                y: nanobots.Min(x => x.position.y),
                z: nanobots.Min(x => x.position.z));

            Vector max = new Vector(
                x: nanobots.Max(x => x.position.x) + 1,
                y: nanobots.Max(x => x.position.y) + 1,
                z: nanobots.Max(x => x.position.z) + 1);

            List<Box> bestBoxes = new List<Box>();
            List<Box> testBoxes = new List<Box>
            {
                new Box(min, max)
            };

            while (true)
            {
                //Console.WriteLine("###################################");
                //Console.WriteLine("");
                //Console.WriteLine("Staring Iteration");
                //Console.WriteLine("");

                int bestBoxCount = -1;
                bestBoxes.Clear();

                foreach (Box testBox in testBoxes)
                {
                    foreach (Box box in testBox.SliceBox())
                    {
                        int count = CountBots(box);
                        //Console.WriteLine($"Box {box}, Count {count}");
                        if (count > bestBoxCount)
                        {
                            bestBoxes.Clear();
                            bestBoxes.Add(box);
                            bestBoxCount = count;
                        }
                        else if (count == bestBoxCount)
                        {
                            bestBoxes.Add(box);
                        }
                    }
                }

                //Console.WriteLine("");
                //Console.WriteLine($"Best Box Bot Count: {bestBoxCount}");
                //foreach (Box bestBox in bestBoxes)
                //{
                //    Console.WriteLine($"    {bestBox}");
                //}
                //Console.WriteLine("");

                testBoxes.Clear();
                testBoxes.AddRange(bestBoxes);

                Box testingBox = testBoxes[0];
                if ((testingBox.min.x + 1 == testingBox.max.x) && (testingBox.min.y + 1 == testingBox.max.y) && (testingBox.min.z + 1 == testingBox.max.z))
                {
                    break;
                }
            }

            Console.WriteLine($"Found {testBoxes.Count} final boxes");

            min = testBoxes.Select(x => x.min).OrderBy(x => x.DistanceFromOrigin).First();

            Console.WriteLine($"Best Position: {min}, Bots: {CountBots(min)}, Distance from Origin: {min.DistanceFromOrigin}");
            
            Console.WriteLine("");
            Console.ReadKey();
        }

        public static int CountBots(in Vector position)
        {
            int count = 0;

            foreach (Nanobot bot in nanobots)
            {
                if (bot.IsWithinRange(position))
                {
                    count++;
                }
            }

            return count;
        }

        public static int CountBots(Box box)
        {
            int count = 0;

            foreach (Nanobot bot in nanobots)
            {
                if (bot.IsWithinRange(box))
                {
                    count++;
                }
            }

            return count;
        }


        public readonly struct Box
        {
            public readonly Vector min;
            public readonly Vector max;

            public Box(in Vector min, in Vector max)
            {
                this.min = min;
                this.max = max;
            }

            public bool Contains(in Vector point) =>
                (point.x >= min.x && point.x < max.x) &&
                (point.y >= min.y && point.y < max.y) &&
                (point.z >= min.z && point.z < max.z);

            public long DistanceFromPoint(in Vector point)
            {
                if (Contains(point))
                {
                    return 0;
                }

                return Clamp(point, min, max - Vector.Ones).Distance(point);
            }

            public long DistanceFromOrigin => DistanceFromPoint(Vector.Origin);

            public override string ToString() => $"[{min} {max}]";

            public IEnumerable<Box> SliceBox()
            {
                Vector deltas = max - min;

                Vector sliceMin = Vector.Ones;
                Vector sliceMax = Vector.Ones * 4;

                Vector slices = Clamp(deltas, sliceMin, sliceMax);

                Vector stepSize = (deltas + slices - Vector.Ones) / slices;


                for (long xSlice = 0; xSlice < slices.x; xSlice++)
                {
                    for (long ySlice = 0; ySlice < slices.y; ySlice++)
                    {
                        for (long zSlice = 0; zSlice < slices.z; zSlice++)
                        {
                            Vector slice = new Vector(xSlice, ySlice, zSlice);

                            yield return new Box(
                                min: min + slice * stepSize,
                                max: min + (slice + Vector.Ones) * stepSize);
                        }
                    }
                }
            }
        }

        public readonly struct Vector
        {
            public static readonly Vector Origin = new Vector(0, 0, 0);
            public static readonly Vector Ones = new Vector(1, 1, 1);

            public readonly long x;
            public readonly long y;
            public readonly long z;

            public long DistanceFromOrigin => Math.Abs(x) + Math.Abs(y) + Math.Abs(z);

            public Vector(long x, long y, long z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public long Distance(in Vector v) => Math.Abs(x - v.x) + Math.Abs(y - v.y) + Math.Abs(z - v.z);

            public static Vector operator +(in Vector lhs, in Vector rhs)
            {
                return new Vector(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
            }

            public static Vector operator -(in Vector lhs, in Vector rhs)
            {
                return new Vector(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
            }

            /// <summary>
            /// Element-wise division
            /// </summary>
            public static Vector operator /(in Vector lhs, in Vector rhs)
            {
                return new Vector(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z);
            }

            /// <summary>
            /// Element-wise multiplication
            /// </summary>
            public static Vector operator *(in Vector lhs, in Vector rhs)
            {
                return new Vector(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
            }

            public static bool operator ==(in Vector lhs, in Vector rhs)
            {
                return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
            }

            public static bool operator !=(in Vector lhs, in Vector rhs)
            {
                return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
            }

            public static Vector operator /(in Vector lhs, long rhs)
            {
                return new Vector(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs);
            }

            public static Vector operator *(in Vector lhs, long rhs)
            {
                return new Vector(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
            }

            public override string ToString() => $"({x,10},{y,10},{z,10})";

            public override bool Equals(object obj)
            {
                if (!(obj is Vector))
                {
                    return false;
                }

                Vector vector = (Vector)obj;
                return x == vector.x &&
                       y == vector.y &&
                       z == vector.z;
            }

            public override int GetHashCode()
            {
                int hashCode = 373119288;
                hashCode = hashCode * -1521134295 + x.GetHashCode();
                hashCode = hashCode * -1521134295 + y.GetHashCode();
                hashCode = hashCode * -1521134295 + z.GetHashCode();
                return hashCode;
            }
        }


        public class Nanobot
        {
            public readonly long r;

            public readonly Vector position;

            public Nanobot(string line)
            {
                int endOfPos = line.IndexOf('>');
                long[] values = line.Substring(5, endOfPos - 5).Split(',').Select(long.Parse).ToArray();

                position = new Vector(
                    x: values[0],
                    y: values[1],
                    z: values[2]);


                int startOfRadius = line.IndexOf('r') + 2;

                r = long.Parse(line.Substring(startOfRadius));
            }

            public bool IsWithinRange(Nanobot source) => position.Distance(source.position) <= r;
            public bool IsWithinRange(Box box) => box.DistanceFromPoint(position) <= r;
            public bool IsWithinRange(Vector pos) => pos.Distance(position) <= r;
        }

        public static long Clamp(long value, long min, long max) => Math.Max(min, Math.Min(value, max));

        public static Vector Clamp(in Vector value, in Vector min, in Vector max) =>
            new Vector(
                x: Clamp(value.x, min.x, max.x),
                y: Clamp(value.y, min.y, max.y),
                z: Clamp(value.z, min.z, max.z));

        public static Vector Min(in Vector a, in Vector b) =>
            new Vector(
                x: Math.Min(a.x, b.x),
                y: Math.Min(a.y, b.y),
                z: Math.Min(a.z, b.z));

        public static Vector Max(in Vector a, in Vector b) =>
            new Vector(
                x: Math.Max(a.x, b.x),
                y: Math.Max(a.y, b.y),
                z: Math.Max(a.z, b.z));
    }
}
