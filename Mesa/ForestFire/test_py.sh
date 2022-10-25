#!/bin/sh

OUTPUT=$1
PARTIAL="par_${1}"

for ((i=0; i<3 ; i++)); do

     echo "${OUTPUT} - Iteration ${i}"
    { time python3.9 benchmark.py; } &>> $PARTIAL
    #  { time python3.9 benchmark.py >/dev/null 2>&1; } 2>> $PARTIAL
done

grep "real" $PARTIAL > $OUTPUT
echo "------" >> $OUTPUT
grep "Mesa ForestFire" $PARTIAL >> $OUTPUT

if [ -f $PARTIAL ]; then
    rm $PARTIAL
    echo "partial file removed"
fi