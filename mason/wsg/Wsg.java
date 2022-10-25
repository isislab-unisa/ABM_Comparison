/*
  Copyright 2006 by Sean Luke and George Mason University
  Licensed under the Academic Free License version 3.0
  See the file "LICENSE" for more information
*/

package sim.app.wsg;
import sim.engine.*;
import sim.util.*;
import sim.field.grid.IntGrid2D;
import sim.field.grid.SparseGrid2D;

public class Wsg extends SimState
    {
    private static final long serialVersionUID = 1;
    public static final int GRID_HEIGHT = 1131;
    public static final int GRID_WIDTH = 1131;
    public SparseGrid2D fieldSheeps = new SparseGrid2D(GRID_WIDTH, GRID_HEIGHT);
    public SparseGrid2D fieldWolves = new SparseGrid2D(GRID_WIDTH, GRID_HEIGHT);
    public IntGrid2D fieldGrass = new IntGrid2D(GRID_WIDTH, GRID_HEIGHT);

    public int numWS = 128000;
    public int numSheep = (int) (numWS * 0.6);
    public int numWolves = (int) (numWS * 0.4);

    /** Creates a Flockers simulation with the given random number seed. */
    public Wsg(long seed)
        {
        super(seed);
        }
    
    public void start()
        {
        super.start();

        for (int i = 0; i < GRID_WIDTH; i++) {
            for (int j = 0; j < GRID_HEIGHT; j++) {
                if (random.nextDouble() < 0.5) {
                    fieldGrass.set(i, j, random.nextInt(20) + 1);
                } else {
                    fieldGrass.set(i, j, 20);
                }
            }
        }

        // make a bunch of flockers and schedule 'em.  A few will be dead
        for(int x=0;x<numSheep;x++)
            {
            Int2D location = new Int2D(random.nextInt(GRID_WIDTH), random.nextInt(GRID_HEIGHT));
            Sheep sheep = new Sheep(location);
            sheep.fieldSheeps = fieldSheeps;
            sheep.fieldGrass = fieldGrass;
            sheep.theWsg = this;
            sheep.energy = random.nextInt(2*4) + 1;
            fieldSheeps.setObjectLocation(sheep, location.x, location.y);
            schedule.scheduleRepeating(sheep);
            }

        for(int x=0;x<numWolves;x++)
        {
            Int2D location = new Int2D(random.nextInt(GRID_WIDTH), random.nextInt(GRID_HEIGHT));
            Wolf wolf = new Wolf(location);
            wolf.fieldSheeps = fieldSheeps;
            wolf.fieldWolves = fieldWolves;
            wolf.theWsg = this;
            wolf.energy = random.nextInt(2*20) + 1;
            fieldWolves.setObjectLocation(wolf, location.x, location.y);
            schedule.scheduleRepeating(wolf);
        }

        schedule.scheduleRepeating(Schedule.EPOCH,1, new Steppable()
            {
            public void step(SimState state) { fieldGrass.add(1);}
            }, 1);
        }

    public static void main(String[] args)
        {
        doLoop(Wsg.class, args);
        System.exit(0);
        }    

    }
