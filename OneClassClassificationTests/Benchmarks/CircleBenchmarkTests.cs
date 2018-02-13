using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneClassClassification.Benchmarks;
using OneClassClassification.Data;
using OneClassClassification.Models;

namespace OneClassClassificationTests.Benchmarks
{
    [TestClass()]
    public class CircleBenchmarkTests
    {
        [TestMethod()]
        public void CircleBenchmarkConstructorTest()
        {
            var benchmark = new CircleBenchmark(2);
            var constraint = new[] { 1.0, -2.0, 1.0, -4.0 };

            for ( var i = 0; i < constraint.Length; i++ )
            {
                Assert.AreEqual(benchmark.Constraints[0][i], constraint[i]);
            }

            benchmark = new CircleBenchmark(3);
            constraint = new[] { 1.0, -2.0, 1.0, -4.0, 1.0, -6.0 };

            for ( var i = 0; i < constraint.Length; i++ )
            {
                Assert.AreEqual(benchmark.Constraints[0][i], constraint[i]);
            }
        }

        [TestMethod()]
        public void GetConstraintForCalculationsTest()
        {
            GlobalVariables.Dimensions = 2;
            var benchmark = new CircleBenchmark(GlobalVariables.Dimensions);

            var constraints = new double[2][];
            constraints[0] = new [] {0.0, 1.0, 0.0, 0.0};
            constraints[1] = new [] {0.0, 0.0, 0.0, 1.0};

            var list = new List<Constraint>
            {
                new Constraint {Axis = 0, Value = 10},
                new Constraint {Axis = 1, Value = 15}
            };
            var resultConstraints = benchmark.GetConstraintsForCalculations(list);
            for (var i = 0; i < constraints.Length; i++)
            {
                for (var j = 0; j < constraints[i].Length; j++)
                {
                    Assert.AreEqual(resultConstraints[i][j],constraints[i][j]);
                }
            }
        }

        [TestMethod()]
        public void AssignCircleExampleBool2DTest()
        {
            var ben = new CircleBenchmark(2);

            var trueValues = new[]
            {
                new [] { 5.9, 2.0 },
                new [] { 0.0, 6.0 },
                new[] {-3.9, 2.0}
            };
            var falseValues = new[]
            {
                new[] {6.0, 2.0 },
                new[] {1.0, 7.0},
                new[] {-4.0, 3.0}
            };

            foreach( var item in trueValues )
            {
                Assert.AreEqual(ben.AssignExampleBool(item), true);
            }
            foreach( var item in falseValues )
            {
                Assert.AreEqual(ben.AssignExampleBool(item), false);
            }

        }

        [TestMethod()]
        public void GenerateCircleExample2DTest()
        {
            var ben = new CircleBenchmark(2);
            var rng = new MersenneTwister();

            for( int i = 0; i < 10000; i++ )
            {
                var vector = ben.GenerateExample(rng);
                for( int j = 0; j < vector.Length; j++ )
                {
                    Assert.AreEqual(vector[j] > j + 1 - 2 * GlobalVariables.R, true);
                    Assert.AreEqual(vector[j] < j + 1 + 2 * GlobalVariables.R, true);
                }
            }
        }
    }
}