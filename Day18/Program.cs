using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day18
{
    class Program
    {
        private const string inputFile = "..\\..\\..\\input18.txt";

        const char Open = '.';
        const char Trees = '|';
        const char Lumberyard = '#';

        static char[,] areaA;
        static char[,] areaB;
        static int Size = 50;

        static void Main(string[] args)
        {
            Console.WriteLine("Day 18");
            Console.WriteLine("Star 1");
            Console.WriteLine("");

            string[] inputFileLines = System.IO.File.ReadAllLines(inputFile);

            Size = inputFileLines.Length;

            char[,] initialCondition = new char[Size, Size];
            areaA = new char[Size, Size];
            areaB = new char[Size, Size];

            char[,] tempAreaC = new char[Size, Size];

            for (int j = 0; j < Size; j++)
            {
                for (int i = 0; i < Size; i++)
                {
                    areaA[i, j] = inputFileLines[j][i];
                    initialCondition[i, j] = areaA[i, j];
                }
            }

            for (int gen = 0; gen < 10; gen++)
            {
                AdvanceMap();
            }


            Console.WriteLine("");
            Console.WriteLine($"Resource Value: {CalculateResourceValue()}");
            Console.WriteLine("");

            Console.WriteLine("Star 2");
            Console.WriteLine("");

            //Reset to initial condition
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    areaA[x, y] = initialCondition[x, y];
                }
            }

            HashSet<int> seenStates = new HashSet<int>();
            int firstClearIndex = -1;
            int cyclePeriod = -1;
            int cycleStartHash = -1;

            for (int gen = 0; gen < 1_000_000_000; gen++)
            {
                AdvanceMap();

                if (!seenStates.Add(EncodeMap()))
                {
                    if (firstClearIndex == -1)
                    {
                        firstClearIndex = gen;
                        cycleStartHash = EncodeMap();
                        Console.WriteLine($"First cycle detected at {gen}. Clearing.");

                        seenStates.Clear();
                        seenStates.Add(cycleStartHash);
                    }
                    else
                    {
                        cyclePeriod = gen - firstClearIndex;
                        Console.WriteLine($"Second cycle completed. Took from {firstClearIndex} to {gen}.  Cycle Period: {cyclePeriod}");

                        break;
                    }
                }
            }


            int targetMinute = 1_000_000_000;

            int targetPhase = (targetMinute - firstClearIndex - 1) % cyclePeriod;
            //We are currently at the start of the cycle, so we only need to advance by the Phase

            for (int gen = 0; gen < targetPhase; gen++)
            {
                AdvanceMap();
            }

            Console.WriteLine($"Resource Value: {CalculateResourceValue()}");
            Console.WriteLine("");

            Console.ReadKey();
        }

        static int EncodeMap()
        {
            const int SamplesPerBatch = 5;
            int samplesInBatch = 0;
            int accumulatedSamples = 0;

            int hash = (Size * Size) / SamplesPerBatch;

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    accumulatedSamples = 3 * accumulatedSamples + GetEncodedValue(x, y);

                    if (++samplesInBatch == SamplesPerBatch)
                    {
                        hash = unchecked(hash * 314159 + accumulatedSamples);
                        samplesInBatch = 0;
                        accumulatedSamples = 0;
                    }
                }
            }

            if (samplesInBatch > 0)
            {
                hash = unchecked(hash * 314159 + accumulatedSamples);
            }

            return hash;
        }

        static int GetEncodedValue(int x, int y)
        {
            switch (areaA[x, y])
            {
                case Open: return 0;
                case Trees: return 1;
                case Lumberyard: return 2;
                default: throw new Exception();
            }
        }

        static void CopyMap(char[,] destination)
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    destination[x, y] = areaA[x, y];
                }
            }
        }
        static bool CompareMaps(char[,] a, char[,] b)
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    if (a[x, y] != b[x, y])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        static void PrintMap()
        {
            Console.Write("\n\n");

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    Console.Write(areaA[x, y]);
                }

                Console.Write('\n');
            }
        }

        static void AdvanceMap()
        {
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    areaB[x, y] = areaA[x, y];

                    switch (areaA[x, y])
                    {
                        case Open:
                            if (CountNeighbors(x, y, Trees) >= 3)
                            {
                                areaB[x, y] = Trees;
                            }
                            break;
                        case Trees:
                            if (CountNeighbors(x, y, Lumberyard) >= 3)
                            {
                                areaB[x, y] = Lumberyard;
                            }
                            break;
                        case Lumberyard:
                            if (CountNeighbors(x, y, Trees) == 0 || CountNeighbors(x, y, Lumberyard) == 0)
                            {
                                areaB[x, y] = Open;
                            }
                            break;
                        default: throw new Exception();
                    }
                }
            }

            (areaA, areaB) = (areaB, areaA);
        }

        static int CountNeighbors(int x0, int y0, char field)
        {
            int hits = 0;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        //Skip the center tile
                        continue;
                    }

                    if (x0 + x < 0 || x0 + x >= Size)
                    {
                        continue;
                    }

                    if (y0 + y < 0 || y0 + y >= Size)
                    {
                        continue;
                    }

                    if (areaA[x0 + x, y0 + y] == field)
                    {
                        hits++;
                    }
                }
            }

            return hits;
        }

        static int CalculateResourceValue()
        {
            int lumberyards = 0;
            int woodedAreas = 0;

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    switch (areaA[x, y])
                    {
                        case Lumberyard:
                            lumberyards++;
                            break;
                        case Open:
                            break;
                        case Trees:
                            woodedAreas++;
                            break;
                    }
                }
            }

            return lumberyards * woodedAreas;
        }
    }
}
