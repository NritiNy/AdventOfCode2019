using System;

namespace AdventOfCode
{
    public class Day4
    {
        public static void Problem1(string input)
        {
            var lines = Misc.ReadLines(input, Environment.NewLine);

            int start = int.Parse(lines[0].Split("-")[0]);
            int end = int.Parse(lines[0].Split("-")[1]);

            int counter = 0;

            for(int i = start; i <= end;++i)
            {
                bool doubleFound = false;
                bool increasing = true;

                string number = "" + i;
                for(int j = 0; j < number.Length-1; ++j)
                {
                    doubleFound = doubleFound || (number[j] == number[j+1]);
                    increasing = increasing && (int.Parse(""+number[j]) <= int.Parse(""+number[j + 1]));
                }
                if (doubleFound && increasing && number.Length == 6)
                    ++counter;
            }

            Console.WriteLine($"The result for problem 1 is {counter}.");
        }

        public static void Problem2(string input)
        {
            var lines = Misc.ReadLines(input, Environment.NewLine);

            int start = int.Parse(lines[0].Split("-")[0]);
            int end = int.Parse(lines[0].Split("-")[1]);

            int counter = 0;

            for (int i = start; i <= end; ++i)
            {
                bool doubleFound = false;
                bool increasing = true;

                string number = "" + i;
                for (int j = 0; j < number.Length-1; ++j)
                {               
                    increasing = increasing && (int.Parse("" + number[j]) <= int.Parse("" + number[j + 1]));
                    doubleFound = doubleFound || ((number[j] == number[j + 1]) && !number.Contains(""+number[j]+ number[j]+ number[j]));
                }
                if (doubleFound && increasing && number.Length == 6)
                    ++counter;
            }

            Console.WriteLine($"The result for problem 2 is {counter}.");
        }
    }
}
