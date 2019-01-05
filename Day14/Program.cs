using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day14
{
    class Program
    {
        //What are the 10 recipes after this:
        private const int INPUT = 939601;
        private const int TARGET = INPUT + 10;

        static void Main(string[] args)
        {
            Console.WriteLine("Day 14");
            Console.WriteLine("Star 1");
            Console.WriteLine("");

            //New recipes are made by adding together the current recipe values.
            //The recipes are values 0-9
            //If the sum is greater than 9, then 2 recipes are created and appended to the list

            //Then each elf walks forward 1 + their current recipe value

            //The starting configuration is (3)[7]

            List<int> recipes = new List<int>(TARGET + 2) { 3, 7 };

            //Elf indices
            int elfA = 0;
            int elfB = 1;

            while(recipes.Count < TARGET)
            {
                int newVal = recipes[elfA] + recipes[elfB];
                if (newVal > 9)
                {
                    recipes.Add(1);
                    recipes.Add(newVal % 10);
                }
                else
                {
                    recipes.Add(newVal);
                }

                elfA += recipes[elfA] + 1;
                elfB += recipes[elfB] + 1;

                elfA = elfA % recipes.Count;
                elfB = elfB % recipes.Count;
            }

            Console.WriteLine($"10 recipes after {INPUT}: {string.Join("", recipes.GetRange(INPUT, 10).Select(x => x.ToString()))}");

            Console.ReadKey();

        }
    }
}
