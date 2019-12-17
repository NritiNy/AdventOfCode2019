using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public class Day10
    {
        public static void Problem1(string input)
        {
            var lines = Misc.ReadLines(input, Environment.NewLine);

            var asteroids = new List<(int X, int Y)>();

            for (int y = 0; y < lines.Count; ++y)
            {
                var points = lines[y].ToList();
                for (int x = 0; x < points.Count; ++x)
                {
                    if (points[x] == '#')
                        asteroids.Add((x, y));
                }
            }

            var max = DetectAsteroids(asteroids).Values.ToList().ConvertAll(direction => direction.Count).Max();

            Console.WriteLine($"The result for problem 1 is {max}.");
        }

        public static void Problem2(string input)
        {
            var lines = Misc.ReadLines(input, Environment.NewLine);

            var asteroids = new List<(int X, int Y)>();

            for (int y = 0; y < lines.Count; ++y)
            {
                var points = lines[y].ToList();
                for (int x = 0; x < points.Count; ++x)
                {
                    if (points[x] == '#')
                        asteroids.Add((x, y));
                }
            }

            var directions = DetectAsteroids(asteroids);
            var dirCounts = directions.ToDictionary(kp => kp.Key, kp => kp.Value.Count);
            var location = dirCounts.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

            var locDirs = directions[location].ToDictionary(dir => dir, dir => Math.Acos((double)((dir.X * 0 + dir.Y * -1) / (1 * 1))));
            locDirs = (from entry in locDirs orderby entry.Value ascending select entry).ToDictionary(entry => entry.Key, entry => entry.Value);

            while(asteroids.Count > 0)
            {
                var dirs = new Dictionary<(int X, int Y), HashSet<(decimal X, decimal Y)>>();
                dirs.Add(location, new HashSet<(decimal X, decimal Y)>());
                foreach (var ast in asteroids)
                {
                    if (ast.X == location.X && ast.Y == location.Y)
                        continue;

                    var x = ast.X - location.X;
                    var y = ast.Y - location.Y;

                    directions[location].Add(Normalize((x, y)));
                }
            }

            Console.WriteLine(locDirs.Count);
            Console.WriteLine(string.Join(", ", locDirs));
        }

        public static Dictionary<(int X, int Y), HashSet<(decimal X, decimal Y)>> DetectAsteroids(List<(int X, int Y)> asteroids)
        {
            var directions = new Dictionary<(int X, int Y), HashSet<(decimal X, decimal Y)>>();
            foreach (var asteroid in asteroids)
            {
                directions.Add(asteroid, new HashSet<(decimal X, decimal Y)>());
                foreach (var ast in asteroids)
                {
                    if (ast.X == asteroid.X && ast.Y == asteroid.Y)
                        continue;

                    var x = ast.X - asteroid.X;
                    var y = ast.Y - asteroid.Y;

                    directions[asteroid].Add(Normalize((x, y)));
                }
            }
            return directions;
        }

        public static (decimal X, decimal Y) Normalize((int X, int Y) vector)
        {
            var x = (decimal)(vector.X / Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y));
            var y = (decimal)(vector.Y / Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y));

            return (x, y);
        }

        public class AngleComparer : IComparer<(decimal X, decimal Y)>
        {
            public int Compare((decimal X, decimal Y) x, (decimal X, decimal Y) y)
            {
                //Console.WriteLine($"Angle for {x.X}, {x.Y}: {Math.Acos((double)((x.X * 1 + x.Y * 0) / (1 * 1)))}");
                return Math.Acos((double)((x.X * -1 + x.Y * 0)/(1 * 1))).CompareTo(0);
            }
        }
    }
}
