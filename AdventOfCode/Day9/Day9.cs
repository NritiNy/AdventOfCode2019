﻿using AdventOfCode.Intcodes;
using System;
using System.Collections.Generic;

namespace AdventOfCode
{
    public class Day9
    {
        public static void Problem1(string input)
        {
            var lines = Misc.readLines(input, Environment.NewLine);
            int[] values = new List<string>(lines[0].Split(",", StringSplitOptions.RemoveEmptyEntries)).ConvertAll((string val) => int.Parse(val)).ToArray();

            var computer = new IntcodeComputer(values);
        }
    }
}