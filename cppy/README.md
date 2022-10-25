## CppyABM

- These are the lines to build the cppy framework provided by the docs. If you have troubles while executing the 'make install' command, ignore it
    ```console
    git clone https://github.com/janursa/CppyABM && cd CppyABM && mkdir build && cd build && cmake .. -DCPP=TRUE && make install
    ```
- Copy the flockers folders from this repo into the CppyABM folder
```console
cd flockers && mkdir build && cd build && cmake .. && make && ./appABM
```
- change the parameters in the models.h file for desired configuration
- change the number of repetitions in batch_run.cpp file 
- if you want to write a new model, you may missing a dependencies considered fundamental by the library. 
add this line at the start of models.h file
```c++
#include <chrono>
``` 
