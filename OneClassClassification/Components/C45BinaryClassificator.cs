using System.IO;
using System.Threading.Tasks;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math;
using OneClassClassification.Data;

namespace OneClassClassification.Components
{
    /// <summary>
    /// <see cref="Accord.MachineLearning.DecisionTrees.Learning.C45Learning"/> wrapper for
    /// for two class classification
    /// </summary>
    public class C45BinaryClassificator
    {
        /// <summary>
        /// Input data values
        /// </summary>
        public double[][] Inputs { get; set; }

        /// <summary>
        /// Input data classes
        /// </summary>
        public int[] Outputs { get; set; }

        /// <summary>
        /// Used to set <see cref="Accord.MachineLearning.DecisionTrees.Learning.C45Learning.Join"/>
        /// </summary>
        public int Join { get; set; }

        /// <summary>
        /// Used to set <see cref="Accord.MachineLearning.DecisionTrees.Learning.C45Learning.MaxHeight"/>
        /// </summary>
        public int MaxHeight { get; set; }

        public DecisionTree DecisionTree { get; set; }
        public string OutputRules { get; set; }

        /// <summary>
        /// Decision Tree rules output file
        /// </summary>
        public string OutputPath { get; set; } = $"{GlobalVariables.ProjectPath}/outputRules.txt";

        /// <summary>
        /// Create instance of C45BinaryClassificator for 2 class classification.
        /// The class is using <see cref="C45Learning"/> algorithm to teach <see cref="DecisionTree"/>
        /// </summary>
        /// <param name="data">Matrix with last column of output classes.</param>
        public C45BinaryClassificator( double[][] data )
        {
            Inputs = data
                .GetColumns(Vector.Range(0, GlobalVariables.Dimensions))
                .To<double[][]>();

            Outputs = data
                .GetColumn(GlobalVariables.Dimensions)
                .To<int[]>();

            Join = GlobalVariables.Join;
            MaxHeight = GlobalVariables.MaxHeight;
        }

        /// <summary>
        /// Learns a model, wrapper for
        /// <see cref="Accord.MachineLearning.DecisionTrees.Learning.C45Learning.Learn(double[][], int[], double[])"/>
        /// </summary>
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

            c45.Learn(Inputs, Outputs);

            // Getting rules from tree and saving them to file
            using ( var sw = new StreamWriter(OutputPath) )
            {
                OutputRules = DecisionTree.ToRules().ToString().Replace(",", ".");
                sw.Write(OutputRules);
            }
        }
    }
}
