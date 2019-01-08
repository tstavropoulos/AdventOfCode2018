using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day17
{
    class Program
    {
        private const string inputFile = "..\\..\\..\\input17.txt";

        private const char Empty = ' ';
        private const char Source = '+';
        private const char WaterFlow = '|';
        private const char Steady = '~';
        public const char Clay = '#';

        private static char[,] field;
        private static int XOffset;
        private static int depth;
        private static Queue<Flow> sourceQueue;

        static void Main(string[] args)
        {
            Console.WriteLine("Day 17");
            Console.WriteLine("Star 1");
            Console.WriteLine("");

            List<Range> ranges = new List<Range>();

            foreach (string line in System.IO.File.ReadAllLines(inputFile))
            {
                string[] splitLine = line.Split(',');

                if (splitLine[0][0] == 'x')
                {
                    ranges.Add(new YRange(
                        x: splitLine[0].Substring(2),
                        y: splitLine[1].Substring(3)));
                }
                else
                {
                    ranges.Add(new XRange(
                        x: splitLine[1].Substring(3),
                        y: splitLine[0].Substring(2)));
                }
            }


            depth = 0;

            int minX = 500;
            int maxX = 500;
            int minY = int.MaxValue;

            foreach (Range range in ranges)
            {
                if (range.MinY < minY)
                {
                    minY = range.MinY;
                }

                if (range.MaxY > depth)
                {
                    depth = range.MaxY;
                }

                if (range.MinX < minX)
                {
                    minX = range.MinX;
                }

                if (range.MaxX > maxX)
                {
                    maxX = range.MaxX;
                }
            }

            //Move the boundaries out one more
            minX--;
            maxX++;

            XOffset = minX;

            int w = maxX - minX + 3;
            int h = depth + 1;

            field = new char[w, h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    field[x, y] = Empty;
                }
            }

            field[500 - minX, 0] = Source;

            foreach (Range range in ranges)
            {
                range.Apply();
            }

            //Flow travels downward until it is over an obstacle, and then sideways until it is not.

            //When flowing Down:
            //If you hit a Flow (|), stop.
            //If you hit Clay (#) or Still Water (~), propagate Left and Right

            //When flowing Sideways:
            //Flow left until termination, then flow right
            //If you hit a Flow (|), stop.
            //If you hit Clay when going both directions, convert to still water and jump to prior flow as new source to propagate.


            sourceQueue = new Queue<Flow>();
            sourceQueue.Enqueue(new DownFlow(500 - minX, 1));
            field[500 - minX, 1] = WaterFlow;


            while (sourceQueue.Count > 0)
            {
                sourceQueue.Dequeue().ExecuteFlow();
            }


            //Console.WriteLine("");
            //for (int y = 0; y < h; y++)
            //{
            //    for (int x = 0; x < w; x++)
            //    {
            //        Console.Write(field[x, y]);
            //    }
            //    Console.Write('\n');
            //}

            int flowCount = 0;
            int steadyCount = 0;
            for (int y = minY; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    switch (field[x,y])
                    {
                        case Steady:
                            steadyCount++;
                            break;
                        case WaterFlow:
                            flowCount++;
                            break;
                        default:
                            break;
                    }
                }
            }

            Console.WriteLine($"Tiles with water: {steadyCount + flowCount}");
            Console.WriteLine("");

            Console.WriteLine("Star 2");
            Console.WriteLine("");
            Console.WriteLine($"Tiles with steady water: {steadyCount}");

            Console.ReadKey();
        }

        public abstract class Flow
        {
            public abstract void ExecuteFlow();
        }

        public class DownFlow : Flow
        {
            int x;
            int y;

            public DownFlow(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public override void ExecuteFlow()
            {
                if (field[x, y] == Steady)
                {
                    //Watch out for starting a dead flow in steady water
                    return;
                }

                while (y < depth && field[x, y + 1] == Empty)
                {
                    ++y;

                    field[x, y] = WaterFlow;
                }

                if (y == depth)
                {
                    //Don't propagate below bottom - duh!
                    return;
                }

                switch (field[x, y + 1])
                {
                    case WaterFlow: return;

                    case Clay:
                    case Steady:
                        //Propagate Both directions
                        sourceQueue.Enqueue(new AcrossFlow(x, y));

                        break;
                    default: throw new Exception();
                }
            }
        }

        public class AcrossFlow : Flow
        {
            int x;
            int y;

            public AcrossFlow(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public override void ExecuteFlow()
            {
                int leftX = x;
                int rightX = x;

                if (field[x, y] == Steady)
                {
                    //Watch out for starting a dead flow in steady water
                    return;
                }

                while (field[leftX - 1, y] == Empty || field[leftX - 1, y] == WaterFlow)
                {
                    --leftX;

                    field[leftX, y] = WaterFlow;
                    if (field[leftX, y + 1] == Empty)
                    {
                        sourceQueue.Enqueue(new DownFlow(leftX, y));
                        break;
                    }
                    else if (field[leftX, y + 1] == WaterFlow)
                    {
                        break;
                    }
                }

                while (field[rightX + 1, y] == Empty || field[rightX + 1, y] == WaterFlow)
                {
                    ++rightX;

                    field[rightX, y] = WaterFlow;
                    if (field[rightX, y + 1] == Empty)
                    {
                        sourceQueue.Enqueue(new DownFlow(rightX, y));
                        break;
                    }
                    else if (field[rightX, y + 1] == WaterFlow)
                    {
                        break;
                    }
                }

                if (field[leftX - 1, y] == Clay && field[rightX + 1, y] == Clay)
                {
                    //Test bottoms
                    bool solidBottom = true;
                    for (int x2 = leftX; x2 <= rightX; x2++)
                    {
                        solidBottom &= (field[x2, y + 1] == Clay || field[x2, y + 1] == Steady);
                    }

                    if (solidBottom)
                    {
                        for (int x2 = leftX; x2 <= rightX; x2++)
                        {
                            field[x2, y] = Steady;
                            if (field[x2, y - 1] == WaterFlow)
                            {
                                sourceQueue.Enqueue(new AcrossFlow(x2, y - 1));
                            }
                        }
                    }
                }
            }
        }

        public abstract class Range
        {
            public abstract int MinX { get; }
            public abstract int MaxX { get; }
            public abstract int MinY { get; }
            public abstract int MaxY { get; }

            public abstract void Apply();
        }

        public class XRange : Range
        {
            public readonly int x0;
            public readonly int x1;
            public readonly int y;

            public override int MinX => x0;
            public override int MaxX => x1;
            public override int MinY => y;
            public override int MaxY => y;

            public XRange(string x, string y)
            {
                x0 = int.Parse(x.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                x1 = int.Parse(x.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[1]);

                this.y = int.Parse(y);
            }

            public override void Apply()
            {
                for (int x = x0; x <= x1; x++)
                {
                    field[x - XOffset, y] = Clay;
                }
            }
        }

        public class YRange : Range
        {
            public readonly int y0;
            public readonly int y1;
            public readonly int x;

            public override int MinX => x;
            public override int MaxX => x;
            public override int MinY => y0;
            public override int MaxY => y1;

            public YRange(string x, string y)
            {
                y0 = int.Parse(y.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                y1 = int.Parse(y.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[1]);

                this.x = int.Parse(x);
            }

            public override void Apply()
            {
                for (int y = y0; y <= y1; y++)
                {
                    field[x - XOffset, y] = Clay;
                }
            }
        }
    }

}
