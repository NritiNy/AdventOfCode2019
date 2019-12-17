using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public static class Misc
    {
        public static List<string> ReadLines(string inputFile, string seperator)
        {
            return new List<string>(File.ReadAllText(inputFile).Split(seperator, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
