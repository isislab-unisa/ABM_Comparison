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
    public class Sheep : InsectAgent
    {
        public Sheep(int energy) : base(energy)
        {
        }

        public override void Setup()
        {
            _turnsSurvived = 0;
            _world = Environment.Memory["World"];
            if (Settings.Verbose)
                Console.WriteLine($"AntAgent {Name} started in ({Line},{Column})");
        }

        public override void ActDefault()
        {
            //try
            {
                AntAction();
            }
        }

        private void AntAction()
        {
            // move
            TryToMove(); // implemented in base class InsectAgent

            _energy -= 1;
            if (_energy <= 0)
            {
                Die();
            }

            //eat
            if (TryToEat()) {
                //Console.WriteLine($"agg magnat");
                _energy += 4;
            }

            // reproduce
            if (TryToBreed())
            {
                //Console.WriteLine($"double pecora");
                _energy /= 2;
            };
        }
    }
}