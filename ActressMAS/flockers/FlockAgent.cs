using ActressMas;
using System;
using System.Collections.Generic;


// my flocker
namespace Flockers
{
    public class FlockAgent : InsectAgent
    {


        public override void Setup()
        {
           
            _world = Environment.Memory["World"];

            if (Settings.Verbose)
                Console.WriteLine($"FlockAgent {Name} started in ({Line},{Column})");
        }

        public void setCoord(double xx, double yy)
        {
            x = xx;
            y = yy;
        }

        public override void ActDefault()
        {
            {
                FlockAction();
            }

        }

        private void FlockAction()
        {
            step();
        }
    }
}