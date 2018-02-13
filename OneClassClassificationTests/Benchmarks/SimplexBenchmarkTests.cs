using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneClassClassification.Benchmarks;
using OneClassClassification.Data;
using OneClassClassification.Models;
using OneClassClassification.Utils;

namespace OneClassClassificationTests.Benchmarks
{
    [TestClass()]
    public class SimplexBenchmarkTests
    {
        [TestMethod()]
        public void GenerateSimplexExamplesTest()
        {
            var mt = new MersenneTwister(0);
            var benchmark = new SimplexBenchmark(3);
            for ( var i = 0; i < 1000; i++ )
            {
                Assert.IsTrue(benchmark.GenerateExample(mt).All(x => x > -1 && x < 7));
            }
        }

        [TestMethod()]
        public void SimplexBenchmarkConstructorTest()
        {
            var simplexCot = MathHelper.Cot(Math.PI / 12);
            var simplexTan = Math.Tan(Math.PI / 12);

            var benchmark = new SimplexBenchmark(2);

            var constraints = new double[3][];
            constraints[0] = new[] { simplexCot, -simplexTan };
            constraints[1] = new[] { -simplexTan, simplexCot };
            constraints[2] = new[] { 1.0, 1.0 };

            for ( var i = 0; i < benchmark.Constraints.Length; i++ )
            {
                for ( var j = 0; j < benchmark.Constraints[0].Length; j++ )
                {
                    Assert.AreEqual(benchmark.Constraints[i][j], constraints[i][j]);
                }
            }
        }

        [TestMethod()]
        public void GetConstraintForCalculationsTest()
        {
            var benchmark = new SimplexBenchmark(2);

            var list = new List<Constraint>
            {
                new Constraint { Axis = 0 },
                new Constraint { Axis = 1}
            };

            var constraints = new double[2][];
            constraints[0] = new[] { 1.0, 0.0 };
            constraints[1] = new[] { 0.0, 1.0 };

            var fromMethod = benchmark.GetConstraintsForCalculations(list);

            for ( var i = 0; i < fromMethod.Length; i++ )
            {
                for ( var j = 0; j < fromMethod[i].Length; j++ )
                {
                    Assert.AreEqual(constraints[i][j], fromMethod[i][j]);
                }
            }
        }
    }
}