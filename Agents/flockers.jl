using Pkg;
Pkg.add("Agents")

using Agents, LinearAlgebra

mutable struct Bird <: AbstractAgent
    id::Int
    pos::NTuple{2,Float64}
    vel::NTuple{2,Float64}
    speed::Float64
    cohere_factor::Float64
    separation::Float64
    separate_factor::Float64
    match_factor::Float64
    visual_distance::Float64
end

function initialize_model(;
    n_birds = 100,
    speed = 1.0,
    cohere_factor = 0.03,
    separation = 4.0,
    separate_factor = 0.015,
    match_factor = 0.05,
    visual_distance = 5.0,
    extent = (100, 100),
    spacing = visual_distance / 1.5,
)
    space2d = ContinuousSpace(extent, spacing)
    model = ABM(Bird, space2d, scheduler = Schedulers.randomly)
    for _ in 1:n_birds
        vel = Tuple(rand(model.rng, 2) * 2 .- 1)
        add_agent!(
            model,
            vel,
            speed,
            cohere_factor,
            separation,
            separate_factor,
            match_factor,
            visual_distance,
        )
    end
    return model
end

function agent_step!(bird, model)
    # Obtain the ids of neighbors within the bird's visual distance
    neighbor_ids = nearby_ids(bird, model, bird.visual_distance)
    N = 0
    match = separate = cohere = (0.0, 0.0)
    # Calculate behaviour properties based on neighbors
    for id in neighbor_ids
        N += 1
        neighbor = model[id].pos
        heading = neighbor .- bird.pos

        # `cohere` computes the average position of neighboring birds
        cohere = cohere .+ heading
        if edistance(bird.pos, neighbor, model) < bird.separation
            # `separate` repels the bird away from neighboring birds
            separate = separate .- heading
        end
        # `match` computes the average trajectory of neighboring birds
        match = match .+ model[id].vel
    end
    N = max(N, 1)
    # Normalise results based on model input and neighbor count
    cohere = cohere ./ N .* bird.cohere_factor
    separate = separate ./ N .* bird.separate_factor
    match = match ./ N .* bird.match_factor
    # Compute velocity based on rules defined above
    bird.vel = (bird.vel .+ cohere .+ separate .+ match) ./ 2
    bird.vel = bird.vel ./ norm(bird.vel)
    # Move bird according to new velocity and speed
    move_agent!(bird, model, bird.speed)
end

model = initialize_model()
@time step!(model, agent_step!, 100)
