#include "cppyabm/bases.h"
#include "cppyabm/mesh.h"
#include <random>
#include <vector>
#include <chrono>

struct PARAMS{
	static unsigned steps;
	static int height;
	static int width;
	static int num_cells;
	// sheep_reproduce=0.04,
	static int similarity;
	
};
unsigned PARAMS::steps = 200;
// unsigned PARAMS::steps = 10;
int PARAMS::height=282;
int PARAMS::width=282;
int PARAMS::num_cells=8000;
int PARAMS::similarity=3;

struct Schelling;
struct Cell;
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


struct MyPatch: public Patch<Schelling,Cell,MyPatch>{

	using basePatch = Patch<Schelling,Cell,MyPatch>;
	// using basePatch::basePatch;
	MyPatch(shared_ptr<Schelling> env,MESH_ITEM mesh_item):basePatch(env,mesh_item){
		this->setup();
	}
	void setup(){
	}
	
};

struct Cell: public Agent<Schelling,Cell,MyPatch>{

	using baseAgent = Agent<Schelling,Cell,MyPatch>;
	// using baseAgent::baseAgent;
	Cell(shared_ptr<Schelling> env, string class_name):baseAgent(env,class_name){
		this->index = 0;
	}
	
	void random_move();
	void cell_step();

	void step(){
		// this->random_move();
		this->cell_step();
	}
	int index;
	int type;
};


struct Schelling: public Env<Schelling,Cell,MyPatch>{

	using baseEnv = Env<Schelling,Cell,MyPatch>;
	// using baseEnv::baseEnv;

	Schelling():Env<Schelling,Cell,MyPatch>(){
	}
	void reset(unsigned iter_i = 0){
		this->agents.clear();
		this->patches.clear();
		this->iter_i = iter_i;
		// this->data = {{"Sheep",{}},{"Wolf",{}},{"memory",{}}};
		this->data = {{"memory",{}}};
		auto mesh = space::grid2(PARAMS::height, PARAMS::width,1, true);
		this->setup_domain(mesh);
		this->setup_Schelling("Flock");
	}
	void setup_Schelling(string class_name){
		int number = PARAMS::num_cells;
		// cout<<"number: "<<number<<endl;
		for (unsigned i=0; i < number; i++){
			auto a =this->generate_agent(class_name);
			auto patch = this->find_empty_patch();
			a->index = patch->index;
			this->place_agent(patch,a,true);
		}
	}
	shared_ptr<Cell> generate_agent(string agent_name){
		shared_ptr<Cell> obj;

		obj = make_shared<Cell>(this->shared_from_this(),agent_name);
		if (random::randrange(0,1) < 0.5) {
			obj->type = 1;
		} else {
			obj->type = 0;
		}
		this->agents.push_back(obj);
		return obj;
	}
	shared_ptr<MyPatch> generate_patch(MESH_ITEM mesh_item){
		
		auto patch_obj = make_shared<MyPatch>(this->shared_from_this(),mesh_item);
		this->patches[mesh_item.index] = patch_obj;
		return patch_obj;
	}
	void step(){
		
		// int count = 0;
		// int i = 0;
		// for (auto patch:this->patches){
		// 	// cout << "i: " << i << endl;
		// 	//get each agent in patch
		// 	if (patch.second->agents.size() > 0){
		// 		for (auto agent:patch.second->get_agents()){
		// 			cout<< agent->type << "-";
		// 		}
		// 	} else {
		// 		cout << "x-";
		// 	}
			
		// 	i++;
		// 	if (i % 5 == 0){
		// 		cout << endl;
		// 	}
		// }
		// cout << endl;

		this->activate_random();
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

inline void Schelling::output(DataType &results,float cpu_time){
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

inline void Cell::random_move(){
	auto possible_patch = this->env->find_empty_patch();	
	auto in = possible_patch->index;
	this->index = in;
	this->move(possible_patch,true);
}

inline void Cell::cell_step(){
		auto neighbors = this->get_patch()->find_neighbor_agents(false);	
		// cout << "neighbors size: " << neighbors.size() << endl;
		int count_similar = 0;
		for (auto &neighbor: neighbors){
			if (neighbor->type == this->type) count_similar++;
			if (count_similar >= PARAMS::similarity) return;
		}
		// if (count_similar < PARAMS::similarity) {
			this->random_move();
		// }

}



