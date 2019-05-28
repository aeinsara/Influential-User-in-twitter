using System;
using System.Collections.Generic;
using System.Linq;
using TestCommon;

namespace InfluentialUser
{
    public class Betweenness : Processor
    {
        public Betweenness(string testDataName) : base(testDataName)
        {
            //this.ExcludeTestCaseRangeInclusive(1, 2);
            //this.ExcludeTestCaseRangeInclusive(4, 50);
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long[]>)Solve);

        Graph G;

        public long[] Solve(long NodeCount, long[][] edges)
        {
            G = new Graph(NodeCount);
            G.BuildGraph(edges);

            // Find shortest path between each pair of node
            for (int i = 0; i < NodeCount; i++)
            {
                for (int j = 0; j < NodeCount; j++)
                {
                    if (i != j)
                        BFS(i, j); // 1-based
                }
            }

            // return betweenness number of nodes in graph
            return G.betweenness;
        }

        public void BFS(long From, long To)
        {
            List<long> result = new List<long>();
            bool isFound = false;
            Queue<long> queue = new Queue<long>();
            queue.Enqueue(From);
            G.visited[From] = true;

            while (queue.Any())
            {
                long temp = queue.Dequeue();
                foreach (var i in G.Neighbors[From])
                {
                    if (G.visited[i])
                        continue;
                    G.prev[i] = temp;
                    queue.Enqueue(i);
                    G.visited[i] = true;
                    if (i == To)
                    {
                        isFound = true;
                        break;
                    }
                }
                if (isFound)
                    break;
            }
            if (isFound)
            {
                result.Add(To);
                result.Add(From);
                To = G.prev[To];
                while (To != From)
                {
                    result.Add(To);
                    To = G.prev[To];
                }
            }

            //Update Betweenness foreach vertex
            foreach (var r in result)
            {
                G.betweenness[r]++;
            }


            //Reinitialize
            for (int i = 0; i < G.NodeCount; i++)
            {        
                G.visited[i] = false;
                G.prev[i] = -1;
            }
        }
    }
}
