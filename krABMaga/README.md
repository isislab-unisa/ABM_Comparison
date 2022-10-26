## [krABMaga](https://krabmaga.github.io/)

### Prerequisites
- Rust
- krABMaga 
  - The dependency to install the library is already within the Cargo.toml file of each model

### Model Execution
Within the folder of the desired model to run execute the following command.

```bash
cargo run --release
```

### Benchmark
Copy and paste the script file `test.sh` in the model's subfolder and run it specifying the desired number of agents to simulate.

```bash
bash test.sh 1000
```

### Notes
