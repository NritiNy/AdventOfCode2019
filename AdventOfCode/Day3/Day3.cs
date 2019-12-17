using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public class Day3
    {
        internal static Point Zero = new Point()
        {
            X = 0,
            Y = 0
        };

        internal struct Point
        {
            public int X { get; set; }
            public int Y { get; set; }

            public int ManhattenDistance(Point p)
            {
                return (Math.Abs(X - p.X) + Math.Abs(Y - p.Y));
            }
        }
        internal class PointComparer : IEqualityComparer<Point>
        {
            public bool Equals(Point p1, Point p2)
            {
                return p1.X == p2.X && p1.Y == p2.Y;
            }

            public int GetHashCode(Point obj)
            {
                return obj.GetHashCode();
            }
        }

        public static void Problem1(string input)
        {
            var lines = Misc.ReadLines(input, Environment.NewLine);

            // cable 1
            var directions1 = lines[0].Split(",", StringSplitOptions.RemoveEmptyEntries);
            List<Point> points1 = new List<Point>() { Zero };
            foreach(string dir in directions1)
            {
                points1.AddRange(getPoints(points1.Last(), dir));
            }
            points1 = points1.Distinct(new PointComparer()).ToList();
            points1.Remove(Zero);


            // cable 2
            var directions2 = lines[1].Split(",", StringSplitOptions.RemoveEmptyEntries);
            List<Point> points2 = new List<Point>() { Zero };
            foreach (string dir in directions2)
            {
                points2.AddRange(getPoints(points2.Last(), dir));
            }
            points2 = points2.Distinct(new PointComparer()).ToList();
            points2.Remove(Zero);

            // crossing points
            var cross = points1.Intersect(points2).ToList();
            int minimumDistance = cross.ConvertAll((Point p) => p.ManhattenDistance(Zero)).Min();

            Console.WriteLine($"The result for problem 1 is {minimumDistance}.");
        }

        public static void Problem2(string input)
        {
            var lines = Misc.ReadLines(input, Environment.NewLine);

            // cable 1
            var directions1 = lines[0].Split(",", StringSplitOptions.RemoveEmptyEntries);
            List<Point> points1 = new List<Point>() { Zero };
            foreach (string dir in directions1)
            {
                points1.AddRange(getPoints(points1.Last(), dir));
            }


            // cable 2
            var directions2 = lines[1].Split(",", StringSplitOptions.RemoveEmptyEntries);
            List<Point> points2 = new List<Point>() { Zero };
            foreach (string dir in directions2)
            {
                points2.AddRange(getPoints(points2.Last(), dir));
            }

            // crossing points
            var cross = points1.Intersect(points2).ToList();
            cross.Remove(Zero);
            int minimumDistance = cross.ConvertAll((Point p) => points1.IndexOf(p) + points2.IndexOf(p)).Min();

            Console.WriteLine($"The result for problem 2 is {minimumDistance}.");
        }

        internal static List<Point> getPoints(Point start, string direction)
        {
            var ret = new List<Point>();

            var dir = direction.ToLower()[0];
            var amount = int.Parse(direction.Substring(1));

            switch(dir)
            {
                case 'r':
                    for(int x = 1;x <= amount;++x)
                    {
                        ret.Add(new Point()
                        {
                            X = x + start.X,
                            Y = start.Y
                        });
                    }
                    break;
                case 'l':
                    for (int x = 1; x <= amount; ++x)
                    {
                        ret.Add(new Point()
                        {
                            X = start.X - x,
                            Y = start.Y
                        });
                    }
                    break;
                case 'u':
                    for (int Y = 1; Y <= amount; ++Y)
                    {
                        ret.Add(new Point()
                        {
                            Y = start.Y + Y,
                            X = start.X
                        });
                    }
                    break;
                case 'd':
                    for (int y = 1; y <= amount; ++y)
                    {
                        ret.Add(new Point()
                        {
                            Y = start.Y - y,
                            X = start.X
                        });
                    }
                    break;
                default:
                    Console.WriteLine($"Direction {dir} is not supported.");
                    break;
            }

            return ret;
        }
    }
}
