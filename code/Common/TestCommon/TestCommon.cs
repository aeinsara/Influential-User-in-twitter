using Microsoft.SolverFoundation.Solvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TestCommon
{
    public static class TestTools
    {
        public static readonly char[] IgnoreChars = new char[] { '\n', '\r', ' ' };
        public static readonly char[] NewLineChars = new char[] { '\n', '\r' };

        public static void RunLocalTest(
            string AssignmentName,
            Func<string, string> Processor,
            string TestDataName,
            Action<string, string> Verifier,
            bool VerifyResultWithoutOrder = false,
            HashSet<int> excludedTestCases = null) =>
                            RunLocalTest(
                                    AssignmentName,
                                    Processor,
                                    TestDataName,
                                    false,
                                    null,
                                    int.MaxValue,
                                    Verifier ?? (VerifyResultWithoutOrder ?
                                        (Action<string, string>)FileVerifierIgnoreOrder :
                                        (Action<string, string>)FileVerifier),
                                    excludedTestCases);

        public static void RunLocalTest(
            string AssignmentName,
            Func<string, string> Processor,
            string TestDataName = null,
            bool saveMode = false,
            string testDataPathOverride = null,
            int maxTestCases = int.MaxValue,
            Action<string, string> Verifier = null,
            HashSet<int> excludedTestCases = null)
        {
            Verifier = Verifier ?? FileVerifier;
            string testDataPath = $"{AssignmentName}_TestData";
            if (!string.IsNullOrEmpty(TestDataName))
                testDataPath = Path.Combine(testDataPath, TestDataName);

            if (!string.IsNullOrEmpty(testDataPathOverride))
                testDataPath = testDataPathOverride;

            Assert.IsTrue(Directory.Exists(testDataPath));
            string[] inFiles = Directory.GetFiles(testDataPath, "*In_*.txt");

            Assert.IsTrue(inFiles.Length > 0);

            int testCaseNumber = 0;
            List<string> failedTests = new List<string>();
            TimeSpan totalTime = new TimeSpan(0);
            foreach (var inFile in inFiles.OrderBy(x => FileNumber(x)))
            {
                if (excludedTestCases != null &&
                    excludedTestCases.Contains(FileNumber(inFile)))
                {
                    Console.WriteLine($"Excluding test case: {Path.GetFileName(inFile)}");
                    continue;
                }

                if (++testCaseNumber > maxTestCases)
                    break;

                Stopwatch sw;
                string outFile;
                try
                {
                    var lines = File.ReadAllText(inFile);

                    sw = Stopwatch.StartNew();
                    string result = Processor(lines);
                    sw.Stop();
                    totalTime += sw.Elapsed;

                    result = result.Trim(IgnoreChars);
                    if (saveMode)
                    {
                        outFile = inFile.Replace("In_", "Out_");
                        File.WriteAllText(outFile, result);
                        Console.WriteLine($"{Path.GetFileName(Path.GetDirectoryName(inFile))}: {Path.GetFileName(inFile)}=>{Path.GetFileName(outFile)}");
                        continue;
                    }

                    Verifier(inFile, result);

                    Console.WriteLine($"Test Passed ({sw.Elapsed.ToString()}): {inFile}");
                }
                catch (Exception e)
                {
                    failedTests.Add($"Test failed for input {inFile}: {e.Message}");
                    Console.WriteLine($"Test Failed: {inFile}");
                }
            }

            Assert.IsTrue(failedTests.Count == 0,
                $"{failedTests.Count} out of {inFiles.Length} tests failed: " +
                $"{new string(string.Join("\n", failedTests).Take(1000).ToArray())}");

            Console.WriteLine($"All {inFiles.Length} tests passed: {totalTime.ToString()}.");
        }

        private static void FileVerifier(string inputFileName, string testResult) =>
            FileVerifierSpecifyOrder(inputFileName, testResult, false);

        private static void FileVerifierIgnoreOrder(string inputFileName, string testResult) =>
            FileVerifierSpecifyOrder(inputFileName, testResult, true);

        private static void FileVerifierSpecifyOrder(string inputFileName, string testResult, bool ignoreOrder)
        {
            string outFile = inputFileName.Replace("In_", "Out_");
            Assert.IsTrue(File.Exists(outFile));

            var expectedLines = File.ReadAllLines(outFile)
                .Select(line => line.Trim(IgnoreChars)) // Ignore white spaces 
                .Where(line => !string.IsNullOrWhiteSpace(line)); // Ignore empty lines

            testResult = testResult.Replace("\r\n", "\n");

            if (ignoreOrder)
            {
                expectedLines = expectedLines.OrderBy(x => x);
                testResult = string.Join("\n",
                    testResult.Split(NewLineChars, StringSplitOptions.RemoveEmptyEntries)
                              .OrderBy(x => x));
            }
            string expectedResult = string.Join("\n", expectedLines);

            Assert.AreEqual(expectedResult, testResult, $"TestCase:{Path.GetFileName(inputFileName)}");
        }

        public static int FileNumber(string fileName)
        {
            int start = fileName.LastIndexOf('_');
            int end = fileName.LastIndexOf('.');
            string fileNumber = fileName.Substring(start + 1, end - start - 1);
            return int.Parse(fileNumber);
        }

        public static string Process(string inStr, Func<long, long[][], long[]> processor)
        {
            long count;
            long[][] data;
            ParseGraph(inStr, out count, out data);

            return string.Join(" ", processor(count, data));
        }

        public static void ParseGraph(string inStr, out long count, out long[][] data)
        {
            var lines = inStr.Split(NewLineChars, StringSplitOptions.RemoveEmptyEntries);
            count = int.Parse(lines.First());
            data = ReadTree(lines.Skip(1));
        }
        private static long[][] ReadTree(IEnumerable<string> lines)
        {
            return lines.Select(line => line.Split(IgnoreChars, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(n => long.Parse(n))
                                     .ToArray()
                 ).ToArray();
        }
    }
}
