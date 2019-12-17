using AdventOfCode.Intcodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public class Day11
    {
        public static void Problem1(string input)
        {
            var lines = Misc.ReadLines(input, Environment.NewLine);
            long[] values = new List<string>(lines[0].Split(",", StringSplitOptions.RemoveEmptyEntries)).ConvertAll((string val) => long.Parse(val)).ToArray();

            var computer = new IntcodeComputer(values);
            computer.InputMode = InputMode.Automatic;
            computer.OutputMode = OutputMode.Internal;

            var panels = new Dictionary<(int X, int Y), int>();
            (int X, int Y) currentPanel = (0, 0);
            (int X, int Y) newDirection = (0, 1);

            computer.Input.Add(0);
            ExitCode exitCode = computer.Run();

            while (exitCode != ExitCode.SUCCESS)
            {

                var color = (int)computer.Output[computer.Output.Count - 2];
                var dir = computer.Output.Last();

                if (dir == 0)
                    newDirection = TransformLeft(newDirection);
                else if (dir == 1)
                    newDirection = TransformRight(newDirection);

                if (panels.ContainsKey(currentPanel))
                    panels[currentPanel] = color;
                else
                    panels.Add(currentPanel, color);

                currentPanel.X += newDirection.X;
                currentPanel.Y += newDirection.Y;

                computer.Input.Add(panels.GetValueOrDefault(currentPanel, 0));

                exitCode = computer.Run();
            }

            Console.WriteLine($"the result for problem 1 is {panels.Count}");
        }

        public static void Problem2(string input)
        {
            var lines = Misc.ReadLines(input, Environment.NewLine);
            long[] values = new List<string>(lines[0].Split(",", StringSplitOptions.RemoveEmptyEntries)).ConvertAll((string val) => long.Parse(val)).ToArray();

            var computer = new IntcodeComputer(values);
            computer.InputMode = InputMode.Automatic;
            computer.OutputMode = OutputMode.Internal;

            var panels = new Dictionary<(int X, int Y), int>();
            (int X, int Y) currentPanel = (0, 0);
            (int X, int Y) newDirection = (0, 1);

            computer.Input.Add(1);
            ExitCode exitCode = computer.Run();

            while (exitCode != ExitCode.SUCCESS)
            {

                var color = (int)computer.Output[computer.Output.Count - 2];
                var dir = computer.Output.Last();

                if (dir == 0)
                    newDirection = TransformLeft(newDirection);
                else if (dir == 1)
                    newDirection = TransformRight(newDirection);

                if (panels.ContainsKey(currentPanel))
                    panels[currentPanel] = color;
                else
                    panels.Add(currentPanel, color);

                currentPanel.X += newDirection.X;
                currentPanel.Y += newDirection.Y;

                computer.Input.Add(panels.GetValueOrDefault(currentPanel, 0));

                exitCode = computer.Run();
            }
            Print(panels);
        }

        public static (int X, int Y) TransformLeft((int X, int Y) vector)
        {
            return (0 * vector.X + -1 * vector.Y, 1 * vector.X + 0 * vector.Y);
        }

        public static (int X, int Y) TransformRight((int X, int Y) vector)
        {
            return (0 * vector.X + 1 * vector.Y, -1 * vector.X + 0 * vector.Y);
        }

        public static void Print(Dictionary<(int X, int Y), int> panels)
        {
            var points = panels.Keys.ToList();

            var min_x = points.ConvertAll(p => p.X).Min();
            var max_x = points.ConvertAll(p => p.X).Max();
            var min_y = points.ConvertAll(p => p.Y).Min();
            var max_y = points.ConvertAll(p => p.Y).Max();

            for(int y=max_y;y>=min_y;--y)
            {
                for(int x=min_x;x<=max_x;++x)
                {
                    if (panels.ContainsKey((x, y)))
                    {
                        if (panels[(x, y)] == 0)
                            Console.Write(" ");
                        else
                            Console.Write("#");
                    }
                    else
                        Console.Write(" ");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine(" ===============");
            Console.WriteLine();
        }
    }
}
