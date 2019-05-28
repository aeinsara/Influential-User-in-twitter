using System;
using System.IO;
using System.Text;

namespace InfluentialUser
{
    class Program
    {
        static void Main(string[] args)
        {

            //TestDataGen();
        }

        private static int GoodMaxDegree(int NodeCount,
            double w1 = 1, double w2 = 1, double w3 = 1)
        {
            return (int)(
                   (NodeCount > 256 ?
                      w1 * Math.Log(NodeCount, 10) :
                      NodeCount > 15 ?
                        w2 * NodeCount / 4 :
                         w3 * NodeCount)
                    );
        }

        static void TestDataGen()
        {
            string testDataPathPrefix = @"..\..\..\Exam1Tests\TestData\TD1";
            int TestCaseCount = 50;
            int NodeCount = 5;

            Random rnd = new Random(0);

            for (int i = 2; i <= TestCaseCount; i++)
            {
                //Create and Generate a directed unweighted graph
                Graph g = new Graph(NodeCount, rnd);
                g.GenerateGraph(GoodMaxDegree(NodeCount, 0.5, 0.5, 0.5));

                string inFile = Path.Combine(testDataPathPrefix, $"In_{i}.txt");
                File.WriteAllText(inFile, g.ToUnitTestFormat(), Encoding.UTF8);
                Console.WriteLine($"Created {inFile}");

                if (NodeCount < 100)
                {
                    string webgraphVizFile = Path.Combine(testDataPathPrefix, $"In_{i}.webgraphviz");
                    File.WriteAllText(webgraphVizFile, g.ToWebGraphViz());
                    Console.WriteLine($"Created {webgraphVizFile}");
                }


                //Increase number of nodes in graph in each testcase
                NodeCount = (int)(NodeCount * 1.1) + 1;
            }

            Betweenness p = new Betweenness("TD1");
            TestCommon.TestTools.RunLocalTest(
                "Exam1", p.Process, p.TestDataName, true, testDataPathPrefix);
        }

    }
}
