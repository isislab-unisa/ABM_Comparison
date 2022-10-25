/**************************************************************************
 *                                                                        *
 *  Website:     https://github.com/florinleon/ActressMas                 *
 *  Description: Predator-prey simulation (ants and doodlebugs) using     *
 *               ActressMas framework                                     *
 *  Copyright:   (c) 2018, Florin Leon                                    *
 *                                                                        *
 *  This program is free software; you can redistribute it and/or modify  *
 *  it under the terms of the GNU General Public License as published by  *
 *  the Free Software Foundation. This program is distributed in the      *
 *  hope that it will be useful, but WITHOUT ANY WARRANTY; without even   *
 *  the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR   *
 *  PURPOSE. See the GNU General Public License for more details.         *
 *                                                                        *
 **************************************************************************/

using System;
using System.Diagnostics;

namespace PredatorPrey
{
    public class Program
    {
        private static Random _rand = new Random();
        static readonly Stopwatch timer = new Stopwatch();

        private static void Main(string[] args)
        {
            var worldEnv = new WorldEnvironment(Settings.NoTurns); // derived from ActressMas.EnvironmentMas
            var world = worldEnv.Memory["World"];

            int noCells = Settings.GridSize * Settings.GridSize;

            int[] randVect = RandomPermutation(noCells);
            timer.Start();
            
            //for (int i = 0; i < Settings.NoAnts; i++)
            int i = 0;
            while (i < Settings.NoAnts)
            {
                var a = new AntAgent();
                
                var x = _rand.NextDouble() * Settings.GridSize;
                var y = _rand.NextDouble() * Settings.GridSize;
                a.setCoord(x, y);
                a.last_x = 0.0;
                a.last_y = 0.0;
                var xx = (int)(Math.Floor(x));
                var yy = (int)(Math.Floor(y));

                //int discretize_x = (int)Math.Floor((a.x / (10.0 / 1.5)));
                //int discretize_y = (int)Math.Floor((a.y / (10.0 / 1.5)));

                //Console.WriteLine($"{a.x} - {a.y}");
                //Console.WriteLine($"{discretize_x} - {discretize_y}");

                
                //Console.WriteLine($"xx {xx} yy {yy}");

                if (world.AddAgentToMap(a, xx, yy))
                {
                    worldEnv.Add(a, world.CreateName(a));
                    i++;
                }
            }

            worldEnv.Start();
            Console.WriteLine("time elapsed - " + timer.Elapsed.ToString());
            timer.Stop();
        }

        private static int[] RandomPermutation(int n)
        {
            // Fisher-Yates shuffle

            int[] numbers = new int[n];
            for (int i = 0; i < n; i++)
                numbers[i] = i;

            while (n > 1)
            {
                int k = _rand.Next(n--);
                int temp = numbers[n]; numbers[n] = numbers[k]; numbers[k] = temp;
            }

            return numbers;

            // much faster than Enumerable.Range(0, n).OrderBy(x => _rand.Next()).ToArray();
        }
    }
}