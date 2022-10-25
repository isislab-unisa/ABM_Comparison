# bash

for ((i=1000; i<=128000; i=i*2)); do
    grep "Run time" $i.txt > out_$i
done