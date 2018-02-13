using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accord.MachineLearning;
using OneClassClassification.Benchmarks;
using OneClassClassification.Data;
using OneClassClassification.Rescalers;
using OneClassClassification.Utils;

namespace OneClassClassification.Components
{
    /// <summary>
    /// Responsible for data generation
    /// </summary>
    public class DataGenerator
    {
        /// <summary>
        /// List of feasible examples
        /// </summary>
        public List<double[]> Feasibles { get; set; } = new List<double[]>();

        /// <summary>
        /// List of classes
        /// </summary>
        public List<int> Output { get; set; } = new List<int>();
        
        /// <summary>
        /// Complete training set
        /// </summary>
        public List<double[]> TrainingData { get; set; } = new List<double[]>();

        /// <summary>
        /// Test data generated with equal distribution between feasible and infeasible examples
        /// </summary>
        public List<double[]> FixedDistributionTestData { get; set; } = new List<double[]>();

        /// <summary>
        /// Test data generated using Homogenous Distribution of feasible and infeasible examples
        /// </summary>
        public List<double[]> HomogenousTestData { get; set; } = new List<double[]>();

        /// <summary>
        /// Used to rescale boundaries for benchmark based on which examples will be generated
        /// </summary>
        public BoundryRescaler BoundryRescaler { get; set; }

        /// <summary>
        /// New variable's boundaries
        /// </summary>
        public double[,] NewBoundries { get; set; }

        /// <summary>
        /// Benchmark used while generating data
        /// </summary>
        public Benchmark Benchmark { get; set; }

        /// <summary>
        /// Instsnce of RNG
        /// </summary>
        private readonly MersenneTwister _rand;

        /// <summary>
        /// Output path for examples
        /// </summary>
        private readonly string _projectPath;

        /// <summary>
        /// Keeps data in memory for the project
        /// </summary>
        private readonly DAL _dal = DAL.Instance;

        /// <summary>
        /// Initializes instance with settings from <see cref="GlobalVariables"/> class
        /// </summary>
        public DataGenerator()
        {
            _projectPath = GlobalVariables.ProjectPath;
            _rand = new MersenneTwister(GlobalVariables.Seed);
            BoundryRescaler = new MinMaxBoundryRescaler();
            switch (GlobalVariables.BenchmarkName)
            {
                case "circle":
                    Benchmark = new CircleBenchmark(GlobalVariables.Dimensions);
                    break;
                case "cube":
                    Benchmark = new CubeBenchmark(GlobalVariables.Dimensions);
                    break;
                case "simplex":
                    Benchmark = new SimplexBenchmark(GlobalVariables.Dimensions);
                    break;
                default:
                    throw new ArgumentException("Incorrect benchmark parameter");
            }
        }

        /// <summary>
        /// Generates both training set consisting of feasible and infeasible examples
        /// </summary>
        public void GenerateTrainingData()
        {
            Feasibles = GenerateFeasibleExamples(GlobalVariables.FeasibleExamplesCount);

            // Fill training data
            TrainingData = Feasibles.AddColumnWithValues(1.0);

            // Fill output List
            var tempArr = new double[TrainingData.Count];
            Output.AddRange(tempArr.Select(x => 1));

            _dal.TrainingFeasibleExamples = Feasibles.ToArray();

            GaussianMixtureModel gmm = new GaussianMixtureModel(GlobalVariables.Components);
            
            gmm = new GaussianMixtureModel(GlobalVariables.Components)
            {
                Initializations = 100,
                MaxIterations = 10000,
                ParallelOptions = new System.Threading.Tasks.ParallelOptions() { MaxDegreeOfParallelism = 1 },
                Tolerance = 10E-11,
                Options = new Accord.Statistics.Distributions.Fitting.NormalOptions() { Regularization = double.Epsilon }
            };

            // Estimate the Gaussian Mixture
            gmm.Learn(_dal.TrainingFeasibleExamples);
            var iterations = gmm.Iterations;
            var distribution = gmm.ToMixtureDistribution();

            // Get minimal probability of probability density function from distribution (percentile 0)
            var minimalProbability = _dal.TrainingFeasibleExamples
                .Select(item => distribution.ProbabilityDensityFunction(item))
                .Min();

            // Rescale data range for infeasible example creation
            NewBoundries = BoundryRescaler.Rescale(Feasibles);

            // Generate infeasible examples
            var infeasibles = new List<double[]>();

            while (infeasibles.Count < GlobalVariables.InfeasibleExamplesCount)
            {
                // Generate points within new boundry
                var x = GenerateLimitedInputs(GlobalVariables.Dimensions, NewBoundries);

                // Calculate probability density function value for given input
                var probability = distribution.ProbabilityDensityFunction(x);

                // Check if the value is smaller than smallest probability of all feasible examples
                if (probability > minimalProbability)
                    continue;

                infeasibles.Add(x);

                TrainingData.Add(x.ExtendArrayWithValue(0.0));

                Output.Add(0);
            }

            _dal.TrainingInfeasibleExamples = infeasibles.ToArray();
            _dal.TrainingData = TrainingData.ToArray();
        }

        /// <summary>
        /// Generates test data consisting of <see cref="GlobalVariables.TestExamples"/> * 2 examples
        /// both with fixed and homogenous distribution
        /// </summary>
        public void GenerateTestData()
        {
            FixedDistributionTestData.AddRange(GenerateFeasibleExamples(GlobalVariables.TestExamples)
                .AddColumnWithValues(1.0));
            FixedDistributionTestData.AddRange(GenerateInfeasibleExamples(GlobalVariables.TestExamples)
                .AddColumnWithValues(0.0));

            double[] vector;

            while(HomogenousTestData.Count < GlobalVariables.TestExamples * 2 )
            {
                vector = Benchmark.GenerateExample(_rand);
                HomogenousTestData.Add(vector.ExtendArrayWithValue(Benchmark.AssignExample(vector)));
            }
        }

        /// <summary>
        /// Generate inputs within specyfied range
        /// </summary>
        /// <param name="variablesCount">Number of variables</param>
        /// <param name="limitations">Minimum and maximum values for each variable</param>
        /// <returns></returns>
        public double[] GenerateLimitedInputs(int variablesCount, double[,] limitations)
        {
            var x = new double[variablesCount];

            for (var i = 0; i < variablesCount; i++)
                // (0..1) * dim range + offset (min dim value)
                x[i] = _rand.NextDouble() * (limitations[i, 1] - limitations[i, 0])
                                            + limitations[i, 0];

            return x;
        }

        /// <summary>
        /// Saves training data to file in csv format with header
        /// </summary>
        /// <param name="filename"></param>
        public void DumpToFile(string filename)
        {
            var fi = new DirectoryInfo(GlobalVariables.ProjectPath);
            if (!fi.Exists)
                fi.Create();

            using (var fileStream = new StreamWriter($"{_projectPath}/{filename}"))
            {
                for (var i = 0; i < GlobalVariables.Dimensions; i++)
                {
                    fileStream.Write($"x{i},");
                }
                fileStream.Write("y\n");

                foreach (var doubles in TrainingData)
                {
                    for (var i = 0; i < doubles.Length - 1; i++)
                    {
                        fileStream.Write($"{doubles[i]},");
                    }
                    fileStream.Write($"{doubles[doubles.Length - 1]}\n");
                }
            }
        }

        /// <summary>
        /// Generates feasible examples based on given benchmark's boundaries
        /// </summary>
        /// <param name="examplesCount"></param>
        /// <returns></returns>
        public List<double[]> GenerateFeasibleExamples(int examplesCount)
        {
            var feasibleExamples = new List<double[]>();

            // Generating positives examples based on wheather they are within the circle of R from Cosntants.
            while (feasibleExamples.Count < examplesCount)
            {
                var input = Benchmark.GenerateExample(_rand);

                if (!Benchmark.AssignExampleBool(input))
                    continue;

                feasibleExamples.Add(input);
            }

            return feasibleExamples;
        }

        /// <summary>
        /// Generate infeasible examples based on given benchmark's boundaries
        /// </summary>
        /// <param name="examplesCount"></param>
        /// <returns></returns>
        public List<double[]> GenerateInfeasibleExamples(int examplesCount)
        {
            var infeasibleExamples = new List<double[]>();

            while (infeasibleExamples.Count < examplesCount)
            {
                var input = Benchmark.GenerateExample(_rand);

                if (Benchmark.AssignExampleBool(input))
                    continue;

                infeasibleExamples.Add(input);
            }

            return infeasibleExamples;
        }
    }
}