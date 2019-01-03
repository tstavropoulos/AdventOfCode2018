using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day25
{
    class Program
    {
        const string inputFile = "../../../input25.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Day 25");
            Console.WriteLine("Star 1");
            Console.WriteLine("");

            Stack<Coordinate> coords = new Stack<Coordinate>(File.ReadAllLines(inputFile).Select(x => new Coordinate(x)));

            List<Constellation> constellations = new List<Constellation>();

            constellations.Add(new Constellation(coords.Pop()));
            List<Constellation> matches = new List<Constellation>();

            while (coords.Count > 0)
            {
                Coordinate coord = coords.Pop();
                matches.Clear();

                foreach (Constellation constellation in constellations)
                {
                    if (constellation.TestStar(coord))
                    {
                        matches.Add(constellation);
                    }
                }

                if (matches.Count >= 1)
                {
                    matches[0].Add(coord);
                    for (int i = 1; i < matches.Count; i++)
                    {
                        matches[0].Merge(matches[i]);
                        constellations.Remove(matches[i]);
                    }
                }
                else
                {
                    constellations.Add(new Constellation(coord));
                }
            }

            Console.WriteLine($"Number of constellations: {constellations.Count}");
            Console.WriteLine("");
            Console.WriteLine("Star 2");
            Console.WriteLine("");
            Console.ReadKey();

        }

        public class Constellation
        {
            public List<Coordinate> stars = new List<Coordinate>();

            public Constellation(Coordinate star)
            {
                stars.Add(star);
            }

            public void Add(Coordinate star)
            {
                stars.Add(star);
            }

            public void Merge(Constellation other)
            {
                stars.AddRange(other.stars);
            }

            public bool TestStar(Coordinate testStar)
            {
                foreach (Coordinate star in stars)
                {
                    if (star.Distance(testStar) <= 3)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public class Coordinate
        {
            public int x;
            public int y;
            public int z;
            public int t;

            public Coordinate(string serialized)
            {
                string[] split = serialized.Split(',');

                x = int.Parse(split[0]);
                y = int.Parse(split[1]);
                z = int.Parse(split[2]);
                t = int.Parse(split[3]);
            }

            public int Distance(Coordinate otherStar)
            {
                return (Math.Abs(otherStar.x - x) +
                    Math.Abs(otherStar.y - y) +
                    Math.Abs(otherStar.z - z) +
                    Math.Abs(otherStar.t - t));
            }
        }
    }
}
