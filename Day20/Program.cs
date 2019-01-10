using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day20
{
    class Program
    {
        private const string inputFile = "..\\..\\..\\input20_Cleaned.txt";

        static Dictionary<(int x, int y), Room> rooms;

        static void Main(string[] args)
        {
            Console.WriteLine("Day 20");
            Console.WriteLine("Star 1");
            Console.WriteLine("");

            rooms = new Dictionary<(int x, int y), Room>();

            Room startingRoom = new Room(0, 0);

            string input = System.IO.File.ReadAllText(inputFile);

            //Test 1
            //string input = "^ENWWW(NEEE|SSE(EE|N))$";

            //Test 2
            //string input = "^ENNWSWW(NEWS|)SSSEEN(WNSE|)EE(SWEN|)NNN$";

            //Some simple cleanup
            //The input has been cleaned by regex to replace cycles like (NEWS|) => NEWS
            //\(([NSEW]*)\|\) => $1



            //Build map
            TravelPaths(startingRoom, input.Substring(1, input.Length - 2));

            int minX = 0;
            int maxX = 0;
            int minY = 0;
            int maxY = 0;

            foreach (Room room in rooms.Values)
            {
                if (room.x < minX)
                {
                    minX = room.x;
                }

                if (room.x >= maxX)
                {
                    maxX = room.x + 1;
                }

                if (room.y < minY)
                {
                    minY = room.y;
                }

                if (room.y >= maxY)
                {
                    maxY = room.y + 1;
                }
            }

            Room[,] roomGrid = new Room[maxX - minX, maxY - minY];
            int[,] cost = new int[maxX - minX, maxY - minY];

            for (int i = 0; i < cost.GetLength(0); i++)
            {
                for (int j = 0; j < cost.GetLength(1); j++)
                {
                    cost[i, j] = int.MaxValue;
                }
            }

            foreach (Room room in rooms.Values)
            {
                roomGrid[room.x - minX, room.y - minY] = room;
            }

            cost[startingRoom.x - minX, startingRoom.y - minY] = 0;
            Queue<Room> pendingRooms = new Queue<Room>();
            pendingRooms.Enqueue(startingRoom);

            while (pendingRooms.Count > 0)
            {
                Room nextRoom = pendingRooms.Dequeue();

                int newCost = cost[nextRoom.x - minX, nextRoom.y - minY] + 1;

                if (nextRoom.N != null && cost[nextRoom.N.x - minX, nextRoom.N.y - minY] == int.MaxValue)
                {
                    cost[nextRoom.N.x - minX, nextRoom.N.y - minY] = newCost;
                    pendingRooms.Enqueue(nextRoom.N);
                }

                if (nextRoom.S != null && cost[nextRoom.S.x - minX, nextRoom.S.y - minY] == int.MaxValue)
                {
                    cost[nextRoom.S.x - minX, nextRoom.S.y - minY] = newCost;
                    pendingRooms.Enqueue(nextRoom.S);
                }

                if (nextRoom.E != null && cost[nextRoom.E.x - minX, nextRoom.E.y - minY] == int.MaxValue)
                {
                    cost[nextRoom.E.x - minX, nextRoom.E.y - minY] = newCost;
                    pendingRooms.Enqueue(nextRoom.E);
                }

                if (nextRoom.W != null && cost[nextRoom.W.x - minX, nextRoom.W.y - minY] == int.MaxValue)
                {
                    cost[nextRoom.W.x - minX, nextRoom.W.y - minY] = newCost;
                    pendingRooms.Enqueue(nextRoom.W);
                }

            }

            int maxCost = 0;
            int highCost = 0;

            foreach (Room room in rooms.Values)
            {
                if (cost[room.x - minX, room.y - minY] > maxCost)
                {
                    maxCost = cost[room.x - minX, room.y - minY];
                }

                if (cost[room.x - minX, room.y - minY] >= 1000)
                {
                    highCost++;
                }
            }

            Console.WriteLine($"The farthest room requires passing through {maxCost} doors.");
            Console.WriteLine("");


            Console.WriteLine("Star 2");
            Console.WriteLine("");
            Console.WriteLine($"There are {highCost} rooms father at least 1000 doors away.");

            Console.WriteLine("");
            Console.ReadKey();
        }

        public static void TravelPaths(Room currentRoom, string input)
        {
            if (input.Length == 0)
            {
                //Empty - We are done
                return;
            }


            int firstIndex = input.IndexOf('(', 0);

            if (firstIndex == -1)
            {
                //No ( found - Travel the remainder
                foreach (char c in input)
                {
                    currentRoom = currentRoom.Travel(c);
                }

                return;
            }

            //Travel up to the next split
            for (int i = 0; i < firstIndex; i++)
            {
                currentRoom = currentRoom.Travel(input[i]);
            }

            int depth = 1;
            int closePosition = -1;
            List<int> pipePosition = new List<int>();


            for (int i = firstIndex + 1; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case 'N':
                    case 'S':
                    case 'W':
                    case 'E':
                        //Don't care.
                        break;
                    case '(':
                        {
                            ++depth;
                        }
                        break;
                    case ')':
                        {
                            if (depth-- == 1)
                            {
                                pipePosition.Add(i);
                                closePosition = i;
                                goto Breakout;
                            }
                        }
                        break;
                    case '|':
                        {
                            if (depth == 1)
                            {
                                pipePosition.Add(i);
                            }
                        }
                        break;
                    default: throw new Exception();
                }
            }

        Breakout:

            int currentPos = firstIndex + 1;

            for (int i = 0; i < pipePosition.Count; i++)
            {
                int nextPos = pipePosition[i];

                TravelPaths(
                    currentRoom: currentRoom,
                    input: input.Substring(currentPos, nextPos - currentPos) + input.Substring(closePosition + 1));
                currentPos = nextPos + 1;
            }
        }

        public class Room
        {
            public int x;
            public int y;

            public (int x, int y) Coordinates => (x, y);

            public Room N = null;
            public Room S = null;
            public Room E = null;
            public Room W = null;

            public Room(int x, int y)
            {
                this.x = x;
                this.y = y;

                if (rooms.ContainsKey((x, y)))
                {
                    throw new Exception();
                }

                rooms.Add((x, y), this);
            }

            public Room(int x, int y, char source)
                : this(x, y)
            {
                switch (source)
                {
                    case 'N':
                        S = rooms[(x, y - 1)];
                        break;
                    case 'S':
                        N = rooms[(x, y + 1)];
                        break;
                    case 'E':
                        W = rooms[(x - 1, y)];
                        break;
                    case 'W':
                        E = rooms[(x + 1, y)];
                        break;
                    default: throw new Exception();
                }
            }

            public Room Travel(char dir)
            {
                switch (dir)
                {
                    case 'N': return N ?? (rooms.TryGetValue((x, y + 1), out N) ? N : (N = new Room(x, y + 1, dir)));
                    case 'S': return S ?? (rooms.TryGetValue((x, y - 1), out S) ? S : (S = new Room(x, y - 1, dir)));
                    case 'E': return E ?? (rooms.TryGetValue((x + 1, y), out E) ? E : (E = new Room(x + 1, y, dir)));
                    case 'W': return W ?? (rooms.TryGetValue((x - 1, y), out W) ? W : (W = new Room(x - 1, y, dir)));
                    default: throw new Exception();
                }
            }
        }

    }
}
