## [CppyABM](https://pypi.org/project/cppyabm/)

### Prerequisites
- CppyABM 
```console
git clone https://github.com/janursa/CppyABM && cd CppyABM && mkdir build && cd build && cmake .. -DCPP=TRUE && make install
```

### Model Execution
Each model can be executed copying the corresponding folder into the CppyABM folder created when cloning the repository.

```console
cd <folder-name> && mkdir build && cd build && cmake .. && make && ./appABM
```

### Notes
- Configuration parameters must be set within the `folder-name/Cpp/models.h` file.
- Repetitions number must be set within the `folder-name/Cpp/batch_run.cpp` file.
- Any new model requires the addition of the following dependencies at the start of the `models.h` file.

```c++
#include <chrono>
``` 


