# ABM_Comparison
Code used for the performance comparison for the article "Experimenting with Agent-based Model Simulation Tools"

# ABMSurvey

In this repository, we tested some ABM frameworks available:

    ActressMas
    Agent.jl
    AgentPy
    CppyABM
    Gama
    krABMaga
    MASON
    MESA
    NetLogo
    Repast

for each model not provided by the framework, we tried our best to develop a coherent model. 
In the following table, :white_check_mark: means the model is already available, :x: otherwise. 

| abm/model  | flockers | wsg  | schelling | ff   |
|------------|----------|------|-----------|------|
| krABMaga   | :white_check_mark:     | :white_check_mark: | :white_check_mark:      | :white_check_mark: |
| Agent.jl   | :white_check_mark:     | :white_check_mark: | :white_check_mark:      | :white_check_mark: |
| MASON      | :white_check_mark:     | :white_check_mark: | :x:     | :x: |
| Repast     | :white_check_mark:     | :white_check_mark: | :white_check_mark:      | :x: |
| Netlogo    | :white_check_mark:     | :white_check_mark: | :white_check_mark:      | :white_check_mark: |
| Mesa       | :white_check_mark:     | :white_check_mark: | :white_check_mark:      | :white_check_mark: |
| ActressMas | :x:     | :white_check_mark: | :x:      | :x: |
| Agentpy    | :white_check_mark:     | :x: | :white_check_mark:      | :white_check_mark: |
| CppyABM    | :x:     | :white_check_mark: | :x:      | :x: |
| GAMA       | :white_check_mark:     | :x: | :white_check_mark:      | :x: |


In each directory, you can find a Readme which contains all the informations needed to execute the tests. Pay attention, you have to correctly install frameworks in order to run the benchmarks. Feel free to contact us if you have any questions about scripts and/or tests or if you can't reproduce our results.

