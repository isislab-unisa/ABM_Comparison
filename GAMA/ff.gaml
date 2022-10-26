model ff

global {
	file my_csv_file <- csv_file("path/to/matrix100.csv");
	init {
		matrix data <- matrix(my_csv_file);
		ask grid_cell{
			grid_value <- float(data[grid_x, grid_y]);
			self.value <- grid_value;
			if (grid_value = 0) {
		        color <- #white;
		    } else if (grid_value = 1) {
		        color <- #green;
		    } else if (grid_value = 2) {
				//write cycle;
		        color <- #orange;
		    }
		}
	}
	reflex update {
		ask grid_cell {
	        do update_color;
	    }
	} 
	
	reflex stop_sim when: (cycle > 200) {
		do pause;
	}
	
	reflex stop_sim when: (cycle = 199) {
		save ("timeX " + total_duration) to: "result.txt" type: "text" rewrite: (cycle=0) ? true : false;		
	}
	
	
	 
}

grid grid_cell width: 800 height: 800 neighbors: 8 {

	int value;
	
	action update_color {
		if (value = 2) {
			if (grid_x < cycle + 1) {
				list<grid_cell> neigh <- (self neighbors_at 1);
				list<grid_cell> neigh2 <- neigh where (each.value = 1);
				if length(neigh2) > 0 {
					loop i from: 0 to: length(neigh2)-1 {
					neigh2[i].value <- 2;
					neigh2[i].color <- #orange;
					
					}
				}
				
				color <- #red;
				
			   
		    }
		}

    }
}

experiment ff type: gui {
	output {
		display main_display {
			grid grid_cell border: #black;
		}
	}
}


experiment fff type: batch repeat: 5 keep_seed: false until: (cycle = 200) {	
	reflex stop_sim when: (cycle = 199) {
		save ("time " + total_duration) to: "result.csv" type: "text" rewrite: (int(self)=0) ? true : false;		
	}
	
}