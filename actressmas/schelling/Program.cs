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
using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;

namespace Agents1
{
    public enum CellState { Empty, Red, Blue };

    public class Cell
    {
        public CellState State;
        public Boolean moved;

        public Cell()
        {
            State = CellState.Empty;
            moved = false;
        }
    }

    public class Program
    {
        static readonly Stopwatch timer = new Stopwatch();

        private static void Main(string[] args)
        {
            var env = new EnvironmentMas(noTurns: Settings.NoTurns);
            var a1 = new MyAgent(); env.Add(a1, "sch");
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

        public override void Setup()
        {
            Console.WriteLine($"{Name} starting");
            _map = new Cell[Settings.GridSize, Settings.GridSize];
            for (int i = 0; i < Settings.GridSize; i++)
                for (int j = 0; j < Settings.GridSize; j++)
                    _map[i, j] = new Cell();

            int c = 0;
            while (c < Settings.NoCell/2) {
                int r1 = _rand.Next(Settings.GridSize);
                int r2 = _rand.Next(Settings.GridSize);
                double r = _rand.NextDouble();
                if (r < 0.5)
                {
                    if (_map[r1, r2].State == CellState.Empty)
                    {
                        _map[r1, r2].State = CellState.Red;
                        c++;
                    }
                }     
            }
            
            while (c < Settings.NoCell)
            {
                int r1 = _rand.Next(Settings.GridSize);
                int r2 = _rand.Next(Settings.GridSize);
                double r = _rand.NextDouble();
                if (r < 0.5)
                {
                    if (_map[r1, r2].State == CellState.Empty)
                    {
                        _map[r1, r2].State = CellState.Blue;
                        c++;
                    }
                }
            }
            Console.WriteLine("end startup");

        }

        public override void ActDefault()
        {
            
            {
                
                for (int i = 0; i < Settings.GridSize; i++)
                {

                    for (int j = 0; j < Settings.GridSize; j++)
                    {

                        if (_map[i, j].State == CellState.Empty)
                        {
                            //nothing
                        }
                        else
                        {
                            //i'm a cell - check neigh
                            int count_similar = 0;
                            int ii = 0;
                            int jj = 0;
                            while (ii < 3) {

                                while (jj < 3) {

                                    if (!(ii == 1 && jj == 1)) {
                                        int x = i + jj - 1;
                                        int y = j + ii - 1;
                                        if (x >= Settings.GridSize || y >= Settings.GridSize || x < 0 || y < 0)
                                        {
                                            // nothing to do
                                            //x = x % Settings.GridSize;
                                            //y = y % Settings.GridSize;
                                            //x = Settings.GridSize - 1;
                                            //y = Settings.GridSize - 1;

                                        } else if (_map[x, y].State == _map[i, j].State)
                                            count_similar++;
                                            
                                        
                                    }
                                    jj++;
                                    
                                }
                                jj = 0;
                                ii++;
                            }

                            if (count_similar >= Settings.similar) {
                                // not move from this pos
                                
                                _map[i, j].moved = true;
                            } else {
                                // move
                                while (true) {

                                    int r1 = _rand.Next(Settings.GridSize);
                                    int r2 = _rand.Next(Settings.GridSize);

                                    if (_map[r1, r2].State == CellState.Empty)
                                    {
                                        
                                        _map[r1, r2].State = _map[i, j].State;
                                        _map[r1, r2].moved = true;
                                        _map[i, j].State = CellState.Empty;
                                        _map[i, j].moved = true;
                                        break;
                                    }
                                }
                            }

                        }
                    }
                }

                for (int i = 0; i < Settings.GridSize; i++)
                    for (int j = 0; j < Settings.GridSize; j++)
                        _map[i, j].moved = false;
                ////print matrix
                //for (int i=0; i<Settings.GridSize; i++) {
                //    for (int j = 0; j < Settings.GridSize; j++) {
                //        if (_map[i, j].State == CellState.Empty)
                //            Console.Write("X-");
                //        else if (_map[i, j].State == CellState.Red)
                //            Console.Write("R-");
                //        else
                //            Console.Write("B-");

                //    }
                //    Console.WriteLine();
                //}
                //Console.WriteLine();
                        
            }
           
        }
    }

    
}