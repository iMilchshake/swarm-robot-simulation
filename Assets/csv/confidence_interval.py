import pandas as pd
import matplotlib
import matplotlib.pyplot as plt
import numpy as np
import statsmodels.api as sm
import math

# read csv file
df = pd.read_csv('boids2_1.csv', sep=';')

# group data by cluster size
df_grouped = df.groupby(['n'])

# get grouped stats
grouped_stats = df.groupby(['n'])['frames'].agg(['mean', 'count', 'std'])
print(grouped_stats)

# calculate confidence interval
ci95_hi = []
ci95_lo = []
for i in grouped_stats.index:
    m, c, s = grouped_stats.loc[i]
    ci95_hi .append(m + 1.96*s/math.sqrt(c))
    ci95_lo.append(m - 1.96*s/math.sqrt(c))

grouped_stats['ci95_hi'] = ci95_hi
grouped_stats['ci95_lo'] = ci95_lo
print(grouped_stats)

# remove index for easier plotting
grouped_stats = grouped_stats.reset_index()

# plot
fig, ax = plt.subplots()
ax.plot(grouped_stats['n'], grouped_stats['mean'], label='mean', c='blue')
#ax.bar(grouped_stats['n'], grouped_stats['ci95_hi'] - grouped_stats['ci95_lo'], 0.8, grouped_stats['ci95_lo'], label='95% confidence interval')
ax.errorbar(grouped_stats['n'], grouped_stats['mean'], yerr=(grouped_stats['ci95_hi'] - grouped_stats['ci95_lo'])/2, ecolor='black', fmt='o', elinewidth=1, capsize=5, label='95% confidential interval')

ax.set(xlabel='cluster size', ylabel='frames',
       title='amount frames it took a robot cluster to find the goal')
ax.grid()
ax.legend()
plt.show()


