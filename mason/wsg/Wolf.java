/*
  Copyright 2006 by Sean Luke and George Mason University
  Licensed under the Academic Free License version 3.0
  See the file "LICENSE" for more information
*/

package sim.app.wsg;
import sim.engine.*;
import sim.field.continuous.*;
import sim.field.grid.IntGrid2D;
import sim.field.grid.SparseGrid2D;
import sim.util.*;
import ec.util.*;


public class Wolf implements Steppable, sim.portrayal.Orientable2D
    {
    private static final long serialVersionUID = 1;

    public Int2D loc = new Int2D(0,0);
    public Int2D lastd = new Int2D(0,0);
    public SparseGrid2D fieldWolves;
    public SparseGrid2D fieldSheeps;
    public Wsg theWsg;
    public double mom = 0.5;
    public boolean dead = false;
    public int energy = 0;
    public Wolf(Int2D location) { loc = location; }
    
    public boolean isDead() { return dead; }
    public void setDead(boolean val) { dead = val; }
    
    public void setOrientation2D(double val)
        {
        Double2D lastdd = new Double2D(Math.cos(val),Math.sin(val));
        lastd = new Int2D((int)lastdd.x, (int)lastdd.y);
        
        }
    
    public double orientation2D()
        {
        if (lastd.x == 0 && lastd.y == 0) return 0;
        return Math.atan2(lastd.y, lastd.x);
        }
    
    public void step(SimState state)
        {     
            
        if (dead) return;

        final Wsg wsg = (Wsg)state;
        

        MersenneTwisterFast random = new MersenneTwisterFast(1);
        boolean moved = false;
        int x = loc.x;
        int y = loc.y;

        if (random.nextDouble() < mom) {
            int xm = x + (x - lastd.x);
            int ym = y + (y - lastd.y);
            Int2D new_loc = new Int2D(xm, ym);
            if (xm > 0 && xm < Wsg.GRID_WIDTH - 1 && ym > 0 && ym < Wsg.GRID_HEIGHT - 1) {
                wsg.fieldWolves.setObjectLocation(this, new_loc);
                loc = new_loc;
                lastd = new Int2D(x, y);
                moved = true;
            }
        }

        if (!moved) {
            int xmin = (x>0) ? -1 : 0;
            int xmax = (x<Wsg.GRID_WIDTH-1) ? 1 : 0;
            int ymin = (y>0) ? -1 : 0;
            int ymax = (y<Wsg.GRID_HEIGHT-1) ? 1 : 0;

            //generate int between xmin and xmax
            int dx = x + random.nextInt((xmax-xmin) + 1) + xmin;
            int dy = y + random.nextInt((ymax-ymin) + 1) + ymin;
            Int2D new_loc = new Int2D(dx, dy);
            loc = new_loc;
            lastd = new Int2D(x, y);
        }

        wsg.fieldWolves.setObjectLocation(this, loc);
        
        //eat sheep
        Bag b = wsg.fieldSheeps.getObjectsAtLocation(loc.x, loc.y);
        if (b != null && b.numObjs > 0) {
            for (int k = 0; k < b.numObjs; k++) {
                Sheep s = (Sheep) b.objs[k];
                if (!s.isDead())
                {
                    s.setDead(true);
                    energy += 20;
                    break;
                }
            }
        }
            

        energy -= 1;
        if (energy <= 0) {
            dead = true;
            state.schedule.scheduleOnce(this);
            return;
        }

        //reproduce
        if (random.nextDouble() < 0.2) {
            energy /= 2;
            Int2D location = new Int2D(random.nextInt(Wsg.GRID_WIDTH), random.nextInt(Wsg.GRID_HEIGHT));

            Wolf w = new Wolf(location);
            w.energy = energy;

            wsg.fieldWolves.setObjectLocation(w, location);
            Schedule schedule = state.schedule;
            schedule.scheduleRepeating(w);
        }

        }
 
    }
