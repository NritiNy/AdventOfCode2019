using AdventOfCode.Intcodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public class Day2
    {
        public static void Problem1(string input)
        {
            var lines = Misc.readLines(input, Environment.NewLine);
            int[] values = new List<string>(lines[0].Split(",", StringSplitOptions.RemoveEmptyEntries)).ConvertAll((string val) => int.Parse(val)).ToArray();

            var computer = new IntcodeComputer(values);
            computer.Run();

            Console.WriteLine($"The result of problem 1 is {computer.Output}");
        }

        public static void Problem2(string input)
        {
            var lines = Misc.readLines(input, Environment.NewLine);
            int[] inputValues = new List<string>(lines[0].Split(",", StringSplitOptions.RemoveEmptyEntries)).ConvertAll((string val) => int.Parse(val)).ToArray();

            var computer = new IntcodeComputer(inputValues);

            int noun = 0;
            int verb = 0;
            bool found = false;

            for(;noun < 100 && !found; ++noun)
            {
                verb = 0;
                for(;verb < 100 && !found; ++verb)
                {
                    inputValues[1] = noun;
                    inputValues[2] = verb;

                    computer.Programm = (int[])inputValues.Clone();
                    computer.Reset();
                    computer.Run();
                    if (computer.CurrentMemoryState[0] == 19690720)
                    {
                        found = true;
                        --noun; // it is raised by one at the end of the for loop
                        --verb; // it is raised by one at the end of the for loop
                    }

                }
            }

            if (found)
                Console.WriteLine($"The result of problem 2 is {100 * noun + verb}");
            else
                Console.WriteLine("No combination could be found.");
        }
    }
}
