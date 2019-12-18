using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public class Day12
    {
        public static void Problem1(string input)
        {
            var lines = Misc.ReadLines(input, Environment.NewLine);

            var moons = new List<Moon>();
            foreach (string l in lines)
            {
                var line = l.Substring(1, l.Length - 2);
                var x = int.Parse(line.Split(",")[0].Split("=")[1]);
                var y = int.Parse(line.Split(",")[1].Split("=")[1]);
                var z = int.Parse(line.Split(",")[2].Split("=")[1]);

                moons.Add(new Moon(x, y, z));
            }


            for (int i = 0; i < 1000; ++i)
            {
                var gravity = new Dictionary<Moon, (int X, int Y, int Z)>();
                for (int j = 0; j < moons.Count - 1; ++j)
                {
                    var moon = moons[j];
                    if (!gravity.ContainsKey(moon))
                        gravity.Add(moon, (0, 0, 0));

                    for (int k = j + 1; k < moons.Count; ++k)
                    {
                        var moon2 = moons[k];
                        if (!gravity.ContainsKey(moon2))
                            gravity.Add(moon2, (0, 0, 0));

                        var x1 = -1 * moon.X.CompareTo(moon2.X);
                        var y1 = -1 * moon.Y.CompareTo(moon2.Y);
                        var z1 = -1 * moon.Z.CompareTo(moon2.Z);

                        var x2 = -1 * moon2.X.CompareTo(moon.X);
                        var y2 = -1 * moon2.Y.CompareTo(moon.Y);
                        var z2 = -1 * moon2.Z.CompareTo(moon.Z);

                        gravity[moon] = (gravity[moon].X + x1, gravity[moon].Y + y1, gravity[moon].Z + z1);
                        gravity[moon2] = (gravity[moon2].X + x2, gravity[moon2].Y + y2, gravity[moon2].Z + z2);
                    }
                }

                for (int l = 0; l < moons.Count; ++l)
                {
                    var moon = moons[l];
                    moon.Velocity += gravity[moon];
                    moon.X += moon.Velocity.X;
                    moon.Y += moon.Velocity.Y;
                    moon.Z += moon.Velocity.Z;
                }

                if ((i + 1) % 1000 == 0)
                {
                    Console.WriteLine($"After {i + 1} steps:");
                    moons.ForEach(moon => Console.WriteLine($"pos<x={moon.X}, y={moon.Y}, z={moon.Z}>, vel=<x={moon.Velocity.X}, y={moon.Velocity.Y}, z={moon.Velocity.Z} >"));
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }

            var sum = 0;
            foreach (var moon in moons)
                sum += moon.PotentialEnergy * moon.KineticEnergy;

            Console.WriteLine($"the result for problem 1 is {sum}.");
        }

        public static void Problem2(string input)
        {
            var lines = Misc.ReadLines(input, Environment.NewLine);

            var moons = new Dictionary<Moon, (bool repeated, int count)>();
            var initialState = new List<Moon>();
            foreach (string l in lines)
            {
                var line = l.Substring(1, l.Length - 2);
                var x = int.Parse(line.Split(",")[0].Split("=")[1]);
                var y = int.Parse(line.Split(",")[1].Split("=")[1]);
                var z = int.Parse(line.Split(",")[2].Split("=")[1]);

                moons.Add(new Moon(x, y, z), (false, 0));
                initialState.Add(new Moon(x, y, z));
            }

            bool repeat = false;
            for (int i=0; !repeat;++i)
            {
                var gravity = new Dictionary<Moon, (int X, int Y, int Z)>();
                for (int j = 0; j < moons.Count - 1; ++j)
                {
                    var moon = moons.Keys.ToList()[j];
                    if (!gravity.ContainsKey(moon))
                        gravity.Add(moon, (0, 0, 0));

                    for (int k = j + 1; k < moons.Count; ++k)
                    {
                        var moon2 = moons.Keys.ToList()[k];
                        if (!gravity.ContainsKey(moon2))
                            gravity.Add(moon2, (0, 0, 0));

                        var x1 = -1 * moon.X.CompareTo(moon2.X);
                        var y1 = -1 * moon.Y.CompareTo(moon2.Y);
                        var z1 = -1 * moon.Z.CompareTo(moon2.Z);

                        var x2 = -1 * moon2.X.CompareTo(moon.X);
                        var y2 = -1 * moon2.Y.CompareTo(moon.Y);
                        var z2 = -1 * moon2.Z.CompareTo(moon.Z);

                        gravity[moon] = (gravity[moon].X + x1, gravity[moon].Y + y1, gravity[moon].Z + z1);
                        gravity[moon2] = (gravity[moon2].X + x2, gravity[moon2].Y + y2, gravity[moon2].Z + z2);
                    }
                }

                for (int l = 0; l < moons.Count; ++l)
                {
                    var moon = moons.Keys.ToList()[l];
                    moon.Velocity += gravity[moon];
                    moon.X += moon.Velocity.X;
                    moon.Y += moon.Velocity.Y;
                    moon.Z += moon.Velocity.Z;

                    bool r = moons[moon].repeated || moon.IsEqual(initialState[l]);
                    int count = moons[moon].count + (r ? 0 : 1);
                    moons[moon] = (r, count);
                }

            }
        }
    }
}
