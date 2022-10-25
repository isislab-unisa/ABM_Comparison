## [AgentPy](https://agentpy.readthedocs.io/en/latest/)

### Prerequisites
- Python
- AgentPy

```bash
pip install agentpy
```

### Model Execution
Each model can be executed using the following command replacing `<file-name.py>` with the desired model's name and `<file-results.txt>` with the name where results will be saved (e.g. `python flockers.py > 1000.txt`).

```python
python file-name.py > file-results.txt
```

### Benchmark
The tests have been conducted running the model multiple times and using the script `script.sh` to retrieve the results.

```bash
bash script.sh
```

### Notes
The framework provides specific fuctions to run multiple experiments. However, the resulting time is additive on each simulations executed.
