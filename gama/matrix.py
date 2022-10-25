#write a matrix of 0 and 1 to a 
import sys
import random
import numpy as np

#set the size of the matrix from cli
size = int(sys.argv[1])

#initialize the matrix
matrix = np.zeros((size,size))

# 0 empty
# 1 green
# 2 burning
# 3 burned

#set the initial state of the matrix
for i in range(size):
    for j in range(size):
        if random.random() < 0.7:
            if (j == 0):
                matrix[i][j] = 2
            else:
                matrix[i][j] = 1

#write matrix to csv file
def write_matrix(matrix):
    with open("matrix" + sys.argv[1] + ".csv", "w") as f:
        for i in range(size):
            for j in range(size):
                f.write(str(int(matrix[i][j])))
                if j < size - 1:
                    f.write(",")
            f.write("\n")
        

write_matrix(matrix)
