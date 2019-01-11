﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day08
{
    class Program
    {
        private const string inputFile = "..\\..\\..\\input8.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Day 8");
            Console.WriteLine("Star 1");
            Console.WriteLine("");

            Queue<int> values = new Queue<int>(File.ReadAllText(inputFile).Split(' ').Select(int.Parse));

            Node rootNode = new Node(values);

            int targetValue = AccumulateMetaData(rootNode);

            Console.WriteLine($"Accumulated MetaData: {targetValue}");
            Console.WriteLine("");
            Console.WriteLine("Star 2");
            Console.WriteLine("");
            Console.WriteLine($"Special Values: {rootNode.Value}");


            Console.ReadKey();
        }

        public static int AccumulateMetaData(Node node)
        {
            int value = 0;

            foreach (int val in node.metaData)
            {
                value += val;
            }

            foreach (Node child in node.children)
            {
                value += AccumulateMetaData(child);
            }

            return value;
        }

        public class Node
        {
            public Node[] children = null;
            public int[] metaData = null;

            public int Value
            {
                get
                {
                    if (children.Length == 0)
                    {
                        return metaData.Sum();
                    }

                    int temp = 0;
                    foreach (int value in metaData)
                    {
                        if (value <= children.Length)
                        {
                            temp += children[value-1].Value;
                        }
                    }

                    return temp;
                }
            }

            public Node(Queue<int> values)
            {
                children = new Node[values.Dequeue()];
                metaData = new int[values.Dequeue()];

                for (int i = 0; i < children.Length; i++)
                {
                    children[i] = new Node(values);
                }

                for (int i = 0; i < metaData.Length; i++)
                {
                    metaData[i] = values.Dequeue();
                }
            }
        }
    }
}
