/**
 * 
 */
package repast.ff;

import java.util.List;

import repast.simphony.context.space.grid.GridFactory;
import repast.simphony.context.space.grid.GridFactoryFinder;
import repast.simphony.engine.watcher.Watch;
import repast.simphony.engine.watcher.WatcherTriggerSchedule;
import repast.simphony.query.space.grid.GridCell;
import repast.simphony.query.space.grid.GridCellNgh;
import repast.simphony.random.RandomHelper;
import repast.simphony.space.SpatialMath;
import repast.simphony.space.continuous.ContinuousSpace;
import repast.simphony.space.continuous.NdPoint;
import repast.simphony.space.grid.Grid;
import repast.simphony.space.grid.GridBuilderParameters;
import repast.simphony.space.grid.GridPoint;
import repast.simphony.space.grid.SimpleGridAdder;
import repast.simphony.space.grid.WrapAroundBorders;
import repast.simphony.util.SimUtilities;

/**
 * @author nick
 *
 */
public class Forest {
	
	private ContinuousSpace<Object> space;
	private Grid<Object> grid;
	private int dim = 1131;
	private int[][] mygrid = new int[dim][dim];
	public static final int GREEN = 0;
	public static final int BURNING = 1;
	public static final int BURNED = 2;
	public static final int EMPTY = 3;
	private int step = 0;

	public Forest(ContinuousSpace<Object> space, Grid<Object> grid) {
		this.space = space;
		this.grid = grid;
		
		for (int i=0; i<dim; i++) {
			for (int j=0; j<dim; j++) {
				this.mygrid[i][j] = EMPTY;
			}
		}
		
		for (int i=0; i<dim; i++) {
			for (int j=0; j<dim; j++) {
				if (RandomHelper.nextDoubleFromTo(0, 1) < 0.7) {
					if (j == 0) {
						//i'm burning
						this.mygrid[i][j] = BURNING;
					} else {
						this.mygrid[i][j] = GREEN;
					}
				}
			}
		}

	}
	
//	@Watch(watcheeClassName = "jzombies.Zombie", watcheeFieldNames = "moved", 
//			query = "within_vn 1", whenToTrigger = WatcherTriggerSchedule.IMMEDIATE)
	public void run() {
		System.out.println("step " + step);
		
		int minmax= (step + 1 < dim - 1 ) ? step + 1 : dim - 1;
		for (int j = minmax; j>=0; j--) {
			for (int i = 0; i < dim; i++) {
				int v = mygrid[i][j];
				if (v == GREEN) {
					for (int k = i-1; k<= i+1; k++) {
						for (int l = j-1; l <= j+1; l++) {
							if (k>=0 && k < dim && l>=0 && l<dim && (k != i || l != j)) {
								if (mygrid[k][l] == BURNING) {
									mygrid[i][j] = BURNING;
									break;
								}
							}
						}
					}
					
				} else if (v == BURNING) {
					mygrid[i][j] = BURNED;
				}
				
			}
		}
		
		//print_matrix();
		// get the grid location of this Forest
//		GridPoint pt = grid.getLocation(this);
//
//		// use the GridCellNgh class to create GridCells for
//		// the surrounding neighborhood.
//		GridCellNgh<Zombie> nghCreator = new GridCellNgh<Zombie>(grid, pt,
//				Zombie.class, 1, 1);
//		List<GridCell<Zombie>> gridCells = nghCreator.getNeighborhood(true);
//		SimUtilities.shuffle(gridCells, RandomHelper.getUniform());
//
//		GridPoint pointWithLeastZombies = null;
//		int minCount = Integer.MAX_VALUE;
//		for (GridCell<Zombie> cell : gridCells) {
//			if (cell.size() < minCount) {
//				pointWithLeastZombies = cell.getPoint();
//				minCount = cell.size();
//			}
//		}
//		
//		if (energy > 0) {
//			moveTowards(pointWithLeastZombies);
//		} else {
//			energy = startingEnergy;
//		}
		step++;
	}
	
	public void print_matrix() {
		for (int i=0; i<dim; i++) {
			for (int j=0; j<dim; j++) {
				int v = mygrid[i][j];
				if (v == BURNING) {
					System.out.print("B-");
				} else if (v == GREEN) {
					System.out.print("G-");
				} else if (v == BURNED) {
					System.out.print("R-");
				} else {
					System.out.print("X-");
				}
			}
			System.out.println();
		}
		System.out.println();
	}
	
//	public void moveTowards(GridPoint pt) {
//		// only move if we are not already in this grid location
//		if (!pt.equals(grid.getLocation(this))) {
//			NdPoint myPoint = space.getLocation(this);
//			NdPoint otherPoint = new NdPoint(pt.getX(), pt.getY());
//			double angle = SpatialMath.calcAngleFor2DMovement(space, myPoint, otherPoint);
//			space.moveByVector(this, 2, angle, 0);
//			myPoint = space.getLocation(this);
//			grid.moveTo(this, (int)myPoint.getX(), (int)myPoint.getY());
//			//energy--;
//		}
//	}

}
