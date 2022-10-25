## Agents.jl

- install Julia on your machine
- you can locally install the library through the package manager or programmatically inside the file

```julia
using Pkg; Pkg.add("Agents")
```

- you can easily run the models with this command
```console 
julia namefile.jl
```

- for the benchmarks, we collect the results through the script test_agent.sh

```console 
bash test_agents.sh output
```

- if you don't install locally the library before the benchmark, you may want to exclude the first execution that will have increased time for building.
