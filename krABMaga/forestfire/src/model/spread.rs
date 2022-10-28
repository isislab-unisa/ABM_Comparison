use crate::model::forest::Forest;
use crate::model::forest::Status;
use crate::Tree;
use core::fmt;
use krabmaga::engine::agent::Agent;
use krabmaga::engine::location::Int2D;
use krabmaga::engine::schedule::Schedule;
use krabmaga::engine::state::State;
use std::cell::RefCell;
use std::hash::{Hash, Hasher};

#[derive(Clone, Copy)]
pub struct Spread {
    pub id: u32,
}

impl Agent for Spread {
    fn step(&mut self, state: &mut dyn State) {

        let real_state = state.as_any_mut().downcast_mut::<Forest>().unwrap();

        let minmax;
        if real_state.step + 1 < (real_state.dim.0 - 1).try_into().unwrap() {
            minmax = real_state.step + 1;
        } else {
            minmax = (real_state.dim.0 - 1).try_into().unwrap();
        }
        let mut l = real_state.last_active;
        if l <= 0 {
            l = 0;
        } else {
            l -= 1;
        }
        
        for my_i  in l..minmax  {
            let mut active_tree = false;

            for my_j in 0..real_state.dim.0 {
                let mut tree = real_state
                    .field
                    .get_objects_unbuffered(&Int2D {
                        x: my_i as i32,
                        y: my_j as i32,
                    })
                    .unwrap()[0];
                if tree.status == Status::Green {
                    let mut isolated = true;
                    for i in 0..3 {
                        for j in 0..3 {
                            if !(i == 1 && j == 1) {
                                let loc_n = Int2D {
                                    // location of neighbor
                                    x: my_i as i32 + i - 1,
                                    y: my_j as i32 + j - 1,
                                };
                                // not toroidal
                                if loc_n.x < 0
                                    || loc_n.y < 0
                                    || loc_n.x >= real_state.dim.0
                                    || loc_n.y >= real_state.dim.1
                                {
                                    continue;
                                };

                                // take the neighbor
                                let neighbor = match real_state.field.get_objects_unbuffered(&loc_n)
                                {
                                    Some(t) => t[0],
                                    None => {
                                        continue;
                                    }
                                };
                                // if a neighbor is BURNING, set me on BURNING
                                if neighbor.status == Status::Green || neighbor.status == Status::Burning {
                                    isolated = false;
                                    continue;
                                    
                                }
                            }
                        }
                        
                    }
                    if !isolated {
                        active_tree = true;
                        l = my_i+1;
                        break;
                    }
                    

                    
                } else if tree.status == Status::Burning {
                    active_tree = true;
                    l = my_i+1;
                    break;
                }
            }
            if active_tree {
                break;
            }
        }
        
        if (real_state.last_active == l) {
            real_state.stucked += 1;
            if real_state.stucked == 5 {
                real_state.last_active += 1;
                real_state.stucked = 0;
            }
        } else {
            real_state.last_active = l;
            real_state.stucked = 0;
        }

        let lg = real_state.last_active as u64;

        for ii in ((lg-1)..minmax + 1).rev() {
            for jj in 0..real_state.dim.0 {
                let mut tree = real_state
                    .field
                    .get_objects_unbuffered(&Int2D {
                        x: ii as i32,
                        y: jj as i32,
                    })
                    .unwrap()[0];
                let x = ii as i32;
                let y = jj as i32;
                if tree.status == Status::Green {
                    // get the neighbors around me
                    let mut update = false;
                    for i in 0..3 {
                        for j in 0..3 {
                            if !(i == 1 && j == 1) {
                                let loc_n = Int2D {
                                    // location of neighbor
                                    x: x + i - 1,
                                    y: y + j - 1,
                                };
                                // not toroidal
                                if loc_n.x < 0
                                    || loc_n.y < 0
                                    || loc_n.x >= real_state.dim.0
                                    || loc_n.y >= real_state.dim.1
                                {
                                    continue;
                                };

                                // take the neighbor
                                let neighbor = match real_state.field.get_objects_unbuffered(&loc_n)
                                {
                                    Some(t) => t[0],
                                    None => {
                                        continue;
                                    }
                                };
                                // if a neighbor is BURNING, set me on BURNING
                                if neighbor.status == Status::Burning {
                                    tree.status = Status::Burning;
                                    //println!("I am {:?} passing on {:?} from {:?} step {}", value.id, value.status, neighbor.id, schedule.step);
                                    update = true;
                                    real_state.field.set_object_location(tree, &Int2D { x, y });
                                    break; // avoid to be burned more than once
                                }
                            }
                        }
                        if update {
                            break;
                        }
                    }
                } else if tree.status == Status::Burning {
                    tree.status = Status::Burned;
                    real_state.field.set_object_location(tree, &Int2D { x, y });
                }


            }
        }
        
    }
}

impl Spread {
    #[allow(dead_code)]
    fn update(
        _loc: &Int2D,
        _value: &Tree,
        _state: &mut dyn State,
        _schedule: &mut Schedule,
        _schedule_id: u32,
    ) {
    }
}

impl Hash for Spread {
    fn hash<H>(&self, state: &mut H)
    where
        H: Hasher,
    {
        self.id.hash(state);
    }
}

impl fmt::Display for Spread {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        write!(f, "{}", self.id)
    }
}

impl Eq for Spread {}

impl PartialEq for Spread {
    fn eq(&self, other: &Spread) -> bool {
        self.id == other.id
    }
}
