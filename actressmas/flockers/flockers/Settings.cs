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

namespace PredatorPrey
{
    public class Settings
    {
        public static int GridSize = 10000; //20;3160
        public static int NoTurns = 200;
        public static int NoAnts = 10000000; //100;1000000

        public static bool ShowWorld = false;
        public static bool Verbose = false;
        public static string WorldStateFileName = "world-state.txt";
    }
}