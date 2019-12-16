using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public class Day6
    {
        public static void Problem1(string input)
        {
            var lines = Misc.readLines(input, Environment.NewLine);

            Dictionary<string, TreeNode> objects = new Dictionary<string, TreeNode>();
            Tree tree = null;
            foreach(string line in lines)
            {
                var parent = line.Split(")")[0];
                var child = line.Split(")")[1];

                if(!objects.ContainsKey(parent))
                    objects.Add(parent, new TreeNode(null, parent));
                if(!objects.ContainsKey(child))
                    objects.Add(child, new TreeNode(null, child));

                objects[parent].Children.Add(objects[child]);
                objects[child].Parent = objects[parent];

                if (parent == "COM")
                    tree = new Tree(objects[parent]);
            }

            int sum = 0;
            foreach(string o in objects.Keys)
            {
                var steps = tree.FindNode(tree.Root, o).Steps;
                sum += steps;
            }
            Console.WriteLine($"The result for problem 1 is {sum}.");
        }

        public static void Problem2(string input)
        {
            var lines = Misc.readLines(input, Environment.NewLine);

            Dictionary<string, TreeNode> objects = new Dictionary<string, TreeNode>();
            Tree tree = null;
            foreach (string line in lines)
            {
                var parent = line.Split(")")[0];
                var child = line.Split(")")[1];

                if (!objects.ContainsKey(parent))
                    objects.Add(parent, new TreeNode(null, parent));
                if (!objects.ContainsKey(child))
                    objects.Add(child, new TreeNode(null, child));

                objects[parent].Children.Add(objects[child]);
                objects[child].Parent = objects[parent];

                if (parent == "COM")
                    tree = new Tree(objects[parent]);
            }

            List<TreeNode> mine = new List<TreeNode>();
            List<TreeNode> santas = new List<TreeNode>();

            TreeNode node = tree.FindNode(tree.Root, "SAN").Node;
            while(node != null)
            {
                node = node.Parent;
                if(node != null)
                    santas.Add(node);
            }

            node = tree.FindNode(tree.Root, "YOU").Node;
            while (node != null)
            {
                node = node.Parent;
                if (node != null)
                    mine.Add(node);
            }

            var same = mine.Intersect(santas).ToList().ConvertAll(n => tree.FindNode(tree.Root, n.Data));
            same.Sort((n1, n2) => n1.Steps.CompareTo(n2.Steps));

            var baseObject = same.Last().Node;
            var steps = tree.FindNode(baseObject, "YOU").Steps + tree.FindNode(baseObject, "SAN").Steps;

            Console.WriteLine($"The result for problem 2 is {steps-2}.");
        }

        public class Tree
        {
            public TreeNode Root { get; private set; } = null;

            public Tree(TreeNode root)
            {
                Root = root;
            }

            public void AddChild(string parent, string data)
            {
                Console.WriteLine($"Adding {data} to {parent}");
                var n = FindNode(Root, parent).Node;
                n.Children.Add(new TreeNode(n, data));
            }

            public (TreeNode Node, int Steps) FindNode(TreeNode start, string name, int steps = 0)
            {
                if (start.Data.Trim() == name.Trim())
                    return (start, steps);
                else
                {
                    ++steps;
                    foreach(TreeNode node in start.Children)
                    {
                        //Console.WriteLine($"searching for {name}, starting from {start.Data}, with current steps: {steps}");
                        //Console.WriteLine($"Current node: {node.Data}");
                        var result = FindNode(node, name, steps);
                        if(result.Node != null)
                            return result;
                    }
                }
                return (null, 0);
            }
        }
        public class TreeNode
        {
            public TreeNode Parent { get; set; }

            public string Data { get; private set; }
            public List<TreeNode> Children { get; } = new List<TreeNode>();

            public TreeNode(TreeNode parent,  string data)
            {
                Parent = parent;
                Data = data;
            }
        }
    }
}
