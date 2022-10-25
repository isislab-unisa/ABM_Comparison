model boids 
global torus: torus_environment{ 
	int number_of_agents <- 64000 min: 1 max: 128000;
	int number_of_obstacles <- 0 min: 0;
	float maximal_speed <- 15.0 min: 0.1 max: 15.0;
	int cohesion_factor <- 200;
	int alignment_factor <- 100; 
	float minimal_distance <- 30.0; 
	int width_and_height_of_environment <- 800;  
	bool torus_environment <- false; 
	bool apply_cohesion <- true ;
	bool apply_alignment <- true ;
	bool apply_separation <- true;
	bool apply_avoid <- true;  
	bool apply_wind <- true;   
	bool moving_obstacles <- false;   
	int bounds <- int(width_and_height_of_environment / 20); 
	point wind_vector <- {0,0}; 
	list<image_file> images  <- [image_file('../images/bird1.png'),image_file('../images/bird2.png'),image_file('../images/bird3.png')]; 
	int xmin <- bounds;   
	int ymin <- bounds;  
	int xmax <- (width_and_height_of_environment - bounds);     
	int ymax <- (width_and_height_of_environment - bounds);   
	action move_goal {
		ask first(boids_goal) {
			do goto target: #user_location speed: 30.0;
		}
	}
	geometry shape <- square(width_and_height_of_environment);
	init { 
		create boids number: number_of_agents { 
			 location <- {rnd (width_and_height_of_environment - 2) + 1, rnd (width_and_height_of_environment -2) + 1 };
		} 
		create obstacle number: number_of_obstacles {
			location <- {rnd (width_and_height_of_environment - 2) + 1, rnd (width_and_height_of_environment -2) + 1 }; 
		}
		create  boids_goal;	
	}	
}
species boids_goal skills: [moving] {
	float range  <- 20.0;
	reflex wander {  
		do  wander amplitude: 45.0 speed: 20.0;  
	}
	aspect default {
		draw circle(10) color: #red ;
		draw circle(40) color: #orange wireframe: true;
	}
} 
species boids skills: [moving] {
	//Speed of the boids agents
	float speed max: maximal_speed <- maximal_speed;
	//Range used to consider the group of the agent
	float range <- minimal_distance * 2;
	point velocity <- {0,0};
	reflex separation when: apply_separation {
		point acc <- {0,0};
		ask (boids overlapping (circle(minimal_distance)))  {
			acc <- acc - ((location) - myself.location);
		}  
		velocity <- velocity + acc;
	}
	reflex alignment when: apply_alignment {
		list others  <- ((boids overlapping (circle (range)))  - self);
		point acc <- mean (others collect (each.velocity)) - velocity;
		velocity <- velocity + (acc / alignment_factor);
	}
	reflex cohesion when: apply_cohesion {
		list others <- ((boids overlapping (circle (range)))  - self);
		point mass_center <- (length(others) > 0) ? mean (others collect (each.location)) : location;
		point acc <- mass_center - location;
		acc <- acc / cohesion_factor; 
		velocity <- velocity + acc;   
	}
	reflex avoid when: apply_avoid { 
		point acc <- {0,0};
		list<obstacle> nearby_obstacles <- (obstacle overlapping (circle (range)) );
		loop obs over: nearby_obstacles {
			acc <- acc - ((location of obs) - my (location));
		}
		velocity <- velocity + acc; 
	}
	action bounding {
		if  !(torus_environment) {
			if  (location.x) < xmin {
				velocity <- velocity + {bounds,0};
			} else if (location.x) > xmax {
				velocity <- velocity - {bounds,0};
			}
			if (location.y) < ymin {
				velocity <- velocity + {0,bounds};
			} else if (location.y) > ymax {
				velocity <- velocity - {0,bounds};
			}
		} else {
			if (location.x) < 0.0 {
				location <- {width_and_height_of_environment + location.x,location.y};
			} else if (location.x) > width_and_height_of_environment {
				location <- {location.x - width_and_height_of_environment ,location.y};
			}
			if (location.y) < 0.0 {
				location <- {location.x, width_and_height_of_environment + location.y};
			} else if (location.y) > width_and_height_of_environment {
				location <- {location.x,location.y - width_and_height_of_environment};
			}	
		}
	}
	reflex follow_goal {
		velocity <- velocity + ((first(boids_goal).location - location) / cohesion_factor);
	}
	reflex wind when: apply_wind {
		velocity <- velocity + wind_vector;
	}
	action do_move {  
		if (((velocity.x) as int) = 0) and (((velocity.y) as int) = 0) {
			velocity <- {(rnd(4)) -2, (rnd(4)) - 2};
		}
		point old_location <- copy(location);
		do goto target: location + velocity;
		velocity <- location - old_location;
	}
	reflex movement {
		do do_move;
		do bounding;
	}
	aspect image {
		draw (images at (rnd(2))) size: {50,50} rotate: heading ;      
	}
	aspect circle { 
		draw circle(15)  color: #red;
	}
	aspect default { 
		draw circle(20) color: #lightblue wireframe: true;
	}
} 
species obstacle skills: [moving] {
	float speed <- 2.0;

	init {
		shape <- triangle(15);
	}	
	//Reflex to move the obstacles if it is available
	reflex move_obstacles when: moving_obstacles {
		//Will make the agent go to a boid with a 50% probability
		if flip(0.5)  
		{ 
			do goto target: one_of(boids);
		} 
		else{ 
			do wander amplitude: 360.0;   
		}
	}
	aspect default {
		draw  triangle(20) color: #black ;
	}

}
experiment "exp" type: batch repeat: 6 keep_seed: false until: (cycle = 200) {	

}