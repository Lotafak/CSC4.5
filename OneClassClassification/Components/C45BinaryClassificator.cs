using System.IO;
using System.Threading.Tasks;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math;
using OneClassClassification.Data;
using System.Collections.Generic;
using System;
using Accord.MachineLearning.DecisionTrees.Pruning;

namespace OneClassClassification.Components
{
    public class C45BinaryClassificator
    {
        public double[][] TrainingInput { get; set; }
        public double[][] PruningInput { get; set; }
        public int[] TrainingOutput { get; set; }
        public int[] PruningOutput { get; set; }
        public int Join { get; set; }
        public int MaxHeight { get; set; }
        public DecisionTree DecisionTree { get; set; }
        public string OutputRules { get; set; }
        public string OutputPath { get; set; } = $"{GlobalVariables.ProjectPath}/outputRules.txt";

        /// <summary>
        /// Create instance of C45BinaryClassificator for 2 class classification.
        /// The class is using <see cref="C45Learning"/> algorithm to teach <see cref="DecisionTree"/>
        /// </summary>
        /// <param name="data">Matrix with last column of output classes.</param>
        public C45BinaryClassificator( double[][] data )
        {
            int idx;
            var rnd = new MersenneTwister(GlobalVariables.Seed);
            double[][] pruningExamples = new double[Convert.ToInt32(0.1 * data.Length)][]; 
            for( int i = 0; i < pruningExamples.Length; i++ )
            {
                idx = rnd.Next(data.Length);
                pruningExamples[i] = data[idx];
                data = data.RemoveAt(idx);
            }
                
            TrainingInput = data.GetColumns(Vector.Range(0, GlobalVariables.Dimensions))
                .To<double[][]>();
            TrainingOutput = data.GetColumn(GlobalVariables.Dimensions).To<int[]>();

            PruningInput = pruningExamples.GetColumns(Vector.Range(0, GlobalVariables.Dimensions))
                .To<double[][]>();
            PruningOutput = pruningExamples.GetColumn(GlobalVariables.Dimensions).To<int[]>();

            Join = GlobalVariables.Join;
            MaxHeight = GlobalVariables.MaxHeight;
        }

        public void Learn()
        {
            var features = new DecisionVariable[GlobalVariables.Dimensions];

            // Adding DecisionVariables (features)
            for ( var i = 0; i < GlobalVariables.Dimensions; i++ )
            {
                features[i] = new DecisionVariable($"x{i}", DecisionVariableKind.Continuous);
            }

            // Create 2 class tree object 
            DecisionTree = new DecisionTree(features, 2);

            var c45 = new C45Learning(DecisionTree)
            {
                Join = Join,
                MaxHeight = MaxHeight,
                ParallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 1 }  // Only 1 thread will be used by the learning algorithm
            };

            c45.Learn(TrainingInput, TrainingOutput);

            ErrorBasedPruning prune = new ErrorBasedPruning(DecisionTree, PruningInput, PruningOutput);

            double lastError;
            var error = Double.PositiveInfinity;
            var counter = 0;

            do
            {
                lastError = error;
                error = prune.Run();
                if(error == lastError) counter++;
            } while( error <= lastError && counter < 10);

            // Getting rules from tree and saving them to file
            using ( var sw = new StreamWriter(OutputPath) )
            {
                OutputRules = DecisionTree.ToRules().ToString().Replace(",", ".");
                sw.Write(OutputRules);
            }
        }
    }
}
