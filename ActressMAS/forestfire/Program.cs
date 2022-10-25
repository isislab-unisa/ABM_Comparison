/**************************************************************************
 *                                                                        *
 *  Description: Simple example of using the ActressMas framework         *
 *  Website:     https://github.com/florinleon/ActressMas                 *
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

using ActressMas;
using Agents2;
using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;

namespace Agents2
{
    public enum TreeState { Empty, Green, Burning, Burned };

    public class Cell
    {
        public TreeState State;

        public Cell()
        {
            State = TreeState.Empty;
        }
    }

    public class Program
    {
        static readonly Stopwatch timer = new Stopwatch();

        private static void Main(string[] args)
        {
            var env = new EnvironmentMas(noTurns: Settings.NoTurns);
            var a1 = new MyAgent(); env.Add(a1, "fire");
            timer.Start();

            env.Start();
            Console.WriteLine("time elapsed - " + timer.Elapsed.ToString());
            timer.Stop();
        }
    }

    public class MyAgent : Agent
    {
        private static Random _rand = new Random();
        private Cell[,] _map;
        private int turn = 0;
        public override void Setup()
        {
            Console.WriteLine($"{Name} starting");
            _map = new Cell[Settings.GridSize, Settings.GridSize];
            for (int i = 0; i < Settings.GridSize; i++)
                for (int j = 0; j < Settings.GridSize; j++) {
                    _map[i, j] = new Cell();
                    double r = _rand.NextDouble();
                    if (r < Settings.density) {
                        if (j == 0) { _map[i, j].State = TreeState.Burning; }
                        else _map[i, j].State = TreeState.Green;
                    }
                }
              
            Console.WriteLine("end startup");

        }

        public override void ActDefault()
        {

            {
               
                
                for (int i = 0; i < Settings.GridSize; i++)
                {

                    int minmax = turn + 1 < Settings.GridSize - 1 ? turn + 1 : Settings.GridSize - 1;
                    for (int j = minmax; j >= 0; j--)
                    {

                        if (_map[i, j].State == TreeState.Empty || _map[i, j].State == TreeState.Burned)
                        {
                            

                        }
                        else if (_map[i, j].State == TreeState.Green)
                        {
                            //i'm a tree - check neigh
                            int ii = 0;
                            int jj = 0;
                            while (ii < 3)
                            {

                                while (jj < 3)
                                {

                                    if (!(ii == 1 && jj == 1))
                                    {
                                        int x = i + jj - 1;
                                        int y = j + ii - 1;
                                        if (x >= Settings.GridSize || y >= Settings.GridSize || x < 0 || y < 0)
                                        {
                                            //out of border
                                        }
                                        else if (_map[x, y].State == TreeState.Burning)
                                            _map[i,j].State = TreeState.Burning;

                                    }
                                    jj++;

                                }
                                jj = 0;
                                ii++;
                            }
                           
                        }
                        else if (_map[i, j].State == TreeState.Burning)
                        {
                            _map[i, j].State = TreeState.Burned;
                        }

                    }
                }

                //print matrix
                //for (int i = 0; i < Settings.GridSize; i++)
                //{
                //    for (int j = 0; j < Settings.GridSize; j++)
                //    {
                //        if (_map[i, j].State == TreeState.Empty)
                //            Console.Write("X-");
                //        else if (_map[i, j].State == TreeState.Burning)
                //            Console.Write("B-");
                //        else if (_map[i, j].State == TreeState.Green)
                //            Console.Write("G-");
                //        else
                //            Console.Write("R-");

                //    }
                //    Console.WriteLine();
                //}
                //Console.WriteLine();

                turn++;
            }

        }
    }


}