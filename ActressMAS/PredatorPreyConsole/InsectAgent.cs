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

using ActressMas;
using System;
using System.Collections.Generic;

namespace PredatorPrey
{
    public class InsectAgent : Agent
    {
        protected int _turnsSurvived;
        protected World _world;
        protected static Random _rand = new Random();
        protected int _energy = 0;

        public InsectAgent(int energy)
        {
            _energy = energy;
        }

        public int Line { get; set; } // position on the grid

        public int Column { get; set; } // position on the grid
        

        protected bool TryToMove()
        {
            var direction = (Direction)_rand.Next(4);

            if (_world.ValidMovement(this, direction, CellState.Empty, out int newLine, out int newColumn))
            {
                if (Settings.Verbose)
                    Console.WriteLine($"Moving {Name}");

                _world.Move(this, newLine, newColumn);

                return true;
            }
            else
                return false;
        }

        protected bool TryToEat()
        {
            return _world.CheckFood(this.Line, this.Column);
        }


        protected bool TryToBreed()
        {
            var allowedDirections = new List<Direction>();
            int newLine, newColumn;

            for (int i = 0; i < 4; i++)
            {
                if (_world.ValidMovement(this, (Direction)i, CellState.Empty, out newLine, out newColumn))
                    allowedDirections.Add((Direction)i);
            }

            if (allowedDirections.Count == 0)
                return false;

            int r = _rand.Next(allowedDirections.Count);
            _world.ValidMovement(this, allowedDirections[r], CellState.Empty, out newLine, out newColumn);

            double r2 = _rand.NextDouble();
            if (r2 < 0.4)
            {
                var newInsect = _world.Breed(this, newLine, newColumn, this._energy/2);
                Environment.Add(newInsect);
                return true;
            }
            

            return false;
        }

        protected void Die()
        {
            // removing the ant

            if (Settings.Verbose)
                Console.WriteLine($"Removing {Name}");

            _world.Die(this);
            Stop();
        }
    }
}