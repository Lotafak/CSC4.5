using System;
using System.IO;
using System.Text;
using ExperimentDatabase;
using Microsoft.Data.Sqlite;
using OneClassClassification.Components;
using OneClassClassification.Data;

namespace OneClassClassification.Utils
{
    /// <summary>
    /// Class containing database operations
    /// </summary>
    public static class DatabaseUtils
    {
        /// <summary>
        /// Saves experiments data to database. Data is gathered from different program's classes given as parameters
        /// </summary>
        /// <param name="classificator">Instance of <see cref="C45BinaryClassificator"/>class 
        /// (Used only for rules saving which is commented)</param>
        /// <param name="model"></param>
        /// <param name="statistics"></param>
        /// <param name="data"></param>
        /// <param name="executionTime"></param>
        public static void SaveToDatabase( C45BinaryClassificator classificator, ModelCreator model,
                                        StatisticsCalculator statistics, DataGenerator data,
                                        long executionTime )
        {
            Console.WriteLine(GlobalVariables.Dbpath);
            var db = new Database(GlobalVariables.Dbpath);
            var experiment = db.NewExperiment();

            // Input variables
            experiment["Benchmark"] = GlobalVariables.BenchmarkName;
            experiment["FeasibleExamples"] = GlobalVariables.FeasibleExamplesCount;
            experiment["InfeasibleExamples"] = GlobalVariables.InfeasibleExamplesCount;
            experiment["Dimensions"] = GlobalVariables.Dimensions;
            experiment["K"] = GlobalVariables.K;
            experiment["Join"] = GlobalVariables.Join;
            experiment["MaxHeight"] = GlobalVariables.MaxHeight;
            experiment["Seed"] = GlobalVariables.Seed;
            experiment["Components"] = GlobalVariables.Components;
            experiment["ExperimentName"] = GlobalVariables.ExperimentName;

            // Sometimes is extending SQlite cell length limits which leads to damaging database
            //experiment["Rules"] = classificator.DecisionTree.ToRules().ToString();

            // data
            experiment["Eps"] = data.BoundryRescaler.Eps;
            var sb = new StringBuilder();
            for ( var i = 0; i < data.NewBoundries.GetLength(0); i++ )
            {
                sb.AppendLine($"{data.NewBoundries[i, 0]},{data.NewBoundries[i, 1]}");
            }
            experiment["NewBoundries"] = sb.ToString();

            // Gurobi model data
            experiment["Constraints"] = model.Constraints;
            experiment["Terms"] = model.Terms;

            // Same situaction as with "Rules" cell above
            //using ( var sr = new StreamReader(GlobalVariables.GurobiModelPath) )
            //{
            //    experiment["GurobiModel"] = sr.ReadToEnd();
            //}

            // Statistics
            experiment["FJaccard"] = statistics.FixedJaccardIndex;
            experiment["FAccuracy"] = statistics.FixedAccuracy;

            experiment["FTP"] = statistics.FixedTruePositive;
            experiment["FFP"] = statistics.FixedFalsePositive;
            experiment["FTN"] = statistics.FixedTrueNegative;
            experiment["FFN"] = statistics.FixedFalseNegative;

            experiment["HJaccard"] = statistics.HomogenousJaccardIndex;
            experiment["HAccuracy"] = statistics.HomogenousAccuracy;

            experiment["HTP"] = statistics.HomogenousTruePositive;
            experiment["HFP"] = statistics.HomogenousFalsePositive;
            experiment["HTN"] = statistics.HomogenousTrueNegative;
            experiment["HFN"] = statistics.HomogenousFalseNegative;

            experiment["Total"] = statistics.DataLength;

            experiment["MeanAngle"] = statistics.MeanAngle;
            experiment["StatisticsCalculationTime"] = statistics.ExecutionTime;
            experiment["ProcessingTime"] = executionTime;

            // Errors
            experiment["Errors"] = GlobalVariables.ErrorLog.ToString();

            experiment.Save();

            experiment.Dispose();
            db.Dispose();
        }

        internal static void SaveCaseStudy( double jaccard, ModelCreator model, long executionTime )
        {
            var db = new Database(GlobalVariables.Dbpath);
            var experiment = db.NewExperiment();

            experiment["Dataset"] = GlobalVariables.DatasetName;
            experiment["Seed"] = GlobalVariables.Seed;
            experiment["Jaccard"] = jaccard;
            experiment["Components"] = GlobalVariables.Components;

            experiment["Constraints"] = model.Constraints;
            experiment["Terms"] = model.Terms;

            experiment["ExecutionTime"] = executionTime;

            experiment.Save();
            experiment.Dispose();
            db.Dispose();
        }

        // In case of an error save run parameters and errors into database
        public static void SaveErrorToDatabase( string error )
        {
            var db = new Database(GlobalVariables.Dbpath);
            var experiment = db.NewExperiment();

            // Input variables
            experiment["Benchmark"] = GlobalVariables.BenchmarkName;
            experiment["FeasibleExamples"] = GlobalVariables.FeasibleExamplesCount;
            experiment["Dimensions"] = GlobalVariables.Dimensions;
            experiment["K"] = GlobalVariables.K;
            experiment["Join"] = GlobalVariables.Join;
            experiment["MaxHeight"] = GlobalVariables.MaxHeight;
            experiment["Seed"] = GlobalVariables.Seed;
            experiment["Components"] = GlobalVariables.Components;
            experiment["ExperimentName"] = GlobalVariables.ExperimentName;

            experiment["Errors"] = GlobalVariables.ErrorLog.AppendLine(error).ToString();

            experiment.Save();

            experiment.Dispose();
            db.Dispose();
        }

        /// <summary>
        /// Check whether experiment already exists in database
        /// </summary>
        /// <param name="args">Parameters passed to command line</param>
        /// <returns></returns>
        public static bool CheckIfExists( string[] args )
        {
            if ( !File.Exists(GlobalVariables.Dbpath) )
                return false;

            var filter = new StringBuilder();

            filter.Append($"FeasibleExamples = {args[0]} AND Dimensions = {args[1]} AND ");
            filter.Append($"K = {args[2]} AND [Join] = {args[3]} AND MaxHeight = {args[4]} AND Seed = {args[5]} AND ");
            filter.Append($"Benchmark = '{args[6]}' AND Components = {args[7]} AND Errors = ''");

            var query = $"SELECT * FROM experiments WHERE {filter}";

            using (var conn = new SqliteConnection($"Data Source={GlobalVariables.Dbpath}"))
            {
                conn.Open();

                using (var command = new SqliteCommand(query, conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var res = reader.HasRows;

                        if (!res) return false;

                        reader.Read();

                        if (reader["ExperimentName"].ToString().Contains(args[8])) return true;
                    }
                }
            }
            
            // In case experiment with exact same parameters exists, but was executed under different name,
            // update row with current experiments name
            UpdateExperimentNameColumn(args[8], filter.ToString());

            return true;
        }

        /// <summary>
        /// Update "ExperimentName" column with <paramref name="experimentName"/> value for row selected by 
        /// <paramref name="filter"/>
        /// </summary>
        /// <param name="experimentName"></param>
        /// <param name="filter"></param>
        public static void UpdateExperimentNameColumn( string experimentName, string filter )
        {
            var query = $"UPDATE experiments SET ExperimentName = ExperimentName || ';{experimentName}' WHERE {filter}";

            using (var conn = new SqliteConnection($"Data Source={GlobalVariables.Dbpath}"))
            {
                conn.Open();
                using (var command = new SqliteCommand(query, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
