using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneClassClassification.Benchmarks;
using OneClassClassification.Components;
using OneClassClassification.Data;
using OneClassClassification.Rescalers;

namespace OneClassClassificationTests.Components
{
    [TestClass()]
    public class DataGeneratorTests
    {
        [TestMethod()]
        public void GenerateFeasibleExamplesTest()
        {
            GlobalVariables.BenchmarkName = "simplex";
            GlobalVariables.Dimensions = 3;

            var dataGenrator = new DataGenerator
            {
                BoundryRescaler = new MinMaxBoundryRescaler()
            };
            Assert.AreEqual(dataGenrator.GenerateFeasibleExamples(1000).Count, 1000);
        }
    }
}