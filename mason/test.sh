#!/bin/sh

OUTPUT=$1
PARTIAL="par_${1}"

# you can clone the mason repository on your local machine and build yourself the mason.jar file following the documentation
# be sure to use the correct path to the jar file and the simulation name
# the folder of flocker and schelling simulations are an indicator of the files used for our benchmarks
# check the configuration parameters inside the mason folder that you cloned

for ((i=0; i<5 ; i++)); do

    echo "Iteration ${i}"    
    # you can substitute the path to the desired app to test
    # -for set the step of the simulation
    # -quiet dont print the useless information
    # { time java -jar mason.jar sim.app.wsg.Wsg -for 200 -quiet; } &>> $PARTIAL
    { time java -jar mason.jar sim.app.forestfire.ForestFire -for 200 -quiet; } &>> $PARTIAL

done

# this commands are used to filter the output and retrieve the
# total time of each simulation and the times of step
grep "real" $PARTIAL > $OUTPUT
grep "Elapsed" $PARTIAL >> $OUTPUT

if [ -f $PARTIAL ]; then
    rm $PARTIAL
    echo "partial file removed"
fi