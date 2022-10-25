#include "cppyabm/bases.h"
#include "cppyabm/mesh.h"
#include <random>
#include <vector>
#include <chrono>

struct PARAMS{
	static unsigned steps;
	static int height;
	static int width;
	static float density;
	
};
unsigned PARAMS::steps = 10;
// unsigned PARAMS::steps = 10;
int PARAMS::height=10;
int PARAMS::width=10;

float PARAMS::density=0.7;

struct ForestFire;
struct Tree;
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


struct MyPatch: public Patch<ForestFire,Tree,MyPatch>{

	using basePatch = Patch<ForestFire,Tree,MyPatch>;
	// using basePatch::basePatch;
	MyPatch(shared_ptr<ForestFire> env,MESH_ITEM mesh_item):basePatch(env,mesh_item){
		this->setup();
	}
	void setup(){
	}
	
};

struct Tree: public Agent<ForestFire,Tree,MyPatch>{

	using baseAgent = Agent<ForestFire,Tree,MyPatch>;
	// using baseAgent::baseAgent;
	Tree(shared_ptr<ForestFire> env, string class_name):baseAgent(env,class_name){
		// 0 - empty 1 - green 2 - burning 3 - burned
		for (int i = 0; i< PARAMS::height; i++) {
			for (int j = 0; j< PARAMS::width; j++) {
				if (random::randrange(0,1) < PARAMS::density) {
					if (j == 0) {
						this->array[i][j] = 2;
					} else {
						this->array[i][j] = 1;
					}
				} else {
					this->array[i][j] = 0;
				}
			}
		}
		// this->type = 0;
	}
	
	void ff_step();
	void step(){
		// this->random_move();
		this->ff_step();
	}
	// int type;
	//int x;
	//int y;
	int arr[PARAMS::height][PARAMS::width];
};


struct ForestFire: public Env<ForestFire,Tree,MyPatch>{

	using baseEnv = Env<ForestFire,Tree,MyPatch>;
	// using baseEnv::baseEnv;

	ForestFire():Env<ForestFire,Tree,MyPatch>(){
	}
	void reset(unsigned iter_i = 0){
		this->agents.clear();
		this->patches.clear();
		this->iter_i = iter_i;
		this->data = {{"memory",{}}};
		auto mesh = space::grid2(1, 1, 1, true);
		this->setup_domain(mesh);
		this->setup_ForestFire("Tree");
	}
	void setup_ForestFire(string class_name){
		
		// cout<<"number: "<<number<<endl;
		// for (unsigned i=0; i < PARAMS::width; i++){
		// 	for (unsigned j=0; j < PARAMS::height; j++) {
		auto a =this->generate_agent(class_name);

		//auto dest_index = random::choice(this->patches_keys);
		auto dest_index = 0;
		// cout<<"dest_index: "<<dest_index<<endl;

		auto dest = this->patches[dest_index];
		this->place_agent(dest,a,true);
	}
			
			
			
			
		
	shared_ptr<Tree> generate_agent(string agent_name){
		shared_ptr<Tree> obj;

		obj = make_shared<Tree>(this->shared_from_this(),agent_name);
		// random probability
		// if (random::randrange(0,1) < PARAMS::density){
		// 	// if i'm on first column
		// 	if (y==0) {
		// 		obj->type = 2;
		// 	}
		// 	else{
		// 		obj->type = 1;
		// 	}
		// }
		// obj->x = i;
		// obj->y = y;
		this->agents.push_back(obj);
		return obj;
	}
	shared_ptr<MyPatch> generate_patch(MESH_ITEM mesh_item){
		
		auto patch_obj = make_shared<MyPatch>(this->shared_from_this(),mesh_item);
		this->patches[mesh_item.index] = patch_obj;
		return patch_obj;
	}

	void step(int count_step){


		// for (unsigned i=0; i < PARAMS::width; i++){
		// 	for (unsigned j=0; j < PARAMS::height; j++) {
		// 		auto a = this->agents[i*PARAMS::width+j];
		// 		if (a->type == 0)
		// 			cout << "X-";
		// 		else if (a-> type == 1)
		// 			cout << "G-";
		// 		else if (a-> type == 2)
		// 			cout << "B-";
		// 		else if (a-> type == 3)
		// 			cout << "R-";
		// 	}
		// 	cout << endl;
		// }
		// cout << endl;
		this->agents[0]->step();
		// for (int i = this->agents.size()-1; i>=0; i--){
		// 	if (this->agents[i]->y <= count_step + 1){
		// 		this->agents[i]->step();
		// 	}
		// }
		// this->step_patches();
		// this->collect_output();
		//this->update();
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
		for (unsigned i=0; i<=PARAMS::steps; i++) {
			if (i > PARAMS::width + 1)
				break;
			//cout << "----- step: " << i << endl;
			this->step(i);
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

inline void ForestFire::output(DataType &results,float cpu_time){
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


inline void Tree::ff_step(){
		auto neighbors = this->get_patch()->find_neighbor_agents(false);	
		// cout << "neighbors size: " << neighbors.size() << endl;
		if (this->type == 3 || this->type == 0) {
			return;
		}

		if (this->type == 2) {
			// cout << "burning to burned" << endl;
			this->type = 3;
			return;
		}

		if (this->type == 1) {
			// cout << "green" << endl;
			for (auto &neighbor: neighbors){
				// cout << "neighbor type: " << neighbor->type << endl;
				if (neighbor->type == 2){
					// cout << "green to burning" << endl;
					this->type = 2;
					break;
				}
			}
		}
		
		

}



