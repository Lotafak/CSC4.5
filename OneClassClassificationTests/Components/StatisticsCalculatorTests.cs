using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneClassClassification.Components;
using OneClassClassification.Models;
using OneClassClassification.Benchmarks;
using OneClassClassification.Data;

namespace OneClassClassificationTests.Components
{
    [TestClass()]
    public class StatisticsCalculatorTests
    {
        private static readonly double[] Truth =        { 1.0, 1.0, 0.0, 0.0 };
        private static readonly double[] Predictions =  { 1.0, 0.0, 0.0, 1.0 };

        [TestMethod()]
        public void AngleBetweenWeightVectors2DTest()
        {
            var list1 = new[]
            {
                new[] { 10.0, 10.0 },
                new[] { 10.0, 15.0 },
                new[] { 15.0, 10.0 },
                new[] { 15.0, 15.0 }
            };

            var list2 = new[]
            {
                new[] {14.92, 14.82},
                new[] {14.99, 14.82}
            };

            var results = new[,]
            {
                { 0.003362462 , 0.005702722},
                { 0.200758022, 0.203098282},
                { 0.194033098, 0.191692837},
                { 0.003362462, 0.005702722}
            };

            for ( var i = 0; i < results.GetLength(0); i++ )
            {
                for ( var j = 0; j < results.GetLength(1); j++ )
                {
                    Assert.AreEqual(
                        Math.Round(StatisticsCalculator.AngleBetweenVectors(list1[i], list2[j]), 9),
                        results[i, j]);
                }
            }
        }

        [TestMethod()]
        public void AngleBetweenWeightVectors3DTest()
        {
            var list1 = new[]
            {
                new[] {10.0, 10.0, 10},
                new[] {10.0, 15.0, 10},
                new[] {15.0, 10.0, 10},
                new[] {15.0, 15.0, 10}
            };

            var list2 = new[]
            {
                new[] {14.92, 14.82, 10},
                new[] {14.99, 14.82, 10}
            };

            var results = new[,]
            {
                { 0.171629101, 0.172589753 },
                { 0.189056835, 0.19135961 },
                { 0.183431982, 0.181833547 },
                { 0.004533724, 0.005707512 }
            };

            for ( var i = 0; i < results.GetLength(0); i++ )
            {
                for ( var j = 0; j < results.GetLength(1); j++ )
                {
                    Assert.AreEqual(
                        Math.Round(StatisticsCalculator.AngleBetweenVectors(list1[i], list2[j]), 9),
                        results[i, j]);
                }
            }
        }

        [TestMethod()]
        public void CalculateAccuracyTest()
        {
            var truth = new[]
            {
                new[] {1.0, 1.0},
                new[] {1.0, 0.0}
            };
            var predictions = new[]
            {
                new[] {1.0, 1.0},
                new[] {0.0, 0.0}
            };

            var results = new[,]
            {
                {1.0, 0.0},
                {0.5, 0.5}
            };

            for ( var i = 0; i < truth.Length; i++ )
            {
                for ( var j = 0; j < predictions.Length; j++ )
                {
                    Assert.AreEqual(
                        StatisticsCalculator.CalculateAccuracy(truth[i], predictions[j]),
                        results[i, j]);
                }
            }
        }

        [TestMethod()]
        public void CalculateCubeMeanAngleTest()
        {
            GlobalVariables.Dimensions = 2;
            var benchmark = new CubeBenchmark(GlobalVariables.Dimensions);

            var list2 = new List<Constraint>
            {
                new Constraint {Axis = 0, Value = 14.92},
                new Constraint {Axis = 0, Value = 14.99},
                new Constraint {Axis = 1, Value = 14.82}
            };
            var temp = Math.Round(StatisticsCalculator.CalculateMeanAngle(list2, benchmark), 9);

            Assert.AreEqual(
                temp,
                0//,$"{temp}"
                );
        }

        [TestMethod()]
        public void CalculateTruePositiveTest()
        {
            Assert.AreEqual(StatisticsCalculator.CalculateTruePositive(Truth, Predictions), 1);
        }

        [TestMethod()]
        public void CalculateFalsePositiveTest()
        {
            Assert.AreEqual(StatisticsCalculator.CalculateFalsePositive(Truth, Predictions), 1);
        }

        [TestMethod()]
        public void CalculateTrueNegativeTest()
        {
            Assert.AreEqual(StatisticsCalculator.CalculateTrueNegative(Truth, Predictions), 1);
        }

        [TestMethod()]
        public void CalculateFalseNegativeTest()
        {
            Assert.AreEqual(StatisticsCalculator.CalculateFalseNegative(Truth, Predictions), 1);
        }

        [TestMethod()]
        public void CalculateAccuracyWithParametersTest()
        {
            Assert.AreEqual(StatisticsCalculator.CalculateAccuracy(Truth,Predictions), 0.5);
        }
    }
}