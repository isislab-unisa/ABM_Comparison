import agentpy as ap
import numpy as np
import random
# Visualization
import matplotlib.pyplot as plt
import IPython

class Wsg(ap.Agent):

    """
    Animal
    """

    def setup(self, flag):
        self.dead = False

        if (flag == 'repr_sheep'):
            self.type = 'sheep'
            self.step = self.step_sheep
            self.energy = random.randint(10, 20)
            return
        elif (flag == 'repr_wolf'):
            self.type = 'wolf'
            self.step = self.step_wolf
            self.energy = random.randint(10, 20)
            return

        if self.model.nprandom.random() < 0.6:
            self.type = 'sheep'
            self.step = self.step_sheep
            self.energy = random.randint(0, 2*self.p.gain_energy_sheep)
        else:
            self.type = 'wolf'
            self.step = self.step_wolf
            self.energy = random.randint(0, 2*self.p.gain_energy_wolf)

    def setup_pos(self, space):
        self.space = space
        self.neighbors = space.neighbors
        self.pos = space.positions[self]

    def step_sheep(self, grass):
        if (self.dead):
            return (False, None, 'sheep dead')

        x = self.pos[0] + random.randint(-1, 1)
        y = self.pos[1] + random.randint(-1, 1)
        self.space.move_to(self, (x, y))
        
        
        
        #eat
        if (grass[self.pos[0], self.pos[1]] == 20):
            # print("grass", grass[self.pos[0], self.pos[1]])
            self.energy += self.p.gain_energy_sheep
            grass[self.pos[0]][self.pos[1]] = 0

        self.energy -= 1
        #die
        if (self.energy <= 0):
            #remove agent
            self.dead = True

        #reproduce
        if (self.model.nprandom.random() < self.p.sheep_reproduce):
            self.energy /= 2
            return (True, self.pos, 'sheep_repr')

        
        #self.space.move_by(self, self.p.momentum_prob)

    def step_wolf(self, grass):
        if (self.dead):
            return (False, None, 'wolf dead')
        
        #move
        x = self.pos[0] + (random.randint(0, 1)*2 - 1)
        y = self.pos[1] + (random.randint(0, 1)*2 - 1)
        self.space.move_to(self, (x, y))
        
        

        #eat
        nbs = self.neighbors(self, distance=1)
        for nb in nbs:
            if (nb.type == 'sheep') and (nb.dead == False):
                # print("wolf eat sheep at", nb.pos)
                nb.dead = True
                self.energy += self.p.gain_energy_wolf
                break

        self.energy -= 1
        #die
        if (self.energy <= 0):
            self.dead = True
        
        #reproduce
        if (self.model.nprandom.random() < self.p.wolf_reproduce):
            self.energy /= 2
            return (True, self.pos, 'wolf_repr')
        
class WsgModel(ap.Model):

    def setup(self):
        self.numstep = 0
        """ Initializes the agents and network of the model. """
        self.grass = np.zeros((self.p.size, self.p.size), dtype=int)
        self.space = ap.Grid(self, shape=[self.p.size]*self.p.ndim, torus=True)
        self.agents = ap.AgentList(self, self.p.population, Wsg, flag='repr_no')
        self.space.add_agents(self.agents, random=True)
        self.agents.setup_pos(self.space)

        for i in range(self.p.size):
            for j in range(self.p.size):
                if self.nprandom.random() < 0.5:
                    self.grass[i][j] = 20
                else:
                    self.grass[i][j] = random.randint(0, 20)

    def step(self):

        """ Defines the models' events per simulation step. """
        # print("step ", self.numstep + 1)
        all_agents = self.agents.step(self.grass)
        for ag in all_agents:
            if ag != None and ag[0] == True:
                # self.agents.add(Wsg(self, ag[1], ag[2]))
                new_agents = ap.AgentList(self, 1, Wsg, flag=ag[2])
                self.agents.append(new_agents)
                self.space.add_agents(new_agents, random=True)
                
        self.agents.setup_pos(self.space)

        # sheep_dead = 0
        # wolf_dead = 0
        # sheep_alive = 0
        # wolf_alive = 0
        # for a in self.space.agents: 
        #     if a.type == 'sheep':
        #         if a.dead == True:
        #             sheep_dead += 1
        #         else:
        #             sheep_alive += 1
        #     else:
        #         if a.dead == True:
        #             wolf_dead += 1
        #         else:
        #             wolf_alive += 1
        
        # print("sheep dead", sheep_dead)
        # print("wolf dead", wolf_dead)
        # print("sheep alive", sheep_alive)
        # print("wolf alive", wolf_alive)
                    
        #step grass
        for i in range(self.p.size):
            for j in range(self.p.size):
                if (self.grass[i][j] < 20):
                    self.grass[i][j] += 1
        self.numstep += 1
        # print("------------------------")




parameters = {
    'size': 1131,
    'seed': 123,
    'steps': 200,
    'ndim': 2,
    'population': 128000,
    'neigh': 2,
    'border_strength': 0.5,
    'full_grown': 20,
    'gain_energy_sheep': 4,
    'gain_energy_wolf': 20,
    'sheep_reproduce': 0.2,
    'wolf_reproduce': 0.1,
}

# results = model.run()
exp = ap.Experiment(WsgModel, parameters, iterations=1, randomize=True)
results = exp.run()
exp = ap.Experiment(WsgModel, parameters, iterations=1, randomize=True)
results = exp.run()
exp = ap.Experiment(WsgModel, parameters, iterations=1, randomize=True)
results = exp.run()
exp = ap.Experiment(WsgModel, parameters, iterations=1, randomize=True)
results = exp.run()
exp = ap.Experiment(WsgModel, parameters, iterations=1, randomize=True)
results = exp.run()



