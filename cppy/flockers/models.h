#include "cppyabm/bases.h"
#include "cppyabm/mesh.h"
#include <random>
#include <vector>
#include <chrono>

struct PARAMS{
	static unsigned steps;
	static float height;
	static float width;
	static unsigned num_flockers;
	// sheep_reproduce=0.04,
	static float cohesion;
	static float avoidance;
	static float randomness;
	static float consistency;
	static float momentum;
	static float jump;
	static float discretization;
	
};
unsigned PARAMS::steps = 200;
// unsigned PARAMS::steps = 10;
float PARAMS::height=3162;
float PARAMS::width=3162;
unsigned PARAMS::num_flockers=1000000;
float PARAMS::cohesion=0.8;
float PARAMS::avoidance=1.0;
float PARAMS::randomness=1.1;
float PARAMS::consistency=0.7;
float PARAMS::momentum=1.0;
float PARAMS::jump=0.7;
float PARAMS::discretization=10.0 / 1.5;

	

struct Flockers;
struct Flocker;
struct MyPatch;
using DataType = std::map<std::string,std::vector<float>>;

struct random{
	static double randrange(double min, double max){
	    std::random_device rd;
	    std::mt19937 gen(rd());
	    std::uniform_real_distribution<> dis(min,max);
	    return dis(gen);
	};
	static double randint(int min, int max){
	    return rand()%(max-min) + min;;
	};
	static int choice(vector<int> list){
	    std::random_device rd;
	    std::mt19937 gen(rd());
	    std::discrete_distribution<> dis(list.begin(),list.end());
	    return list[dis(gen)];
	};
};


struct MyPatch: public Patch<Flockers,Flocker,MyPatch>{

	using basePatch = Patch<Flockers,Flocker,MyPatch>;
	// using basePatch::basePatch;
	MyPatch(shared_ptr<Flockers> env,MESH_ITEM mesh_item):basePatch(env,mesh_item){
		this->setup();
	}
	void setup(){
	}
	
};

struct Flocker: public Agent<Flockers,Flocker,MyPatch>{

	using baseAgent = Agent<Flockers,Flocker,MyPatch>;
	// using baseAgent::baseAgent;
	Flocker(shared_ptr<Flockers> env, string class_name):baseAgent(env,class_name){
		this->x = 0.0;
		this->y = 0.0;
		this->last_x = 0.0;
		this->last_y = 0.0;
	}
	
	void random_move();
	void flock_step();

	void step(){
		// this->random_move();
		this->flock_step();
	}
	float x;
	float y;
	float last_x;
	float last_y;
};


struct Flockers: public Env<Flockers,Flocker,MyPatch>{

	using baseEnv = Env<Flockers,Flocker,MyPatch>;
	// using baseEnv::baseEnv;

	Flockers():Env<Flockers,Flocker,MyPatch>(){
	}
	void reset(unsigned iter_i = 0){
		this->agents.clear();
		this->patches.clear();
		this->iter_i = iter_i;
		// this->data = {{"Sheep",{}},{"Wolf",{}},{"memory",{}}};
		this->data = {{"memory",{}}};
		auto mesh = space::grid2(PARAMS::height, PARAMS::width,1, true);
		this->setup_domain(mesh);
		this->setup_Flockers("Flock");
	}
	void setup_Flockers(string class_name){
		int number = PARAMS::num_flockers;
		// cout<<"number: "<<number<<endl;
		for (unsigned i=0; i < number; i++){
			//TODO random here
			auto x = random::randrange(0,PARAMS::width);
			auto y = random::randrange(0,PARAMS::height);
			auto a =this->generate_agent(class_name, x, y);
			
			//auto dest_index = random::choice(this->patches_keys);
			auto dest_index = (int)x * PARAMS::width + (int)y;
			// cout<<"dest_index: "<<dest_index<<endl;

			auto dest = this->patches[dest_index];
			this->place_agent(dest,a,true);
		}
	}
	shared_ptr<Flocker> generate_agent(string agent_name, float x, float y){
		shared_ptr<Flocker> obj;

		obj = make_shared<Flocker>(this->shared_from_this(),agent_name);
		obj->x = x;
		obj->y = y;
		this->agents.push_back(obj);
		return obj;
	}
	shared_ptr<MyPatch> generate_patch(MESH_ITEM mesh_item){
		
		auto patch_obj = make_shared<MyPatch>(this->shared_from_this(),mesh_item);
		this->patches[mesh_item.index] = patch_obj;
		return patch_obj;
	}
	void step(){
		this->activate_random();
		// this->step_patches();
		// this->collect_output();
		this->update();
	}
	// void collect_output(){
	// 	auto counts = this->count_agents();
	// 	for (auto &[key,value]: counts){
	// 		this->data[key].push_back(value);
	// 	}
	// 	auto usage = this->memory_usage();
	// 	this->data["memory"].push_back(float(usage));
	// }
	DataType episode(){
		auto start = chrono::high_resolution_clock::now();
		for (unsigned i=0; i< PARAMS::steps; i++) {
			// cout << "----- step: " << i << endl;
			this->step();
		}

		auto end = chrono::high_resolution_clock::now();
        auto iter_dur = chrono::duration_cast<chrono::milliseconds>(end-start);

		this->output(this->data,iter_dur.count());
		return this->data;
	}
	void output(DataType&,float);
	DataType data;
	unsigned iter_i;
};

inline void Flockers::output(DataType &results,float cpu_time){
	ofstream fd;
	string file_name = "results_" + std::to_string(this->iter_i)+".csv";
	fd.open(file_name);
	vector<string> keys;
	vector<vector<float>> stack_results;
	for (auto & [key,value]:results){
		keys.push_back(key);
		stack_results.push_back(value);
	}
	// write the header
	for (auto &key:keys){
		fd<<","<<key;
	}
	fd<<endl;
	// write the rows
	unsigned len = 0;
	if (stack_results.size()>0) len = stack_results[0].size();
	for (unsigned i = 0; i<len; i++){
		fd<<i;
		for (unsigned j=0; j<keys.size(); j++){
			fd<<","<<stack_results[j][i];
		}	
		fd<<endl;
	}
	fd.close();
	/** output cpu **/
	ofstream fd2;
	string file_name2 = "cpu_" + std::to_string(this->iter_i)+".csv";
	fd2.open(file_name2);
	fd2<<"CPU"<<endl;
	fd2<<cpu_time<<endl;
	fd2.close();
	/** output memory **/
	auto n = this->data["memory"].size(); 
	float average = 0.0f;
	if ( n != 0) {
	     average = accumulate( this->data["memory"].begin(), this->data["memory"].end(), 0.0) / n; 
	}
	ofstream fd3;
	string file_name3 = "memory_" + std::to_string(this->iter_i)+".csv";
	fd3.open(file_name3);
	fd3<<"Memory"<<endl;
	fd3<<average<<endl;
	fd3.close();

}

inline void Flocker::random_move(){
	auto neighbors = this->get_patch()->neighbors;	
	auto chosen_index=random::randint(0,neighbors.size());
	auto dest = neighbors[chosen_index];
	this->move(dest,true);
}

inline void Flocker::flock_step(){
		auto neighbors = this->get_patch()->find_neighbor_agents(false);	
		// cout << "neighbors size: " << neighbors.size() << endl;
		
		auto avoid_x = 0.0f;
		auto avoid_y = 0.0f;
		auto cohesion_x = 0.0f;
		auto cohesion_y = 0.0f;
		auto consistency_x = 0.0f;
		auto consistency_y = 0.0f;
		auto random_x = 0.0f;
		auto random_y = 0.0f;
		auto count = 0;

		for (auto &neighbor: neighbors){
			auto dx = neighbor->x - this->x;
			auto dy = neighbor->y - this->y;
			auto square = sqrt(dx*dx + dy*dy);

			avoid_x += dx / (square * square + 1.0);
			avoid_y += dy / (square * square + 1.0);
		
			cohesion_x += dx;
			cohesion_y += dy;
		
			consistency_x += neighbor->last_x;
			consistency_y += neighbor->last_y;

			count++;
		}
		if (count > 0){
			avoid_x /= count;
			avoid_y /= count;
			cohesion_x /= count;
			cohesion_y /= count;
			consistency_x /= count;
			consistency_y /= count;
			consistency_x /= count;
			consistency_y /= count;	
		}							
		avoid_x = 400.0f * avoid_x;
		avoid_y = 400.0f * avoid_y;

		cohesion_x = -cohesion_x / 10.0;
		cohesion_y = -cohesion_y / 10.0;

		random_x = (float(rand())/float((RAND_MAX)))*2.0-1.0;
		random_y = (float(rand())/float((RAND_MAX)))*2.0-1.0;
		auto sq = sqrt(random_x*random_x + random_y*random_y);
		random_x = random_x * 0.05 / sq;
		random_y = random_y * 0.05 / sq;

		auto mom_x = this->last_x;
		auto mom_y = this->last_y;

		auto ddx = PARAMS::avoidance * avoid_x + PARAMS::cohesion * cohesion_x + PARAMS::consistency * consistency_x + PARAMS::randomness * random_x + PARAMS::momentum * mom_x;
		auto ddy = PARAMS::avoidance * avoid_y + PARAMS::cohesion * cohesion_y + PARAMS::consistency * consistency_y + PARAMS::randomness * random_y + PARAMS::momentum * mom_y;

		auto dis = sqrt(ddx * ddx + ddy * ddy);
		if (dis > 0.0) {
			ddx = ddx / dis * PARAMS::jump;
			ddy = ddy / dis * PARAMS::jump;
		}

		this->last_x = ddx;
		this->last_y = ddy;

		auto loc_x = 0.0;
		auto loc_y = 0.0;

		if (this->x + ddx > PARAMS::width) {
			loc_x = this->x - ddx;
		} else {
			loc_x = this->x + ddx;
		}
		if (this->y + ddy > PARAMS::height) {
			loc_y = this->y - ddy;
		} else {
			loc_y = this->y + ddy;
		}

		if (loc_x < 0.0) {
			loc_x = 0.0;
		}
		if (loc_y < 0.0) {
			loc_y = 0.0;
		}
			
		// cout << "bef x: " << this->x << " y: " << this->y << endl;
		this->x = loc_x;
		this->y = loc_y;
		// cout << "aft x: " << loc_x << " y: " << loc_y << endl;

		auto round_x = (int)round(loc_x);
		auto round_y = (int)round(loc_y);

		if (round_x >= PARAMS::width) {
			round_x = PARAMS::width - 1;
		}
		if (round_y >= PARAMS::height) {
			round_y = PARAMS::height - 1;
		}

		auto index = round_x * PARAMS::width + round_y;
		// print coordinates of my patch
		auto dest = this->env;
		auto dest2 = dest->patches[index];
		// cout << "dest2: " << dest2 << "index" << index << endl;
		this->move(dest2, true);

}



