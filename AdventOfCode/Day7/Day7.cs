using AdventOfCode.Intcodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public class Day7
    {
        public static void Problem1(string input)
        {
            var lines = Misc.ReadLines(input, Environment.NewLine);
            long[] values = new List<string>(lines[0].Split(",", StringSplitOptions.RemoveEmptyEntries)).ConvertAll((string val) => long.Parse(val)).ToArray();

            var computer = new IntcodeComputer(values);
            computer.InputMode = InputMode.Automatic;
            computer.OutputMode = OutputMode.Internal;

            var combinations = GetPermutations(new List<int>() { 0, 1, 2, 3, 4 });
            long max = -1;
            foreach (var combi in combinations)
            {
                long output = 0;
                foreach (int i in combi)
                {
                    computer.Programm = values;
                    computer.Reset();
                    computer.Input = new List<long>() { i, output };
                    computer.Run();

                    output = computer.Output[0];
                }

                if (output > max)
                    max = output;
            }

            Console.WriteLine($"The result for problem 1 is {max}.");
        }

        public static List<int[]> GetPermutations(List<int> values)
        {
            if (values.Count == 1)
                return new List<int[]> { new int[1] { values[0] } };

            List<int[]> ret = new List<int[]>();
            for (int i = 0; i < values.Count; ++i)
            {
                int val = values[i];
                foreach (var combi in GetPermutations(values.Except(new int[] { val }).ToList()))
                {
                    int[] newCombo = new int[combi.Length + 1];
                    newCombo[0] = val;
                    Array.Copy(combi, 0, newCombo, 1, combi.Length);
                    ret.Add(newCombo);
                }
            }
            return ret;
        }

        public static void Problem2(string input)
        {
            var lines = Misc.ReadLines(input, Environment.NewLine);
            long[] values = new List<string>(lines[0].Split(",", StringSplitOptions.RemoveEmptyEntries)).ConvertAll((string val) => long.Parse(val)).ToArray();

            var combinations = GetPermutations(new List<int>() { 5, 6, 7, 8, 9 });
            long max = -1;

            foreach (var combi in combinations)
            {
                List<IntcodeComputer> computers = new List<IntcodeComputer>();
                for (int i = 0; i < 5; ++i)
                {
                    var computer = new IntcodeComputer(values);
                    computer.InputMode = InputMode.Automatic;
                    computer.OutputMode = OutputMode.Internal;
                    computer.Input.Add(combi[i]);

                    computers.Add(computer);
                }

                long output = 0;
                Queue<IntcodeComputer> pcs = new Queue<IntcodeComputer>(computers);

                while (pcs.Count > 0)
                {
                    var computer = pcs.Dequeue();

                    computer.Input.Add(output);
                    var code = computer.Run();

                    output = computer.Output.Last();

                    if (code == ExitCode.ERROR)
                        pcs.Enqueue(computer);
                }

                if (output > max)
                    max = output;
            }

            Console.WriteLine($"The result for problem 2 is {max}.");
        }
    }
}
