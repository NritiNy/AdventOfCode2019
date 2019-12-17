using AdventOfCode.Intcodes;
using System;
using System.Collections.Generic;

namespace AdventOfCode
{
    public class Day5
    {
        public static void Problem1(string input)
        {
            var lines = Misc.ReadLines(input, Environment.NewLine);
            long[] values = new List<string>(lines[0].Split(",", StringSplitOptions.RemoveEmptyEntries)).ConvertAll((string val) => long.Parse(val)).ToArray();

            var computer = new IntcodeComputer(values);
            computer.Run();
        }

        public static void Problem2(string input)
        {
            var lines = Misc.ReadLines(input, Environment.NewLine);
            long[] values = new List<string>(lines[0].Split(",", StringSplitOptions.RemoveEmptyEntries)).ConvertAll((string val) => long.Parse(val)).ToArray();

            var computer = new IntcodeComputer(values);
            computer.Run();
        }
    }
}
