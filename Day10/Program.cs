using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day10
{

    class Program
    {
        private const string inputFile = "..\\..\\..\\input10.txt";

        static void Main(string[] args)
        {
            List<Character> characters = new List<Character>();


            using (FileStream stream = File.OpenRead(inputFile))
            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    characters.Add(new Character(reader.ReadLine()));
                }
            }

            Console.SetBufferSize(301, 301);

            bool ready = false;

            while (!ready)
            {
                foreach (Character c in characters)
                {
                    c.Progress();
                    if (c.x > -150 && c.x < 150 && c.y > -150 && c.y < 150)
                    {
                        ready = true;
                    }
                }
            }


            while (true)
            {
                Console.Clear();

                foreach (Character c in characters)
                {
                    c.Progress();
                    
                    if (c.x >= -150 && c.x + 150 < Console.BufferWidth && c.y <= -150 && 150 - c.y < 300)
                    {
                        Console.SetCursorPosition(c.x + 150, 150 - c.y);
                        Console.Write('#');
                    }
                }


                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    break;
                }

            }



            Console.ReadKey();
        }

        class Character
        {
            public int x;
            public int y;
            public int vx;
            public int vy;

            public Character(string line)
            {
                x = int.Parse(line.Substring(10, 6));
                y = int.Parse(line.Substring(18, 6));

                vx = int.Parse(line.Substring(36, 2));
                vy = int.Parse(line.Substring(39, 3));

                x += 10000 * vx;
                y += 10000 * vy;
            }

            public void Progress()
            {
                x += vx;
                y += vy;
            }
        }
    }
}
