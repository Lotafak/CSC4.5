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

            if ( args.Length != 2 )
            {
                DisplayUsage();
                Console.ReadKey();
                return -1;
            }
            try
            {
                PrepareParameters(args);
                PrintParameters(args);

                var sw = new Stopwatch();
                sw.Start();

                Console.WriteLine("----- Generating data ------");
                var dataReader = new DatasetReader();
                dataReader.PrepareDatasets();
                dataReader.DumpToDisk();

                Console.WriteLine("----- Classification data ------");

                var dataClassificator = new C45BinaryClassificator(dataReader.TrainingData);
                dataClassificator.Learn();

                Console.WriteLine("----- Creating model ------");

                var modelCreator = new ModelCreator(dataClassificator.OutputPath);
                modelCreator.Create();

                sw.Stop();

                var sc = new StatisticsCalculator();
                DatabaseUtils.SaveCaseStudy(sc.EvaluateJaccardIndex(dataReader, dataClassificator),
                    modelCreator, sw.ElapsedMilliseconds);
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
            sb.Append("Program pattern: OneClassClassification.exe [seed] [dataset name] [components]");
            Console.Write(sb.ToString());
            Console.WriteLine("Press any key ... ");
        }

        private static void PrepareParameters( IReadOnlyList<string> args )
        {
            GlobalVariables.Seed = int.Parse(args[0]);
            Accord.Math.Random.Generator.Seed = GlobalVariables.Seed;
            GlobalVariables.DatasetName = args[1];
            //GlobalVariables.Components = int.Parse(args[2]);
        }
    }
}
