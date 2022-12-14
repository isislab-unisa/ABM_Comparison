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
using ActressMas;

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

            for (int i = 0; i < Settings.NoDoodlebugs; i++)
            {
                int e = _rand.Next(20);
                var a = new DoodlebugAgent(e);
                
                worldEnv.Add(a, world.CreateName(a)); // unique name
                world.AddAgentToMap(a, randVect[i]);
            }

            for (int i = Settings.NoDoodlebugs; i < Settings.NoDoodlebugs + Settings.NoAnts; i++)
            {
                int e = _rand.Next(8);
                var a = new Sheep(e);
                worldEnv.Add(a, world.CreateName(a));
                world.AddAgentToMap(a, randVect[i]);
                
            }

            //worldEnv.Start();
            int turn = 0;

            while (true)
            {
                worldEnv.RunTurn(turn);
                world.UpdateGrass();
                turn++;
                if ( turn >= 200)
                    break;
                
            }

            worldEnv.SimulationFinished();
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