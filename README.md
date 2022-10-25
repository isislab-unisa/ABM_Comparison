# Experimenting with Agent-based Model Simulation Tools: ABM Comparison
This repository contains the code developed for the performance comparison included in the article "*Experimenting with Agent-based Model Simulation Tools*" submitted to Applied Science.

**Included ABM tools**
- [**ActressMas**](https://github.com/florinleon/ActressMas)
- [**Agents.jl**](https://juliadynamics.github.io/Agents.jl/stable/)
- [**AgentPy**](https://agentpy.readthedocs.io/en/latest/)
- [**CppyABM**](https://pypi.org/project/cppyabm/)
- [**GAMA**](https://gama-platform.org/)
- [**krABMaga**](https://krabmaga.github.io/)
- [**MASON**](https://cs.gmu.edu/~eclab/projects/mason/)
- [**Mesa**](https://mesa.readthedocs.io/en/latest/)
- [**NetLogo**](https://ccl.northwestern.edu/netlogo/index.html)
- [**Repast**](https://repast.github.io)

The repository contains a dedicated directory for each of the tool tested. The directory contains the implementation of the different examples and the scripts used to run the benchmark.

### Hardware configuration
* **OS**: Ubuntu 22.04 LTS x86_64 
* **Kernel version**: 5.15.0-48-generic 
* **CPU**: Intel i7-8700T (12) @ 4.000GHz
* **GPU**: NVIDIA GeForce GTX 1050 Mobile
* **Memory**: 16 GB

### Models
The efficiency and scale of each tool has been tested in terms of execution time and workload capacity using the following four models:
* **Flockers**: a model by Craig Reynolds simulating a flock's flying behavior; a continuous toroidal space contains the agents moving according to defined rules.
* **Wolf, Sheep, and Grass (WSG)**: a multi-agent model simulating the population dynamics of predators and prey coexisting in a shared environment.
* **Schelling**: a simple segregation model where the agents are placed in a two-dimensional grid moving depending on the status of their neighbors.
* **ForestFire**: a stochastic cellular automaton model reproducing a spreading forest fire.

Implementation provided by the ABM tools' authors have been used if available. Otherwise, the model is been developed from scratch following the platforms guidelines, documentation, and examples. Each model is been implemented to be as similar as possible among the different tools included; however, the differences between the tools introduce some inevitably variance. 

The following table summarize which models is provided by the ABM tools' authors (:white_check_mark:) and which has been developed from scratch (:x:).

| :arrow_down:Tool/Model:arrow_right: | **Flockers**       | **WSG**            | **Schelling**      | **ForestFire**     |
|-------------------------------------|--------------------|--------------------|--------------------|--------------------|
| **ActressMAS**                      | :x:                | :white_check_mark: | :x:                | :x:                |
| **AgentPy**                         | :white_check_mark: | :x:                | :white_check_mark: | :white_check_mark: |
| **Agents.jl**                       | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| **CppyABM**                         | :x:                | :white_check_mark: | :x:                | :x:                |
| **GAMA**                            | :white_check_mark: | :x:                | :white_check_mark: | :x:                |
| **krABMaga**                        | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| **MASON**                           | :white_check_mark: | :white_check_mark: | :x:                | :x:                |
| **Mesa**                            | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| **Netlogo**                         | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| **Repast**                          | :white_check_mark: | :white_check_mark: | :white_check_mark: | :x:                |

### Configurations
Frameworks' performance has been tested with different models configurations, starting with a field 100x100 , 1000 agents, and 200 steps, keeping an agent density of 10%. The subsequent configurations are obtained by doubling the number of agents and changing the field dimensions to preserve the initial agent:

* Agents: 1000 - Field: 100x100
* Agents: 2000 - Field: 141x141
* Agents: 4000 - Field: 200x200
* Agents: 8000 - Field: 282x282
* Agents: 16000 - Field: 400x400
* Agents: 32000 - Field: 565x565
* Agents: 128000 - Field: 1131x1131

The ForestFire model maintains a density of 70%.

### Important note
To correctly use the script provided it is required that the tools is correctly installed with the relative prerequisites.

### Reference
Antelmi, A.; Cordasco, G.; D’Ambrosio, G.; De Vinco, D.; Spagnuolo, C. Experimenting with Agent-based Model Simulation Tools. Applied Sciences 2022.

**Bibtex**

```@Article{Antelmi2022,
AUTHOR = {Antelmi, Alessia and Cordasco, Gennaro and D’Ambrosio, Giuseppe and De Vinco, Daniele and Spagnuolo, Carmine},
TITLE = {Experimenting with Agent-based Model Simulation Tools},
JOURNAL = {Applied Sciences},
VOLUME = {},
YEAR = {2022},
NUMBER = {},
ARTICLE-NUMBER = {},
DOI = {}
}```
