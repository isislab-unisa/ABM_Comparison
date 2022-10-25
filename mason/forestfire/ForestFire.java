/*
  Copyright 2006 by Sean Luke and George Mason University
  Licensed under the Academic Free License version 3.0
  See the file "LICENSE" for more information
*/

package sim.app.forestfire;
import sim.engine.*;
import sim.field.grid.*;
import sim.util.*;

public /*strictfp*/ class ForestFire extends SimState
    {
    private static final long serialVersionUID = 1;
    public static int size = 5;
    public int gridHeight;
    public int gridWidth;
    public int neighborhood = 1;
    public double density = 0.7;

    public IntGrid2D myfield = new IntGrid2D(size, size);

    // we presume that no one relies on these DURING a simulation
    public int getGridHeight() { return gridHeight; }
    public void setGridHeight(int val) { if (val > 0) gridHeight = val; }
    public int getGridWidth() { return gridWidth; }
    public void setGridWidth(int val) { if (val > 0) gridWidth = val; }
    public int getNeighborhood() { return neighborhood; }
    public void setNeighborhood(int val) { if (val > 0) neighborhood = val; }

    public static final int GREEN = 0;
    public static final int BURNING = 1;
    public static final int RED = 2;
    public static final int EMPTY = 3;

    public static void printMatrix(int gridWidth, int gridHeight, IntGrid2D myfield)
        {
            for (int i = 0; i < gridHeight; i++) {
                for (int j = 0; j < gridWidth; j++) {
                    int v = myfield.get(i,j);
                    if (v == BURNING) {
                        System.out.print("B-");
                    } else if (v == GREEN) {
                        System.out.print("G-");
                    } else if (v == RED) {
                        System.out.print("R-");
                    } else {
                        System.out.print("X-");
                    }
                    //System.out.print(myfield.get(i,j) + " ");
                }
                System.out.println();
            }
            System.out.println();
        }    
    

    /** Creates a Schelling simulation with the given random number seed. */
    public ForestFire(long seed)
        {
        this(seed, size, size);
        }
        
    public ForestFire(long seed, int width, int height)
        {
        super(seed);
        gridWidth = width; gridHeight = height;
        createGrids();
        }

    protected void createGrids()
        {
        // neighbors = new IntGrid2D(gridWidth, gridHeight,0);
        // int[][] g = neighbors.field;
        for(int x=0;x<gridWidth;x++)
            for(int y=0;y<gridHeight;y++) {
                if (random.nextDouble() < density) {
                    if (y == 0) {
                        myfield.set(x,y, BURNING);
                    } else {
                        myfield.set(x,y, GREEN);
                    }
                } else {
                    myfield.set(x,y, EMPTY);
                }
            }
        }
    
    /** Resets and starts a simulation */
    public void start() {
        super.start();  // clear out the schedule
        


        // make new grids
        createGrids();


        schedule.scheduleRepeating(Schedule.EPOCH,1, new Steppable()
        {
        public void step(SimState state) {
            // print the number of step
            long step = state.schedule.getSteps();
            // System.out.println("Step: " + step );

            int minmax = (step + 1 < gridHeight - 1) ? (int) step + 1 : gridHeight - 1;
            for (int j = minmax; j >= 0; j--) {
                for (int i = 0; i < gridHeight; i++) {
                    // System.out.println("i: " + i + " j: " + j);
                    int v = myfield.get(i,j);
                    if (v == GREEN) {
                        for (int k = i-1; k <= i+1; k++) {
                            for (int l = j-1; l <= j+1; l++) {
                                if (k >= 0 && k < gridWidth && l >= 0 && l < gridHeight
                                    && (k != i || l != j)) {
                                    if (myfield.get(k,l) == BURNING) {
                                        myfield.set(i,j, BURNING);
                                        break;
                                    }
                                }
                            }
                        }
                    } else if (v == BURNING) {
                        myfield.set(i,j,RED);
                    }
                }
                
            }
            //printMatrix(gridHeight, gridWidth, myfield);
        }
        }, 1);  
    }
    
    
    public static void main(String[] args)
        {
        doLoop(ForestFire.class, args);
        System.exit(0);
        }    
    }
    
    
    
    
    
