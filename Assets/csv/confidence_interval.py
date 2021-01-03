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

    # get grouped stats
    grouped_stats = df.groupby(['n'])['frames'].describe(percentiles=[0.1, 0.9])
    print(grouped_stats)
    # calculate confidence interval
    ci95_hi = []
    ci95_lo = []
    for i in grouped_stats.index:
        c, m, std, min_value, low_10, median, top_10, max_value = grouped_stats.loc[i]
        ci95_hi.append(m + 1.96 * std / math.sqrt(c))
        ci95_lo.append(m - 1.96 * std / math.sqrt(c))

    grouped_stats['ci95_hi'] = ci95_hi
    grouped_stats['ci95_lo'] = ci95_lo

    # remove index for easier plotting
    grouped_stats = grouped_stats.reset_index()

    # return
    return grouped_stats


def plot(files: list, colors: list, names: list, tit: str, t='mean', show_error=False, show_tmin=None):
    # get stats
    stats = list(map(readCSV, files))  # calculate stats for each file

    # plot
    fig, ax = plt.subplots(dpi=200)
    for i in range(len(stats)):
        print(files[i] + ": \n" + str(stats[i]))
        ax.plot(stats[i]['n'], stats[i][t], label=names[i], c=colors[i])
        if show_error:
            ax.errorbar(stats[i]['n'], stats[i]['mean'], yerr=(stats[i]['ci95_hi'] - stats[i]['ci95_lo']) / 2, ecolor='black', fmt='o', elinewidth=1, capsize=5)

    if show_tmin:
        ax.axhline(y=show_tmin, color='gray', label='$T_{min}$', linestyle='dotted')
        plt.yticks(list(range(0, 2250, 250)) + [show_tmin])

    plt.xticks(list(range(2, 42, 2)))
    plt.xlim(2, 40)


    plt.yticks()
    ax.set(xlabel='Schwarmgröße', ylabel='Zeit (in Simulationsschritten)',
           title=tit)
    ax.grid()
    ax.legend()
    plt.show()

if __name__ == "__main__":
    f = ['boids2_19596.csv', 'net_19596.csv', 'solo_coop_19596.csv']  # files to plot
    c = ['red', 'blue', 'green', "blue"]
    n = ['Boids', 'Netz', 'SoloCoop']
    plot(f, c, n, 'Durschnitt Anzahl Simulationsschritte bis zum finden des Zielobjektes', t='mean', show_tmin=98.546667)  # plot
    plot(f, c, n, 'Top 10% Anzahl Simulationsschritte bis zum finden des Zielobjektes', t='10%')  # plot
    #plot([f[0]], [c[0]], show_error=True)
    plot(['min_cheating_19596.csv'], ['blue'], ['min_cheat'], "bla", show_error=True)

