using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode
{
    public static class Misc
    {
        public static List<string> ReadLines(string inputFile, string seperator)
        {
            return new List<string>(File.ReadAllText(inputFile).Split(seperator, StringSplitOptions.RemoveEmptyEntries));
        }
    }
    public class Moon
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public int PotentialEnergy { get => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z); }
        public int KineticEnergy { get => Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) + Math.Abs(Velocity.Z); }

        public Velocity Velocity { get; set; }

        public Moon(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;

            Velocity = new Velocity(0, 0, 0);
        }

        public bool IsEqual(object obj)
        {
            Moon moon = obj as Moon;
            if (moon == null)
                return false;

            if (X != moon.X)
                return false;
            if (Y != moon.Y)
                return false;
            if (Z != moon.Z)
                return false;
            if (!Velocity.Equals(moon.Velocity))
                return false;

            return true;
        }

        public static Moon operator +(Moon moon, Velocity vel)
        {
            moon.X += vel.X;
            moon.Y += vel.Y;
            moon.Z += vel.Z;

            return moon;
        }
    }

    public class Velocity
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Velocity(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override bool Equals(object obj)
        {
            Velocity velocity = obj as Velocity;
            if (velocity == null)
                return false;

            if (X != velocity.X)
                return false;
            if (Y != velocity.Y)
                return false;
            if (Z != velocity.Z)
                return false;

            return true;
        }

        public static Velocity operator +(Velocity velocity, (int x, int y, int z) vel)
        {
            velocity.X += vel.x;
            velocity.Y += vel.y;
            velocity.Z += vel.z;

            return velocity;
        }
    }
}
