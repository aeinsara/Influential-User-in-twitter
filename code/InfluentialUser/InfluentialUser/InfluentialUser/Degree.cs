using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace InfluentialUser
{
    public class Degree : Processor
    {

        public Degree(string testDataName) : base(testDataName)
        {
            //this.ExcludeTestCaseRangeInclusive(1, 2);
            //this.ExcludeTestCaseRangeInclusive(4, 50);
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long[]>)Solve);

        public Graph G;

        public long[] Solve(long NodeCount, long[][] edges)
        {
            G = new Graph(NodeCount);
            G.BuildGraph(edges);

            // Find shortest path between each pair of node
            for (int i = 0; i < NodeCount; i++)
            {
                for (int j = 0; j < NodeCount; j++)
                {
                    if (G.Neighbors[j].Contains(i))
                        G.In_Degree[i]++;                      
                }
            }

            // return betweenness number of nodes in graph
            return G.In_Degree;
        }
    }
}
