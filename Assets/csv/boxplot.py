import plot
import pandas as pd
import matplotlib.pyplot as plt
import numpy as np


def getNAsList(n, df):
    return df.loc[df['n'] == n]['frames'].tolist()


if __name__ == "__main__":
    # settings
    pos = [2, 6, 10, 20, 40]

    # read data
    csv_data_net = pd.read_csv('boids2_19596.csv', sep=';')
    csv_data_solo = pd.read_csv('net_19596.csv', sep=';')
    csv_data_boids = pd.read_csv('solo_coop_19596.csv', sep=';')
    data_net = list(map(lambda n: getNAsList(n, csv_data_net), pos))
    data_solo = list(map(lambda n: getNAsList(n, csv_data_solo), pos))
    data_boids = list(map(lambda n: getNAsList(n, csv_data_boids), pos))

    # create plot
    fig, ax = plt.subplots(dpi=200)
    meanlineprops = dict(linestyle='--', linewidth=1, color='black')
    boxp1 = ax.boxplot(data_net, positions=[1, 4, 7, 10, 13], whis=[10, 90], labels=pos, showfliers=False, showmeans=True, meanline=True, meanprops=meanlineprops, patch_artist=True)
    boxp2 = ax.boxplot(data_solo, positions=[2, 5, 8, 11, 14], whis=[10, 90], labels=pos, showfliers=False, showmeans=True, meanline=True, meanprops=meanlineprops, patch_artist=True)
    boxp3 = ax.boxplot(data_boids, positions=[3, 6, 9, 12, 15], whis=[10, 90], labels=pos, showfliers=False, showmeans=True, meanline=True, meanprops=meanlineprops, patch_artist=True)

    # plot settings
    ax.set_xticklabels(pos)
    ax.set_xticks([2, 5, 8, 11, 14])
    ax.set(xlabel='Schwarmgröße', ylabel='Zeit (in Simulationsschritten)',
           title="")
    ax.yaxis.grid(True)

    # fill with colors
    colors = ['red', 'blue', 'green']
    for boxp, color in zip((boxp1, boxp2, boxp3), colors):
        for patch in boxp['boxes']:
            patch.set_facecolor(color)

        for patch in boxp['medians']:
            patch.set_color('black')

    ax.legend([boxp1["boxes"][0], boxp2["boxes"][0], boxp3["boxes"][0]], ['Boids', 'Netz', 'SoloCoop'], loc='upper right')

    # finalize
    plt.show()


