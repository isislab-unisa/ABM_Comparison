
## [MASON](https://cs.gmu.edu/~eclab/projects/mason/)

### Prerequisites
- Java
- [MASON](https://cs.gmu.edu/~eclab/projects/mason)
- Any IDE with Java support

## Model execution
- Import the model's folder from this repository within the MASON folder.
- Run the java file containing the `main` function.

### Benchmark
- Import the model's folder from this repository within the MASON folder.
- Generate the JAR file of the model using the built-in function of the IDE.
- Run the script `test.sh` replacing the path at line 18 with the corresponding JAR file's path.

```console 
bash test.sh output
```