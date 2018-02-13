using Accord.MachineLearning;
using Accord.Math;
using OneClassClassification.Data;
using OneClassClassification.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OneClassClassification.Components
{
    public class DatasetReader
    {
        public double[][] Domain { get; set; }
        public double[][] FeasibleTrainingData { get; set; }
        public double[][] FeasibleTestData { get; set; }
        public double[][] InfeasibleTrainingData { get; private set; }
        public double[][] TrainingData { get; set; }
        public double[][] InfeasibleTestData { get; set; }
        public double[][] TestData { get; private set; }

        public double[][] ReadFeasibleDataset()
        {
            using( var sr = new StreamReader(Path.Combine(GlobalVariables.DatasetPath, $"{GlobalVariables.DatasetName}.csv")) )
            {
                var list = sr.ReadLine().Split(';').Skip(1).ToArray();
                var trainingExamples = new List<double[]>();
                var counter = 0;
                Domain = new double[list.Length][];

                //Fill parameters 
                GlobalVariables.Join = 2 * list.Length;
                GlobalVariables.MaxHeight = 2 * GlobalVariables.Join;
                GlobalVariables.Dimensions = list.Length;
                GlobalVariables.Components = list.Length;

                var rgx = new Regex(@"\|(.+)]");
                var values = "";

                foreach( var item in list )
                {
                    values = rgx.Match(item).Groups[1].Value;
                    Domain[counter++] = Array.ConvertAll(values.Split('|'), i => double.Parse(i));
                }

                var line = sr.ReadLine();
                while( line != null )
                {
                    var l = Array.ConvertAll(line.Split(';').Skip(1).ToArray(), i => Double.Parse(i));
                    trainingExamples.Add(l);
                    line = sr.ReadLine();
                }

                return trainingExamples.ToArray();
            }
        }

        public void PrepareDatasets()
        {
            FeasibleTrainingData = ReadFeasibleDataset();
            SplitFesibleData();

            // Training Data
            InfeasibleTrainingData = PrepareInfeasibleSet(FeasibleTrainingData);
            TrainingData = FeasibleTrainingData.Concat(InfeasibleTrainingData).ToArray();

            // Test Data
            InfeasibleTestData = PrepareInfeasibleSet(FeasibleTestData);
            TestData = FeasibleTestData.Concat(InfeasibleTestData).ToArray();
        }

        private double[][] PrepareInfeasibleSet( double[][] feasibleData )
        {
            var success = false;
            GaussianMixtureModel gmm = new GaussianMixtureModel(GlobalVariables.Components);
            while( !success )
            {
                try
                {
                    gmm = new GaussianMixtureModel(GlobalVariables.Components)
                    {
                        Initializations = 100,
                        MaxIterations = 0,
                        //ParallelOptions = new System.Threading.Tasks.ParallelOptions() { MaxDegreeOfParallelism = 1 },
                        Tolerance = 1E-11,
                        Options = new Accord.Statistics.Distributions.Fitting.NormalOptions() { Regularization = double.Epsilon }
                    };
                    gmm.Learn(feasibleData);
                    success = true;
                }
                catch
                {
                    Console.WriteLine(GlobalVariables.Components);
                    if( GlobalVariables.Components > 1 )
                        GlobalVariables.Components--;
                }
            }

            // Estimate the Gaussian Mixture
            var distribution = gmm.ToMixtureDistribution();

            // Get 1-percentile probability of probability density function from distribution
            for( var i = 0; i < feasibleData.Length; i++ )
            {
                feasibleData[i] = feasibleData[i]
                    .ExtendArrayWithValue(distribution.ProbabilityDensityFunction(feasibleData[i]));
            }
            feasibleData = feasibleData.OrderBy(item => item[item.Length - 1]).ToArray();
            var mp = feasibleData.
                ElementAt((int)Math.Round((double)(feasibleData.Length / 100), 0, MidpointRounding.AwayFromZero))
                [feasibleData[0].Length - 1];

            // Give positive class to every example in matrix
            for( int i = 0; i < feasibleData.Length; i++ )
            {
                feasibleData[i][feasibleData[i].Length - 1] = 1;
            }

            // Generate infeasible examples
            var infeasibleArray = new List<double[]>();
            var c = 0;
            while( infeasibleArray.Count < feasibleData.Length * (GlobalVariables.Dimensions * GlobalVariables.Dimensions) )
            //while( infeasibleArray.Count < FeasibleTrainingData.Length )
            {
                var example = distribution.Generate();
                if( distribution.ProbabilityDensityFunction(example) < mp )
                {
                    infeasibleArray.Add(example.ExtendArrayWithValue(0));
                    if( c > 1000 )
                    {
                        Console.WriteLine($"{infeasibleArray.Count}");
                        c = 0;
                    }
                    c++;
                }
            }

            return infeasibleArray.ToArray();
        }

        private void SplitFesibleData()
        {
            var rng = new MersenneTwister(GlobalVariables.Seed);
            var initialSize = FeasibleTrainingData.Length;
            var tempTestSet = new List<double[]>();
            while( FeasibleTrainingData.Length > Math.Round((double)initialSize / 2, MidpointRounding.AwayFromZero) )
            {
                var index = rng.Next(FeasibleTrainingData.Length - 1);
                tempTestSet.Add(FeasibleTrainingData[index]);
                FeasibleTrainingData = FeasibleTrainingData.Where(el => el != FeasibleTrainingData[index]).ToArray();
            }
            FeasibleTestData = tempTestSet.ToArray();
        }

        public void GenerateTestDataset()
        {
            Accord.Math.Random.Generator.Seed = 0;
            PrepareDatasets();

            // Save dataset to disk
            using( var sw = new StreamWriter(GlobalVariables.TestDatasetPath) )
            {
                foreach( var ex in TrainingData )
                {
                    var line = "";
                    foreach( var dim in ex )
                    {
                        line += $"{dim};";
                    }
                    line = line.Remove(line.Length - 1);
                    sw.WriteLine(line);
                }
            }
            InfeasibleTestData = TrainingData.Where(item => item[item.Length - 1] == 0).ToArray();
            Accord.Math.Random.Generator.Seed = GlobalVariables.Seed;
        }
        internal void ReadTestDataset()
        {
            using( var sr = new StreamReader(GlobalVariables.TestDatasetPath) )
            {
                var tempTest = new List<double[]>();
                var line = sr.ReadLine();

                while( line != null )
                {
                    var row = Array.ConvertAll(line.Split(';'), item => double.Parse(item));
                    if( row[row.Length - 1] == 0 )
                        tempTest.Add(row);
                    line = sr.ReadLine();
                }
                InfeasibleTestData = tempTest.ToArray();
            }
        }

        public void DumpToDisk()
        {
            using( var sw = new StreamWriter(Path.Combine(GlobalVariables.ProjectOutputPath,
                $"TrainingDataset_{GlobalVariables.DatasetName}.csv")) )
            {
                PrintCsvArray(sw, TrainingData);
            }

            using( var sw = new StreamWriter(Path.Combine(GlobalVariables.ProjectOutputPath,
                $"TestDataset_{GlobalVariables.DatasetName}.csv")) )
            {
                PrintCsvArray(sw, TestData);
            }
        }

        private void PrintCsvArray( StreamWriter sw, double[][] array )
        {
            foreach( var ex in array )
            {
                for( int i = 0; i < ex.Length; i++ )
                {
                    if( i == ex.Length - 1 )
                        sw.WriteLine(ex[i]);
                    else
                        sw.Write($"{ex[i]};");
                }
            }
        }
    }
}