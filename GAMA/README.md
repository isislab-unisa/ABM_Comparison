## [GAMA](https://gama-platform.org/)

### Prerequisites
- GAMA
	- Download executable from [GAMA website](https://gama-platform.org/).

### Model Execution
Each model can be executed using the GAMA IDE. Copy the model's file from this repository to the GAMA workspace.

### Benchmark
The results have been collected using the experiment function provided by GAMA. 

### Notes
The initialization of the ForestFire (ff) model's matrix is based on a csv file. The Python script `matrix.py` generates the matrix randomly providing the size as argument.

```console 
python3 matrix.py 100
```
