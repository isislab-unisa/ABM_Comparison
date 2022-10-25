# Model design
import agentpy as ap

# Visualization
import matplotlib.pyplot as plt
import seaborn as sns
import IPython

class ForestModel(ap.Model):

    def setup(self):

        # Create agents (trees)
        n_trees = int(self.p['Tree density'] * (self.p.size**2))
        trees = self.agents = ap.AgentList(self, n_trees)

        # Create grid (forest)
        self.forest = ap.Grid(self, [self.p.size]*2, track_empty=True)
        self.forest.add_agents(trees, random=True, empty=True)

        # Initiate a dynamic variable for all trees
        # Condition 0: Alive, 1: Burning, 2: Burned
        self.agents.condition = 0

        # Start a fire from the left side of the grid
        unfortunate_trees = self.forest.agents[0:self.p.size, 0:2]
        unfortunate_trees.condition = 1

    def step(self):

        # Select burning trees
        burning_trees = self.agents.select(self.agents.condition == 1)

        # Spread fire
        for tree in burning_trees:
            for neighbor in self.forest.neighbors(tree):
                if neighbor.condition == 0:
                    neighbor.condition = 1 # Neighbor starts burning
            tree.condition = 2 # Tree burns out

        # Stop simulation if no fire is left
        if len(burning_trees) == 0:
            self.stop()

    def end(self):

        # Document a measure at the end of the simulation
        burned_trees = len(self.agents.select(self.agents.condition == 2))
        self.report('Percentage of burned trees',
                    burned_trees / len(self.agents))

parameters = {
    'Tree density': 0.7, # Percentage of grid covered by trees
    'size': 1131, # Height and length of the grid
    'steps': 200,
}


exp = ap.Experiment(ForestModel, parameters, iterations=1, randomize=True)
results = exp.run()
exp = ap.Experiment(ForestModel, parameters, iterations=1, randomize=True)
results = exp.run()
exp = ap.Experiment(ForestModel, parameters, iterations=1, randomize=True)
results = exp.run()
exp = ap.Experiment(ForestModel, parameters, iterations=1, randomize=True)
results = exp.run()
exp = ap.Experiment(ForestModel, parameters, iterations=1, randomize=True)
results = exp.run()