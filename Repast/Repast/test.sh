#!/bin/sh

MODEL="config_${1}"
OUTPUT=$2
PARTIAL="par_${1}"

for ((i=0; i<3 ; i++)); do

    echo "${i}^ Run"
    java -jar batch_runner.jar -hl -r -c $MODEL >> $PARTIAL
    rm -r /tmp/simphony*
    rm -r /tmp/*unrolled_params_1*
    rm -r /tmp/isis_localhost*

done

grep "Run Time:" $PARTIAL > $OUTPUT
grep "Initialization Time:" $PARTIAL >> $OUTPUT
if [ -f $PARTIAL ]; then
    rm $PARTIAL
    echo "partial file removed"
fi