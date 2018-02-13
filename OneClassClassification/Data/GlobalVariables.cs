using System;
using System.IO;
using System.Text;

namespace OneClassClassification.Data
{
    public static class GlobalVariables
    {
        /// <summary>
        /// Number of positive examples to be generated
        /// </summary>
        public static int FeasibleExamplesCount { get; set; }

        /// <summary>
        /// Number of infeasible examples to be generated.
        /// Set to <see cref="FeasibleExamplesCount"/> * <see cref="Dimensions"/> ^ 2
        /// </summary>
        public static int InfeasibleExamplesCount => (int)( FeasibleExamplesCount * Math.Pow(Dimensions, 2.0) );

        /// <summary>
        /// Number of problem's variables
        /// </summary>
        public static int Dimensions { get; set; }
        
        public static double K { get; set; }

        /// <summary>
        /// Used to set <see cref="Accord.MachineLearning.DecisionTrees.Learning.C45Learning.Join"/>
        /// </summary>
        public static int Join { get; set; }

        /// <summary>
        /// Used to set <see cref="Accord.MachineLearning.DecisionTrees.Learning.C45Learning.MaxHeight"/>
        /// </summary>
        public static int MaxHeight { get; set; }

        /// <summary>
        /// Seeding value for RNGs
        /// </summary>
        public static int Seed { get; set; }

        /// <summary>
        /// Name of the benchmark to use.
        /// Allowed values: "circle", "cube" and "simplex"
        /// </summary>
        public static string BenchmarkName { get; set; }
        
        public static StringBuilder ErrorLog { get; set; } = new StringBuilder();

        /// <summary>
        /// Used to set <see cref="Accord.MachineLearning.GaussianMixtureModel"/> components
        /// </summary>
        public static int Components { get; set; }

        /// <summary>
        /// Experiments name
        /// </summary>
        public static string ExperimentName { get; set; }
        
        /// <summary>
        /// Name of the dataset - case study
        /// </summary>
        public static string DatasetName { get; set; }

        /// <summary>
        /// Folder for output files
        /// </summary>
        public static readonly string ProjectOutputPath =
            Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output"));

        /// <summary>
        /// Path to the dataset'ss folder
        /// </summary>
        public static readonly string DatasetPath = Path.GetFullPath(Path.Combine(ProjectOutputPath, @"../input"));

        public static readonly string GurobiModelPath = Path.Combine(ProjectOutputPath, "Gurobi_out.lp");

        public static string Dbpath = Path.GetFullPath(Path.Combine(ProjectOutputPath, @"../testDatabase.sqlite"));

        public static string TestDatasetPath => Path.Combine(ProjectOutputPath, $"{DatasetName}_testDataset.csv");

        /// <summary>
        /// Variable used in MILP model as M - big constant
        /// </summary>
        public static readonly int M = (int)Math.Pow(10, 6);

        // Benchmark settings
        public static readonly double R = 5;
        public static readonly double Offset = 10;
        public static readonly double Range = 3 * R;

        /// <summary>
        /// Feasible and infeasible amount separately. Total is 2 * TestExamples
        /// </summary>
        public static readonly int TestExamples = 50000;
    }
}
