using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day13
{
    class Program
    {
        private const string inputFile = "..\\..\\..\\input13.txt";

        private const char Vert = '║';
        private const char Horiz = '═';
        private const char LL = '╚';
        private const char UL = '╔';
        private const char LR = '╝';
        private const char UR = '╗';
        private const char Int = '╬';

        static void Main(string[] args)
        {
            Console.WriteLine("Day 13");
            Console.WriteLine("Star 1");
            Console.WriteLine("");

            string[] file = System.IO.File.ReadAllLines(inputFile);
            char[,] grid = new char[file[0].Length, file.Length];

            List<Cart> carts = new List<Cart>();

            for (int y = 0; y < file.Length; y++)
            {
                string line = file[y];
                for (int x = 0; x < line.Length; x++)
                {
                    switch (line[x])
                    {
                        case ' ':
                            grid[x, y] = ' ';
                            break;
                        case '/':
                            if (x == 0 || (grid[x - 1, y] == ' ' || grid[x - 1, y] == Vert))
                            {
                                grid[x, y] = UL;
                            }
                            else
                            {
                                grid[x, y] = LR;
                            }
                            break;
                        case '\\':
                            if (x == 0 || (grid[x - 1, y] == ' ' || grid[x - 1, y] == Vert))
                            {
                                grid[x, y] = LL;
                            }
                            else
                            {
                                grid[x, y] = UR;
                            }
                            break;
                        case '|':
                            grid[x, y] = Vert;
                            break;
                        case '-':
                            grid[x, y] = Horiz;
                            break;
                        case '+':
                            grid[x, y] = Int;
                            break;
                        case '^':
                        case 'v':
                            carts.Add(new Cart(x, y, line[x]));
                            grid[x, y] = Vert;
                            break;
                        case '<':
                        case '>':
                            carts.Add(new Cart(x, y, line[x]));
                            grid[x, y] = Horiz;
                            break;
                        default:
                            throw new Exception($"Unimplemented character {line[x]}");
                    }
                }
            }


            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    Console.Write(grid[x, y]);
                }
                Console.Write('\n');
            }



            Console.ReadKey();
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            MAX
        }


        public class Cart
        {
            public Direction direction;
            public int x;
            public int y;

            //Cycles Left(0), Straight(1), Right(2)
            public int cycle = 0;

            public Cart(int x, int y, char cart)
            {
                this.x = x;
                this.y = y;

                switch (cart)
                {
                    case '^':
                        direction = Direction.Up;
                        break;

                    case 'v':
                        direction = Direction.Down;
                        break;

                    case '<':
                        direction = Direction.Left;
                        break;

                    case '>':
                        direction = Direction.Right;
                        break;

                    default:
                        throw new Exception($"Unimplemented character {cart}");
                }
            }

            public void Progress(char[,] grid)
            {

            }
        }
    }
}
