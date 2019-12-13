﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public class Day1
    {
        public static Day1 getInstance => new Day1();

        private Day1()
        {
        }

        public void Problem1(string input)
        {
            var lines = Misc.readLines(input, Environment.NewLine);
            var integers = lines.ConvertAll((string line) => int.Parse(line));

            var sum = DivideBy3minus2(integers).Sum();
            Console.WriteLine($"The result of problem 1 is {sum}.");
        }

        public void Problem2(string input)
        {
            var lines = Misc.readLines(input, Environment.NewLine);
            var integers = lines.ConvertAll((string line) => int.Parse(line));

            int sum = 0;
            do
            {
                integers = DivideBy3minus2(integers);
                sum += integers.Sum();
            } while (integers.Count > 0);

            Console.WriteLine($"The result of problem 2 is {sum}.");
        }

        private List<int> DivideBy3minus2(List<int> inputValues)
        {
            var converted = inputValues.ConvertAll((int value) => value / 3 - 2);
            converted.RemoveAll((int value) => value <= 0);
            return converted;
        }

    }
}
