using ActressMas;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.ConstrainedExecution;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Flockers
{
    public class InsectAgent : Agent
    {
        protected World _world;
        protected static Random _rand = new Random();
        //public int x = 0;
        //public int y = 0;
        public static double COHESION = 0.8;
        public static double AVOIDANCE = 1.0;
        public static double RANDOMNESS = 1.1;
        public static double CONSISTENCY = 0.7;
        public static double MOMENTUM = 1.0;
        public static double JUMP = 0.7;
        public static double DISCRETIZATION = 10.0 / 1.5;

        public double x { get; set; }
        public double y { get; set; }
        public double last_x { get; set; }
        public double last_y { get; set; }

        public int Line { get; set; } // position on the grid

        public int Column { get; set; } // position on the grid

        protected bool step()
        {
            int neigh_distance = 1;
            int matrixLenght = 1131;//works for any matrix as long as you change the lenght

            double avoid_x = 0.0;
            double cohe_x = 0.0;
            double rand_x = 0.0;
            double cons_x = 0.0;
            double avoid_y = 0.0;
            double cohe_y = 0.0;
            double rand_y = 0.0;
            double cons_y = 0.0;
            int count = 0;

            var my_x = (int)Math.Floor(x);
            var my_y = (int)Math.Floor(y);


            for (int i = my_x - neigh_distance; i <= my_x + neigh_distance; i++)
            {
                for (int j = my_y - neigh_distance; j <= my_y + neigh_distance; j++)
                {
                    if (i >= 0 && i < matrixLenght && j >= 0 && j < matrixLenght && (i != my_x || j != my_y))
                    {
                        if (_world._map[i, j].State == CellState.Ant)
                        // here i compute
                        {

                            count++;
                            //var dx = Math.Sqrt(x - _world._map[i, j].AgentInCell.Line);
                            //var dy = Math.Sqrt(y - _world._map[i, j].AgentInCell.Column);
                            var dx = x - _world._map[i, j].AgentInCell.Line;
                            var dy = y - _world._map[i, j].AgentInCell.Column;
                            var square = dx * dx + dy * dy;

                            avoid_x += dx / (int)(square * square + 1.0);
                            avoid_y += dy / (int)(square * square + 1.0);

                            cohe_x += dx;
                            cohe_y += dy;

                            cons_x += _world._map[i, j].AgentInCell.last_x;
                            cons_y += _world._map[i, j].AgentInCell.last_y;
                        }
                    }
                }
            }

            

            if (count > 0)
            {
                avoid_x /= count;
                avoid_y /= count;
                cohe_x /= count;
                cohe_y /= count;
                cons_x /= count;
                cons_y /= count;

                cons_x /= count;
                cons_y /= count;

            }

            avoid_x *= 400.0;
            avoid_y *= 400.0;

            cohe_x = -cohe_x / 10.0;
            cohe_y = -cohe_y / 10.0;

            rand_x = _rand.NextDouble() * 2.0 - 1.0;
            rand_y = _rand.NextDouble() * 2.0 - 1.0;
            var rand_square = Math.Sqrt(rand_x * rand_x + rand_y * rand_y);
            rand_x = 0.05 * rand_x / rand_square;
            rand_y = 0.05 * rand_y / rand_square;

            var mom_x = last_x;
            var mom_y = last_y;


            var ddx = COHESION * cohe_x
                + AVOIDANCE * avoid_x
                + CONSISTENCY * cons_x
                + RANDOMNESS * rand_x
                + MOMENTUM * mom_x;
            var ddy = COHESION * cohe_y
                + AVOIDANCE * avoid_y
                + CONSISTENCY * cons_y
                + RANDOMNESS * rand_y
                + MOMENTUM * mom_y;

            var dis = Math.Sqrt(ddx * ddx + ddy * ddy);
            if (dis > 0.0)
            {
                ddx = ddx / dis * JUMP;
                ddy = ddy / dis * JUMP;
            }

            last_x = ddx;
            last_y = ddy;

            var loc_x = 0.0;
            var loc_y = 0.0;

            if (x + ddx > Settings.GridSize )
            {
                loc_x = x - ddx;
            } else if (x - ddx < 0) {
                loc_x = x + ddx;
            } else
            {
                loc_x = x + ddx;
            }

            if (y + ddy> Settings.GridSize)
            {
                loc_y = y - ddy;
            }
            else if (y - ddy < 0)
            {
                
                loc_y = y + ddy;
            }
            else
            {
                loc_y = y + ddy;
            }

            if (loc_x < 0)
                loc_x = 0;
            if (loc_y < 0)
                loc_y = 0;

            x = loc_x;
            y = loc_y;

            var round_locx = (int)Math.Round(loc_x);
            var round_locy = (int)Math.Round(loc_y);

            if (round_locx >= Settings.GridSize)
                round_locx = Settings.GridSize - 1;
            if (round_locy >= Settings.GridSize)
                round_locy = Settings.GridSize - 1;

            //anti-collision system
            while (!(_world.Move(this, round_locx, round_locy)))
            {
                var min = Math.Min(round_locx, round_locy);
                if (min == round_locx)
                    round_locx += 1;
                else
                    round_locy += 1;

                if (round_locx >= Settings.GridSize)
                    round_locx = 0;
                if (round_locy >= Settings.GridSize)
                    round_locy = 0;

            }


                return true;

        }

        protected double toroidal_distance(double val1, double val2, double dim, out double my_out)
        {
            my_out = 0.0;
            if (Math.Abs(val1 - val2) <= dim / 2.0)
            {
                my_out = val1 - val2;
            }
            var cringe = 0.0;
            var d = toroidal_transform(val1, dim, out cringe) - toroidal_transform(val2, dim, out cringe);

            if (d * 2.0 > dim)
            {
                d -= dim;
            }
            else if (d * 2.0 < -dim)
            {
                d += dim;
            }
            return my_out = d;
        }

        protected double toroidal_transform(double val, double dim, out double my_out)
        {
            my_out = 0.0;
            if (val >= 0.0 && val < dim)
            {
                my_out = val;
            }
            else
            {
                val = val % dim;
                if (val < 0.0)
                {
                    val += dim;
                }
                my_out = val;
            }
            return my_out;
        }

    }

}