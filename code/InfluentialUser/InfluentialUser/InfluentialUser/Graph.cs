using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfluentialUser
{
    /// <summary>
    /// Represents a directed unweighted graph structure
    /// </summary>

    public class Graph
    {
        public long NodeCount;
        public List<long>[] Neighbors;
        public bool[] visited;
        public long[] betweenness;
        public long[] prev;
        public long[] In_Degree;

        protected Random _Rnd = null;
        protected Random Rnd { get => _Rnd ?? (_Rnd = new Random(0)); }
        protected bool Prob(double p) => Rnd.NextDouble() < p;

        public IEnumerable<Tuple<long, long>> Edges
        {
            get
            {
                for (long i = 0; i < Neighbors.Length; i++)
                    foreach (var j in Neighbors[i])
                        yield return Tuple.Create(i, j);
            }
        }

        public Graph(long nodeCount, Random rnd = null)
        {
            this._Rnd = rnd;
            NodeCount = nodeCount;

            // Contains the child nodes for each vertex of the graph
            // Mark all the vertices as not visited 
            // Initialize betwweeness number of each node to zero
            // Initialize prev number of each node to -1
            Neighbors = new List<long>[nodeCount];
            visited = new bool[nodeCount];
            betweenness = new long[nodeCount];
            prev = new long[nodeCount];
            In_Degree = new long[nodeCount];

            for (int i = 0; i < nodeCount; i++)
            {
                Neighbors[i] = new List<long>();
                visited[i] = false;
                betweenness[i] = 0;
                prev[i] = -1;
                In_Degree[i] = 0;
            }               
        }

        public void AddEdge(long u, long v)
        {
            //Adds new edge from u to v and vice versa
            Neighbors[u].Add(v);
        }

        public void BuildGraph(long[][] edges)
        {
            foreach (var e in edges)
            {
                AddEdge(e[0] - 1, e[1] - 1); // Input is 1 based
            }

            foreach (var node in Neighbors)
            {
                node.Sort();
                node.Reverse();
            }
            
        }

        public string ToUnitTestFormat() =>
            $"{NodeCount}\n" + string.Join("\n",
                Edges.OrderBy(n => Rnd.Next())
                     .Select(e => $"{e.Item1 + 1} {e.Item2 + 1}"));

        public string ToWebGraphViz() =>
            "digraph G {\n" +
                string.Join("\n",
                    Enumerable.Range(1, Neighbors.Length)
                        .Select(n => $"   \"{n}\";")
                    ) + "\n" +
                string.Join("\n",
                 Edges.OrderBy(n => Rnd.Next())
                      .Select(e => $"   \"{e.Item1 + 1}\" -> \"{e.Item2 + 1}\";")) +
            "\n}";

        public void GenerateGraph(int? maxDegree = null)
        {
            if (maxDegree != null)
                maxDegree = Math.Min(
                    Neighbors.Length - 1,
                    maxDegree.Value);

            for (int i = 0; i < this.NodeCount; i++)
            {
                //Number of outGoing edges from node i
                long edgesNum = Rnd.Next(0, maxDegree.Value + 1); //+1, upper bound is exclusive

                int maxTry = (int)this.NodeCount;
                while (edgesNum > 0 && maxTry > 0)
                {
                    //Another end of the edge
                    long v = Rnd.Next(0, (int)NodeCount);
                    if (!Neighbors[i].Contains(v) && v != i)
                    {
                        AddEdge(i, v);
                        edgesNum--;
                    }
                    maxTry--;
                }
                maxTry = (int)this.NodeCount;
            }
        }
    }
}
