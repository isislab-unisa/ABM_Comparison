#!/bin/sh

OUTPUT=$1
PARTIAL="par_${1}"

for ((i=0; i<5 ; i++)); do

    echo "Iteration ${i}"
    { time cargo run --release >/dev/null 2>&1; } 2>> $PARTIAL
done

grep "real" $PARTIAL > $OUTPUT

if [ -f $PARTIAL ]; then
    rm $PARTIAL
    echo "partial file removed"
fi