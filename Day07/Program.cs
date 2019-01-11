using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day07
{
    class Program
    {
        private const string inputFile = "..\\..\\..\\input7.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Day 7");
            Console.WriteLine("Star 1");
            Console.WriteLine("");

            Dictionary<char, Node> nodes = new Dictionary<char, Node>();

            foreach (string line in File.ReadAllLines(inputFile))
            {
                char parent = line[5];
                char child = line[36];

                if (!nodes.ContainsKey(parent))
                {
                    nodes.Add(parent, new Node(parent));
                }

                if (!nodes.ContainsKey(child))
                {
                    nodes.Add(child, new Node(child));
                }

                nodes[parent].children.Add(nodes[child]);
                nodes[child].parents.Add(nodes[parent]);
            }

            string output = "";

            while (nodes.Values.Count != 0)
            {
                Node choice = null;

                foreach (Node node in nodes.Values)
                {
                    if (node.parents.Count == 0 && (choice == null || node.name < choice.name))
                    {
                        choice = node;
                    }
                }

                output += choice.name;

                foreach (Node child in choice.children)
                {
                    child.parents.Remove(choice);
                }
                nodes.Remove(choice.name);
            }


            Console.WriteLine($"Order: {output}");
            Console.WriteLine("");

            Console.WriteLine("Star 2");
            Console.WriteLine("");

            nodes.Clear();


            foreach (string line in File.ReadAllLines(inputFile))
            {
                char parent = line[5];
                char child = line[36];

                if (!nodes.ContainsKey(parent))
                {
                    nodes.Add(parent, new Node(parent));
                }

                if (!nodes.ContainsKey(child))
                {
                    nodes.Add(child, new Node(child));
                }

                nodes[parent].children.Add(nodes[child]);
                nodes[child].parents.Add(nodes[parent]);
            }

            int time = 0;

            List<ProcessingNode> processing = new List<ProcessingNode>(5)
            {
                new ProcessingNode(),
                new ProcessingNode(),
                new ProcessingNode(),
                new ProcessingNode(),
                new ProcessingNode()
            };
            
            while (nodes.Values.Count != 0 || !processing.TrueForAll(x=>x.Ready))
            {
                //Step 1: Queue up new tasks
                for (int i = 0; i < 5; i++)
                {
                    if (processing[i].Ready)
                    {
                        processing[i].WithdrawNode(nodes);
                    }
                }


                //Step 4: Increment Time
                time++;
                foreach (ProcessingNode node in processing)
                {
                    node.Progress();
                }
            }

            Console.WriteLine($"Time: {time}");
            Console.WriteLine("");

            Console.ReadKey();
        }

        public class Node
        {
            public char name;
            public List<Node> children = new List<Node>();
            public List<Node> parents = new List<Node>();

            public Node(char name)
            {
                this.name = name;
            }
        }

        public class ProcessingNode
        {
            public int remainingTime;

            public Node originalNode = null;

            public bool Ready => originalNode == null;
            

            public void WithdrawNode(Dictionary<char, Node> nodes)
            {
                foreach (Node node in nodes.Values)
                {
                    if (node.parents.Count == 0 && (originalNode == null || node.name < originalNode.name))
                    {
                        originalNode = node;
                    }
                }

                if (originalNode != null)
                {
                    nodes.Remove(originalNode.name);
                    remainingTime = 61 + (originalNode.name - 'A');
                }
            }

            public void Progress()
            {
                if (originalNode != null)
                {
                    remainingTime--;

                    if (remainingTime == 0)
                    {
                        foreach (Node child in originalNode.children)
                        {
                            child.parents.Remove(originalNode);
                        }
                        originalNode = null;
                    }
                }
            }
        }
    }
}
