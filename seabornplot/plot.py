import pandas as pd
import seaborn as sns
import matplotlib.pyplot as plt


df = pd.read_csv("G:/seabornplot/plot.csv", error_bad_lines=False)

sns.catplot(x="Number Of Nodes in Graph",y="RunTime (Millisecond)",kind='bar',hue='Algorithm', height=4,data=df)

plt.ylim(0, None)
plt.xlim(-0.5, None)

plt.savefig("figure")
plt.show()
