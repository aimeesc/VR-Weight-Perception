import argparse
from os import listdir
from os.path import isfile, join, splitext
import pandas as pd
import numpy as np

parser = argparse.ArgumentParser(description='Process experiment files.')
parser.add_argument('path', metavar='P', type=str, help='Path for the .csv file with experiment data')

args = parser.parse_args()

csv_files = [join(args.path, f) for f in listdir(args.path) if isfile(join(args.path, f)) and f[-3:] == 'csv' and f[-14:] != '_processed.csv' and f[-13:] != '_duration.csv']

experiments_total_time = []

for file in csv_files:
    print()
    print('Processing file:', file)

    print()

    df = pd.read_csv(file, header=None)

    print(df.head())

    id_column = 0
    time_column = len(df.columns) - 1
    distance_column = len(df.columns)
    mse_column = distance_column + 1

    df[distance_column] = 0.0
    df[mse_column] = 0.0
    total_time = 0.0

    for i, row in df.iterrows():
        distance = 0.0
        s_error = 0.0
        values = row.values.tolist()

        for j in range(2, 6):
            for k in range(6, 10):
                if values[j] == values[k]:
                    distance += abs((k - 6) - (j - 2))

                if (k - 6) == (j - 2):
                    s_error += (values[k] - values[j])**2

        df.at[i, distance_column] = distance
        df.at[i, mse_column] = s_error / 4

        total_time += values[time_column]

    print()
    print('After processing:')

    print(df.head())

    name, extension = splitext(file)

    df.to_csv(name + '_processed' + extension, header=False, index=False)
    print()
    print('Result saved at', name + '_processed' + extension)

    experiments_total_time.append([df.at[0, id_column], total_time])

np.savetxt(args.path + '/exp_duration.csv', np.asarray(experiments_total_time), delimiter=',', fmt='%d,%.3f')

print()
print('Experiment total time in', args.path + '/exp_duration.csv')

