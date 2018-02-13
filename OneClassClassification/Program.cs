using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using OneClassClassification.Components;
using OneClassClassification.Data;
using OneClassClassification.Utils;

namespace OneClassClassification
{
    internal class Program
    {
        private static int Main( string[] args )
        {
            var customCulture = (System.Globalization.CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            Thread.CurrentThread.CurrentCulture = customCulture;

            if ( args.Length != 9 )
            {
                DisplayUsage();
                Console.ReadKey();
                return -1;
            }
            try
            {
                PrepareParameters(args);
                PrintParameters(args);

                if (DatabaseUtils.CheckIfExists(args))
                {
                    Console.WriteLine("Experiment already exists in database!");
                    return 0;
                }

                var sw = new Stopwatch();
                sw.Start();

                Console.WriteLine("----- Generating data ------");
                var dataGenerator = new DataGenerator();
                dataGenerator.GenerateTrainingData();
                dataGenerator.GenerateTestData();

                Console.WriteLine("----- Classification data ------");

                var dataClassificator = new C45BinaryClassificator(dataGenerator.TrainingData.ToArray());
                dataClassificator.Learn();

                Console.WriteLine("----- Creating model ------");

                var modelCreator = new ModelCreator(dataClassificator.OutputPath);
                modelCreator.Create();

                sw.Stop();

                Console.WriteLine("----- Calculating statistics ------");
                
                // Calculating jaccard index for training data
                var statistics = new StatisticsCalculator();
                statistics.CalculateStatistics(dataClassificator.DecisionTree,
                    modelCreator.UniqueConstraints,
                    dataGenerator);

                // Saving data to database
                DatabaseUtils.SaveToDatabase(dataClassificator, modelCreator,
                    statistics,
                    dataGenerator,
                    sw.ElapsedMilliseconds);
            }
            catch ( ArgumentException ex )
            {
                Console.WriteLine(ex.Message);
                DatabaseUtils.SaveErrorToDatabase(ex.Message);
                return -1;
            }
            catch ( FormatException )
            {
                const string message = "Error parsing input parameters";
                DatabaseUtils.SaveErrorToDatabase(message);
                Console.WriteLine(message);
                return -1;
            }

            return 0;
        }

        private static void PrintParameters( IEnumerable<string> args )
        {
            var sb = new StringBuilder();
            sb.Append("Experiment args:\n");
            foreach ( var s in args )
            {
                sb.Append($"{s} ");
            }
            Console.WriteLine(sb.ToString());
        }

        private static void DisplayUsage()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Wrong parameters");
            sb.Append("Program pattern: OneClassClassification.exe [number of positive examples] ");
            sb.Append("[dimensions] [boundry k parameter] [Tree join parameter] ");
            sb.Append("[Tree max height parameter] [Random number generator seed] ");
            sb.Append("[BenchmarkName ={ cricle,cube}] [Components] [Experiment name]\n");
            sb.AppendLine("example: OneClassClassification.exe 100 5 0.5 100 100 25 circle 20 \"Simple experiment\"");
            Console.Write(sb.ToString());
            Console.WriteLine("Press any key ... ");
        }

        /// <summary>
        /// Assign command line parameters to <see cref="GlobalVariables"/> class
        /// </summary>
        /// <param name="args">Arguments passed from command line</param>
        private static void PrepareParameters( IReadOnlyList<string> args )
        {
            GlobalVariables.FeasibleExamplesCount = int.Parse(args[0]);
            GlobalVariables.Dimensions = int.Parse(args[1]);
            GlobalVariables.K = double.Parse(args[2]);
            GlobalVariables.Join = int.Parse(args[3]);
            GlobalVariables.MaxHeight = int.Parse(args[4]);
            GlobalVariables.Seed = int.Parse(args[5]);
            GlobalVariables.BenchmarkName = args[6].ToLower();
            GlobalVariables.Components = int.Parse(args[7]);
            GlobalVariables.ExperimentName = args[8];
        }
    }
}
