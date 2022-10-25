using Pkg;
Pkg.add("Agents")

using Agents
using Random

@agent Automata GridAgent{2} begin end

function forest_fire(; density = 0.6, griddims = (400, 400))
    space = GridSpace(griddims; periodic = false, metric = :euclidean)
    # The `trees` field is coded such that
    # Empty = 0, Green = 1, Burning = 2, Burnt = 3
    forest = ABM(Automata, space; properties = (trees = zeros(Int, griddims),))
    for I in CartesianIndices(forest.trees)
        if rand(forest.rng) < density
            # Set the trees at the left edge on fire
            forest.trees[I] = I[1] == 1 ? 2 : 1
        end
    end
    return forest
end

forest = forest_fire()

function tree_step!(forest)
    # Find trees that are burning (coded as 2)
    for I in findall(isequal(2), forest.trees)
        for idx in nearby_positions(I.I, forest)
            # If a neighbor is Green (1), set it on fire (2)
            if forest.trees[idx...] == 1
                forest.trees[idx...] = 2
            end
        end
        # Finally, any burning tree is burnt out (2)
        forest.trees[I] = 3
    end
end

@time Agents.step!(forest, dummystep, tree_step!, 200)
