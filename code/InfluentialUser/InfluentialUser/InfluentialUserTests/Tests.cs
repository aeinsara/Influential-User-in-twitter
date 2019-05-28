using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace InfluentialUser.Tests
{
    [TestClass()]
    public class Tests
    {
        [TestMethod()]
        [DeploymentItem("TestData", "InfluentialUser_TestData")]
        public void SolveBetweennessTest()
        {
            Processor p = new Betweenness("TD1");
            TestTools.RunLocalTest("InfluentialUser",
                p.Process,
                p.TestDataName,
                p.Verifier,
                false,
                excludedTestCases: p.ExcludedTestCases
                );
        }

        [TestMethod()]
        [DeploymentItem("TestData", "InfluentialUser_TestData")]
        public void SolveDegreeTest()
        {
            Processor p = new Degree("TD2");
            TestTools.RunLocalTest("InfluentialUser",
                p.Process,
                p.TestDataName,
                p.Verifier,
                false,
                excludedTestCases: p.ExcludedTestCases
                );
        }
    }
}
