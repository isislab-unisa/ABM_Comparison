## AgentPy

- Install the library using pip

```bash
pip install agentpy
```
- You can use the functions provided by the framework to run multiple experiments. Be aware that the time showed at the end of simulations is added up for each repetition. We ran the model multiple times (eg. line 120-123 in flock.py) and we used the scripts provided in the folder to retrieve all the results at once. 

```python
python flock.py > 1000.txt
# next configuration
python flock.py > 2000.txt
```
```bash
bash script.sh
```





