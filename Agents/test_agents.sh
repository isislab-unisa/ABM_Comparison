#!/bin/sh

OUTPUT=$1
PARTIAL="par_${1}"

for ((i=0; i<5; i++)); do
    echo "${OUTPUT} - Iteration ${i}"
    # change here the name/path of the file
    { time julia flockers.jl; } &>> $PARTIAL
done

# this commands are used to filter the output and retrieve the
# total time of each simulation and the times of step
echo "TOTAL" > $OUTPUT
grep "real" $PARTIAL >> $OUTPUT
echo "------" >> $OUTPUT
echo "STEP" >> $OUTPUT
grep "seconds" $PARTIAL >> $OUTPUT

if [ -f $PARTIAL ]; then
    rm $PARTIAL
    echo "partial file removed"
fi