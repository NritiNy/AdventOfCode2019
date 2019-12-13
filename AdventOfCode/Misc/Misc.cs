using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode
{
    public class Misc
    {
        public static List<string> readLines(string inputFile, string seperator)
        {
            return new List<string>(File.ReadAllText(inputFile).Split(seperator, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
