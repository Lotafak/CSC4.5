using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneClassClassification.Benchmarks;
using OneClassClassification.Data;

namespace OneClassClassificationTests.Benchmarks
{
    [TestClass()]
    public class CubeBenchmarkTests
    {
        [TestMethod()]
        public void CubeBenchmarkContructorTest()
        {
            GlobalVariables.Dimensions = 2;
            var benchmark = new CubeBenchmark(GlobalVariables.Dimensions);

            Assert.AreEqual(benchmark.Constraints.Length, 4);

            GlobalVariables.Dimensions = 3;
            benchmark = new CubeBenchmark(GlobalVariables.Dimensions);

            Assert.AreEqual(benchmark.Constraints.Length, 6);
        }

        [TestMethod()]
        public void AssignCubeExampleBool2DTest()
        {
            var benchmark = new CubeBenchmark(2);

            var falseValues = new[] {
                new[] { 1.0, 12.2 },
                new[] { 6.1, 1.1 },
                new[] { 5.5, 0.0 },
                new[] {6.6, 12.2},
                new[] {6.00001, 5.9999}
            };
            var trueValues = new[]
            {
                new[] {1.1, 6.6},
                new[] {5.9, 11.9},
                new[] {1.1, 2.1}
            };

            foreach( var item in falseValues )
            {
                Assert.AreEqual(benchmark.AssignExampleBool(item), false);
            }
            foreach( var item in trueValues )
            {
                Assert.AreEqual(benchmark.AssignExampleBool(item), true);
            }
        }

        [TestMethod()]
        public void GenerateCubeExample2DTest()
        {
            var ben = new CubeBenchmark(2);
            var rng = new MersenneTwister();

            for( int i = 0; i < 10000; i++ )
            {
                var vect = ben.GenerateExample(rng);
                for( int j = 0; j < vect.Length; j++ )
                {
                    Assert.AreEqual((j + 1) + 2 * (j + 1) * GlobalVariables.R > vect[j], true);
                    Assert.AreEqual((j + 1) - (j + 1) * GlobalVariables.R < vect[j], true);
                }
            }
        }
    }
}