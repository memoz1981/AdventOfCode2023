using Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day8")]
    [ApiController]
    public class _8_Day8_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data8.txt");
            var trees = new List<Tree>();
            var instruction = new List<string>();
            
            var index = 0; 
            foreach (var line in lines)
            {
                if (index < 2)
                {
                    instruction.Add(line);
                }
                if (index++ >= 2)
                    trees.Add(new Tree(line));
            }


            var treeDic = trees.ToDictionary(m => m.Name, m => m);
            var instructionsAll = $"{instruction[0]}{instruction[1]}";
            var nextTree = treeDic["AAA"];
            var count = 0;
            var array = instructionsAll.ToCharArray();

            while (true)
            {
                var el = array[count % array.Length];
                nextTree = treeDic[nextTree.GetChild(el)];
                count++;
                if (nextTree.Name == "ZZZ")
                    return Ok(count);
            }
            

            return Ok(-1);

            
        }

        [HttpGet("exercise11")]
        public IActionResult Exercise11()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data8.txt");
            var instructionList = new List<char>();

            var dictionary = new Dictionary<string, (string left, string right)>(); 
            for (int i = 0; i < lines.Count; i++)
            {
                if (i < 2)
                {
                    instructionList.AddRange(lines[i].Trim());
                }
                else
                {
                    var splitted = lines[i].Split('=');

                    var name = splitted[0].Trim();

                    var children = splitted[1].Trim();

                    var left = children.Split(',').First().Trim().Substring(1, 3);
                    var right = children.Split(',').Last().Trim().Substring(0, 3);

                    dictionary[name] = (left, right);
                }
            }

            var instructionArray = instructionList.ToArray();
            var startArray = dictionary.Keys.Where(m => m.EndsWith("A")).ToArray();

            long iteration = 0;
            var instructionLength = instructionArray.Length;
            var aArrayLength = startArray.Length;

            var builder = new StringBuilder();
            var current = startArray[5];
            builder.AppendLine($"{current} - {iteration}");
            while (iteration < 1_000_000)
            {
                var direction = instructionArray[iteration % instructionLength];
 
                if (direction == 'L')
                    current = dictionary[current].left;
                else if (direction == 'R')
                    current = dictionary[current].right;
                else
                    throw new ArgumentException();
                iteration++;
                if (current.EndsWith("Z") || current.EndsWith("A"))
                    builder.AppendLine($"{current} - {iteration}");
            }




            //while (true)
            //{
            //    var direction = instructionArray[iteration % instructionLength];
            //    for (int i = 0; i < aArrayLength; i++)
            //    {
            //        var current = startArray[i];

            //        if (direction == 'L')
            //            startArray[i] = dictionary[current].left;
            //        else if (direction == 'R')
            //            startArray[i] = dictionary[current].right;
            //        else
            //            throw new ArgumentException(); 
            //    }
            //    iteration++; 
            //    if (startArray.All(m => m.EndsWith("Z")))
            //        return Ok(iteration); 
            //}

            return Ok(LCMFinder.Main());


        }



        private (int index, string name, int count) FindNextZ(char[] array, string startingName, int startIndex, 
            Dictionary<string, Tree> treeDic, int count)
        {
            var nextTree = treeDic[startingName];
            var index = startIndex;
            var length = array.Length;
            while (true)
            {
                var el = array[index % length];
                nextTree = treeDic[nextTree.GetChild(el)];
                var name = nextTree.Name; 
                count++;
                index++; 
                if (nextTree.Name.EndsWith("Z"))
                    return (index, name, count);
            }
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data8.txt");
            var trees = new List<Tree>();
            var instruction = new List<string>();

            var index = 0;
            foreach (var line in lines)
            {
                if (index < 2)
                {
                    instruction.Add(line);
                }
                if (index++ >= 2)
                    trees.Add(new Tree(line));
            }


            var treeDic = trees.ToDictionary(m => m.Name, m => m);
            var instructionsAll = $"{instruction[0]}{instruction[1]}";
            var nextTree = treeDic["AAA"];
            var count = 1;
            foreach (var ch in instructionsAll.ToCharArray())
            {
                nextTree = treeDic[nextTree.GetChild(ch)];

                if (nextTree.Name == "ZZZ")
                    return Ok(count);
                count++;
            }

            var parent = "AAA";
            var hashSet = new Dictionary<string, int>();
            var parentTree = BuildTree(treeDic, nextTree, 1,
                instructionsAll.Count(m => m == 'L'), instructionsAll.Count(m => m == 'R'), hashSet);

            var builder = new StringBuilder();

            var min = GetDepth(parentTree, int.MaxValue);

            var result = GetPath(parentTree, new List<char>());

            var zTree = treeDic["ZZZ"];

            var prev = treeDic["FQN"];

            var treesSorted = treeDic.OrderBy(m => m.Value.Depth);

            var builder2 = new StringBuilder();

            foreach (var item in treesSorted)
            {
                builder2.AppendLine(item.ToString());
            }

            var cnt = instructionsAll.Length;

            return Ok(builder2.ToString());


        }

        public static string Build(StringBuilder builder, Tree tree)
        {
            builder.AppendLine(tree.ToString());
            builder.AppendLine(); 
            if (tree.ChildL != null)
                builder.Append(Build(builder, tree.ChildL));
            builder.Append("                ");
            if (tree.ChildR != null)
                builder.Append(Build(builder, tree.ChildR));

            return builder.ToString();
        }

        public static int GetDepth(Tree parent, int min)
        {
            if (parent.Name == "ZZZ")
                return Math.Min(min, parent.Depth);

            int minL = int.MaxValue;
            int minR = int.MaxValue;
            var arrayL = new List<char>(); 
            var arrayR = new List<char>();

            if(parent.ChildL!=null)
                minL = GetDepth(parent.ChildL, min);

            if(parent.ChildR!=null)
                minR = GetDepth(parent.ChildR, min); 

            return Math.Min(minL, minR);
        }

        public static List<char> GetPath(Tree parent, List<char> array)
        {
            if (parent.Name == "ZZZ")
                return array;

            int minL = int.MaxValue;
            int minR = int.MaxValue;
            var arrayL = array.Select(m => m).ToList();
            var arrayR = array.Select(m => m).ToList();

            if (parent.ChildL != null)
            {
                arrayL.Add('L');
                arrayL = GetPath(parent.ChildL, arrayL);
            }


            if (parent.ChildR != null)
            {
                arrayR.Add('R');
                arrayR = GetPath(parent.ChildR, arrayR);
            }
                

            if (arrayL.Count>array.Count+1)
                minL = arrayL.Count;

            if (arrayR.Count > array.Count + 1)
                minR = arrayR.Count;

            if (minL < minR)
            {

                return arrayL; 
            }

            else if (minR < minL)
            {

                return arrayR;

            }
            else if (minL != int.MaxValue)
            {
                return arrayL; 
            }

            else 
                return new List<char>(); 
        }

        
        public static Tree BuildTree(Dictionary<string, Tree> treeDic, Tree current, int depth, int lCount, int rCount, 
            Dictionary<string,int> traversed)
        {
            if (current.Name == "ZZZ")
            {
                current.Depth = depth;
                return current;
            }               

            if (traversed.ContainsKey(current.Name))
            {
                if (traversed[current.Name] < depth)
                    return null;
                traversed[current.Name] = depth;
            }
            else
            {
                traversed[current.Name] = depth;    
            }

            Tree leftTree = null;
            Tree rightTree = null;

            if (lCount > 0 && !string.IsNullOrWhiteSpace(current.ChildLeft))
            {
                lCount--;
                leftTree = BuildTree(treeDic, treeDic[current.ChildLeft], depth + 1, lCount, rCount, traversed);
                current.AddLeftTree(leftTree);
            }
            else
                current.AddLeftTree(null);
            if (rCount > 0 && !string.IsNullOrWhiteSpace(current.ChildRight))
            {
                rCount--;
                rightTree = BuildTree(treeDic, treeDic[current.ChildRight], depth + 1, lCount, rCount, traversed);
                current.AddRightTree(rightTree);
            }
            else
                current.AddRightTree(null);
            current.Depth = depth;
            return current;
        }
    }

    public class LCMFinder
    {
        static long GCD(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        static long LCM(long a, long b)
        {
            return (a * b) / GCD(a, b);
        }

        static long CalculateLCM(long[] numbers)
        {
            long lcm = 1;
            foreach (long number in numbers)
            {
                lcm = LCM(lcm, number);
            }
            return lcm;
        }

        public static long Main()
        {
            long[] numbers = { 18113, 20569, 21797, 13201, 24253, 22411 };
            return CalculateLCM(numbers);
        }
    }
    

    public class Tree
    {
        public string ChildLeft { get; set; }

        public string ChildRight { get; set; }

        public string Name { get; set; }

        public Tree ChildL { get; set; }

        public Tree ChildR { get; set; }

        public int Depth { get; set; }

        public string Next { get; set; }

        public Tree(string line)
        {
            var splitted = line.Split('=');

            Name = splitted[0].Trim();

            var children = splitted[1].Trim();

            ChildLeft = children.Split(',').First().Trim().Substring(1, 3);
            ChildRight = children.Split(',').Last().Trim().Substring(0, 3); 
        }

        public string GetChild(char dir)
        {
            if (dir == 'L')
                return ChildLeft;
            else if (dir == 'R')
                return ChildRight;

            throw new ArgumentException(); 
        }

        public Tree(string name, Tree left, Tree right, int depth)
        {
            Name = name; 
            ChildL = left;
            ChildR = right;
            ChildLeft = left?.Name ?? "";
            ChildRight = right?.Name ?? "";
            Depth = depth; 
        }

        public override string ToString()
        {
            return $"{Name};{ChildL?.Name??"N/A"};{ChildR?.Name??"N/A"};{Depth}";
        }

        public void AddLeftTree(Tree leftTree)
        {
            ChildL = leftTree;

            if (leftTree == null)
                ChildLeft = string.Empty; 
        }

        public void AddRightTree(Tree rightTree)
        {
            ChildR = rightTree;

            ChildRight = rightTree?.Name ?? string.Empty; 
        }
    }
}
