## [Agents.jl](https://juliadynamics.github.io/Agents.jl/stable/)

### Prerequisites
- Julia
- Agent.jl

```julia
using Pkg; Pkg.add("Agents")
```
### Model Execution
Each model can be executed using the following command replacing `<file-name.jl>` with the desired model's name (e.g. `julia flockers.jl`).

```console 
julia file-name.jl
```

### Benchmark
The results have been collected using the script `test_agent.sh` that requires to change the model's file name to test (line 9).

```console 
bash test_agents.sh output
```

### Notes
First execution could take more time if the Agents.jl library is not been previously installed.
