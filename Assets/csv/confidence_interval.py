import pandas as pd
import matplotlib
import matplotlib.pyplot as plt
import numpy as np
import statsmodels.api as sm
import math
from scipy.optimize import curve_fit


def readCSV(path: str):
    # read csv file
    df = pd.read_csv(path, sep=';')

    # group data by cluster size
    df_grouped = df.groupby(['n'])

    # get grouped stats
    grouped_stats = df.groupby(['n'])['frames'].agg(['mean', 'count', 'std'])

    # calculate confidence interval
    ci95_hi = []
    ci95_lo = []
    for i in grouped_stats.index:
        m, c, s = grouped_stats.loc[i]
        ci95_hi.append(m + 1.96 * s / math.sqrt(c))
        ci95_lo.append(m - 1.96 * s / math.sqrt(c))

    grouped_stats['ci95_hi'] = ci95_hi
    grouped_stats['ci95_lo'] = ci95_lo

    # remove index for easier plotting
    grouped_stats = grouped_stats.reset_index()

    # return
    return grouped_stats


def plot(files: list, colors: list, show_error=False):
    # get stats
    stats = list(map(readCSV, files))  # calculate stats for each file
    print(stats)

    # plot
    fig, ax = plt.subplots()
    for i in range(len(stats)):
        ax.plot(stats[i]['n'], stats[i]['mean'], label=files[i], c=colors[i])
        if show_error:
            ax.errorbar(stats[i]['n'], stats[i]['mean'], yerr=(stats[i]['ci95_hi'] - stats[i]['ci95_lo']) / 2, ecolor='black', fmt='o', elinewidth=1, capsize=5)

    ax.set(xlabel='cluster size', ylabel='simulation steps',
           title='amount of sim. steps it took a robot cluster to find the goal')
    ax.grid()
    ax.legend()
    plt.show()


if __name__ == "__main__":
    f = ['boids2_c7918.csv', 'net_8ada3.csv']  # files to plot
    c = ['red', 'blue', 'green']
    plot(f, c)  # plot
    plot([f[0]], [c[0]], show_error=True)
    plot([f[1]], [c[1]], show_error=True)
