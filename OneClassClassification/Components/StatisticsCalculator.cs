using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Accord.MachineLearning.DecisionTrees;
using Accord.Math;
using Accord.Math.Distances;
using Gurobi;
using OneClassClassification.Benchmarks;
using OneClassClassification.Data;
using OneClassClassification.Models;

namespace OneClassClassification.Components
{
    /// <summary>
    /// Collects project's modules to gather single run statistics
    /// </summary>
    public class StatisticsCalculator
    {
        public double FixedJaccardIndex { get; set; }
        public double FixedAccuracy { get; set; }

        public int FixedTruePositive { get; set; }
        public int FixedTrueNegative { get; set; }
        public int FixedFalsePositive { get; set; }
        public int FixedFalseNegative { get; set; }

        public int HomogenousTruePositive { get; set; }
        public int HomogenousTrueNegative { get; set; }
        public int HomogenousFalsePositive { get; set; }
        public int HomogenousFalseNegative { get; set; }

        public double MeanAngle { get; set; }
        public double ExecutionTime { get; set; }

        public int DataLength => GlobalVariables.TestExamples * 2;

        public double HomogenousJaccardIndex { get; private set; }
        public double HomogenousAccuracy { get; private set; }

        public void CalculateStatistics( DecisionTree decisionTree, List<Constraint> synthesizedConstraints, DataGenerator dataGenerator )
        {
            var timer = new Stopwatch();
            timer.Start();

            FixedTestDataStatistics(dataGenerator.FixedDistributionTestData, decisionTree);

            HomogenousTestDataStatistics(dataGenerator.HomogenousTestData, decisionTree);

            // Calculate Mean Angle 
            try
            {
                MeanAngle = CalculateMeanAngle(synthesizedConstraints,
                    dataGenerator.Benchmark);
            }
            catch( InvalidEnumArgumentException ex )
            {
                GlobalVariables.ErrorLog.AppendLine($"{ex.Message}\n");
            }

            timer.Stop();
            ExecutionTime = timer.ElapsedMilliseconds;
        }

        /// <summary>
        /// Calculate statistics with use of fixed discribution test data
        /// </summary>
        /// <param name="testData"></param>
        /// <param name="decisionTree"></param>
        private void FixedTestDataStatistics( List<double[]> testData, DecisionTree decisionTree )
        {
            EvaluateDecisionTree(testData, decisionTree, out double[] truth, out double[] predictions);

            // Calculate Jaccard index
            FixedJaccardIndex = CalculateJaccardIndex(truth, predictions);

            FixedAccuracy = CalculateAccuracy(truth, predictions);

            //Confusion matrix calculations
            FixedTruePositive = CalculateTruePositive(truth, predictions);
            FixedFalsePositive = CalculateFalsePositive(truth, predictions);
            FixedTrueNegative = CalculateTrueNegative(truth, predictions);
            FixedFalseNegative = CalculateFalseNegative(truth, predictions);
        }

        /// <summary>
        /// Calculate statistics with use of homogenous distribution test data
        /// </summary>
        /// <param name="testData"></param>
        /// <param name="decisionTree"></param>
        private void HomogenousTestDataStatistics( List<double[]> testData, DecisionTree decisionTree )
        {
            EvaluateDecisionTree(testData, decisionTree, out double[] truth, out double[] predictions);

            HomogenousJaccardIndex = CalculateJaccardIndex(truth, predictions);

            HomogenousAccuracy = CalculateAccuracy(truth, predictions);

            HomogenousTruePositive = CalculateTruePositive(truth, predictions);
            HomogenousFalsePositive = CalculateFalsePositive(truth, predictions);
            HomogenousTrueNegative = CalculateTrueNegative(truth, predictions);
            HomogenousFalseNegative = CalculateFalseNegative(truth, predictions);
        }

        /// <summary>
        /// Prepare truth and prediction for given test set
        /// </summary>
        /// <param name="testData"></param>
        /// <param name="decisionTree"></param>
        /// <param name="truth"></param>
        /// <param name="predictions"></param>
        private static void EvaluateDecisionTree( List<double[]> testData, DecisionTree decisionTree, out double[] truth, out double[] predictions )
        {
            // Prepare test data
            var input = testData.ToArray()
                            .GetColumns(Vector.Range(0, GlobalVariables.Dimensions));

            truth = testData.Select(t => t[t.Length - 1]).ToArray();
            predictions = decisionTree
                .Decide(input)
                .To<double[]>();
        }

        /// <summary>
        /// Calculate Jaccard index for a given input (Wrapper for Accord.NET Jaccard 
        /// Distance method)
        /// </summary>
        /// <param name="truth">Array of truth</param>
        /// <param name="predictions">Array of predictions</param>
        /// <returns><see cref="double"/> value of Jaccard index (1 - Jaccard Distance)</returns>
        public static double CalculateJaccardIndex( double[] truth, double[] predictions )
        {
            var jaccard = new Jaccard();
            return 1 - jaccard.Distance(truth, predictions);
        }

        /// <summary>
        /// Calculate Accuracy for truth and prediction. Accuracy is calculated as amount of 
        /// same results (truth == prediction) divided by number of examples
        /// </summary>
        /// <param name="truth"></param>
        /// <param name="predictions"></param>
        /// <returns></returns>
        public static double CalculateAccuracy( double[] truth, double[] predictions )
        {
            var acc = 0.0;
            for( var i = 0; i < truth.Length; i++ )
            {
                acc += truth[i] == predictions[i] ? 1 : 0;
            }
            return acc / truth.Length;
        }

        /// <summary>
        /// Calculates TP measure. True class is equal to prediction
        /// </summary>
        /// <param name="truth"></param>
        /// <param name="predictions"></param>
        /// <returns></returns>
        public static int CalculateTruePositive( double[] truth, double[] predictions )
        {
            return truth.Select(( t, i ) => (t == 1.0 && predictions[i] == 1.0) ? 1 : 0).Sum();
        }

        /// <summary>
        /// Calculates FP measure. Negative class classified as positive
        /// </summary>
        /// <param name="truth"></param>
        /// <param name="predictions"></param>
        /// <returns></returns>
        public static int CalculateFalsePositive( double[] truth, double[] predictions )
        {
            return truth.Select(( t, i ) => (t == 0.0 && predictions[i] == 1.0) ? 1 : 0).Sum();
        }

        /// <summary>
        /// Calculates TN measure. Negative class is equal to prediction
        /// </summary>
        /// <param name="truth"></param>
        /// <param name="predictions"></param>
        /// <returns></returns>
        public static int CalculateTrueNegative( double[] truth, double[] predictions )
        {
            return truth.Select(( t, i ) => (t == 0.0 && predictions[i] == 0.0) ? 1 : 0).Sum();
        }

        /// <summary>
        /// Calculates FN measure. Positive class classified as negative
        /// </summary>
        /// <param name="truth"></param>
        /// <param name="predictions"></param>
        /// <returns></returns>
        public static int CalculateFalseNegative( double[] truth, double[] predictions )
        {
            return truth.Select(( t, i ) => (t == 1.0 && predictions[i] == 0.0) ? 1 : 0).Sum();
        }

        /// <summary>
        /// Calculates mean angle between weight vectors or corresponding constraints 
        /// in the synthesized and actual models with use of Gurobi
        /// </summary>
        /// <param name="synthesizedConstraints"></param>
        /// <param name="benchmark"></param>
        /// <returns></returns>
        public static double CalculateMeanAngle( List<Constraint> synthesizedConstraints,
                                    Benchmark benchmark )
        {
            var benchmarkCombinations = benchmark.Constraints;
            var synthCombinations = benchmark.GetConstraintsForCalculations(synthesizedConstraints);

            // Assume benchmark constraints indexes are bound to rows
            var matrix = new double[benchmarkCombinations.Length, synthCombinations.Length];


            for( var i = 0; i < matrix.GetLength(0); i++ )
            {
                for( var j = 0; j < matrix.GetLength(1); j++ )
                {
                    matrix[i, j] = AngleBetweenVectors(benchmarkCombinations[i],
                        synthCombinations[j]);
                }
            }

            // Assignment problem
            var env = new GRBEnv("angle.log");
            var model = new GRBModel(env)
            {
                ModelSense = GRB.MINIMIZE,
                ModelName = "Mean Angle Assignment Problem"
            };

            // Binary variable for each matrix cell
            var grbVars = new GRBVar[matrix.GetLength(0), matrix.GetLength(1)];

            for( var i = 0; i < matrix.GetLength(0); i++ )
            {
                for( var j = 0; j < matrix.GetLength(1); j++ )
                {
                    // min 0, max 1, objective 0 (minimizing)
                    grbVars[i, j] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, $"b{i}.{j}");
                }
            }

            var contraintCounter = 0;
            var objectiveExpr = new GRBLinExpr();

            // First add contraints for each row
            for( var i = 0; i < matrix.GetLength(0); i++ )
            {
                var rowExpr = new GRBLinExpr();
                for( var j = 0; j < matrix.GetLength(1); j++ )
                {
                    rowExpr.AddTerm(1.0, grbVars[i, j]);
                    // Creating objective equation ONLY ONCE !
                    objectiveExpr.AddTerm(matrix[i, j], grbVars[i, j]);
                }
                model.AddConstr(rowExpr, GRB.GREATER_EQUAL, 1, $"c{contraintCounter++}");
            }

            // Transpose matrix so we can perform the same operation
            matrix = matrix.Transpose();

            // Add contstaints for originally columns
            for( var i = 0; i < matrix.GetLength(0); i++ )
            {
                var rowExpr = new GRBLinExpr();
                for( var j = 0; j < matrix.GetLength(1); j++ )
                {
                    // Inverted indexes i and j
                    rowExpr.AddTerm(1.0, grbVars[j, i]);
                }
                model.AddConstr(rowExpr, GRB.GREATER_EQUAL, 1, $"c{contraintCounter++}");
            }

            model.SetObjective(objectiveExpr);

            model.Optimize();

            // Back to orignal shape
            matrix = matrix.Transpose();

            // Gets indicators for each bianry variable for solution
            var binarySolutions = new double[matrix.GetLength(0), matrix.GetLength(1)];

            for( var i = 0; i < matrix.GetLength(0); i++ )
            {
                for( var j = 0; j < matrix.GetLength(1); j++ )
                {
                    binarySolutions[i, j] = grbVars[i, j].X;
                }
            }

            // Multiplies binary indicators with coeficients
            var sum = Matrix.ElementwiseMultiply(matrix, binarySolutions).Sum();
            var path = Path.Combine(GlobalVariables.ProjectPath, "angle_out.lp");

            model.Write(path);

            model.Dispose();
            env.Dispose();

            // Return mean angle
            return sum / (matrix.GetLength(0) > matrix.GetLength(1)
                ? matrix.GetLength(0)
                : matrix.GetLength(1));
        }

        /// <summary>
        /// Calculates angle between two vectors
        /// </summary>
        /// <param name="first">First vector</param>
        /// <param name="second">second vector</param>
        /// <returns></returns>
        public static double AngleBetweenVectors( double[] first, double[] second )
        {
            if( first.Length != second.Length )
                throw new InvalidEnumArgumentException("Vecotrs dimension not equal");

            var acc = first.Select(( t, i ) => t * second[i]).Sum();
            var denom = Math.Sqrt(first.Sum(x => x * x))
                    * Math.Sqrt(second.Sum(x => x * x));
            return Math.Acos(Math.Abs(acc) / denom);
        }
    }
}