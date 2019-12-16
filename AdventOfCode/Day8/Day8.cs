using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public class Day8
    {
        public static void Problem1(string input)
        {
            var lines = Misc.readLines(input, Environment.NewLine);

            var pixels = lines[0].ToList().ConvertAll(c => int.Parse("" + c));

            var pixelCounterPerLayer = new Dictionary<int, Dictionary<int, int>>();
            for(int i=0;i<pixels.Count;++i)
            {
                int pixel = pixels[i];
                int currentLayer = i / (25 * 6);

                if (!pixelCounterPerLayer.ContainsKey(currentLayer))
                    pixelCounterPerLayer.Add(currentLayer, new Dictionary<int, int>());

                if (!pixelCounterPerLayer[currentLayer].ContainsKey(pixel))
                    pixelCounterPerLayer[currentLayer].Add(pixel, 0);
                ++pixelCounterPerLayer[currentLayer][pixel];
            }

            int layer = -1;
            int min = int.MaxValue;
            foreach (var l in pixelCounterPerLayer.Keys)
            {
                if (!pixelCounterPerLayer[l].ContainsKey(0))
                    continue;

                int m = pixelCounterPerLayer[l][0];
                if (m < min)
                {
                    Console.WriteLine($"Setting max to {m} in layer {l}");
                    min = m;
                    layer = l;
                }
            }

            Console.WriteLine($"The result for problem 1 is {pixelCounterPerLayer[layer][1] * pixelCounterPerLayer[layer][2]}");
        }

        public static void Problem2(string input)
        {
            var lines = Misc.readLines(input, Environment.NewLine);
            var pixels = lines[0].ToList().ConvertAll(c => int.Parse("" + c));

            int[,] image = new int[25, 6];
            for (int i = 0; i < 6; ++i)
            {
                for (int j = 0; j < 25; ++j)
                {
                    image[j, i] = 2;
                }
            }

            for (int i = 0; i < pixels.Count; ++i)
            {
                int pixel = pixels[i];

                int x = (i % (25 * 6)) % 25;
                int y = (i % (25 * 6)) / 25;

                if (image[x, y] == 2)
                    image[x, y] = pixel;
            }

            for(int i=0;i<6;++i)
            {
                for(int j = 0;j<25;++j)
                {
                    Console.Write(image[j, i] == 0 ? " " : "#");
                }
                Console.WriteLine();
            }
        }
    }
}
