using System.Collections.Generic;
using System.IO;
using Gurobi;
using OneClassClassification.Data;
using OneClassClassification.Models;
using OneClassClassification.Utils;

namespace OneClassClassification.Components
{
    /// <summary>
    /// Creating MILP model using Gurobi
    /// </summary>
    public class ModelCreator
    {
        private readonly string _path;

        /// <summary>
        /// 
        /// </summary>
        public List<Rule> Rules { get; set; } = new List<Rule>();

        /// <summary>
        /// List of unique constraints possessed from decision tree model
        /// </summary>
        public List<Constraint> UniqueConstraints { get; set; }

        /// <summary>
        /// Output Gurobi model
        /// </summary>
        public GRBModel Model { get; set; }

        /// <summary>
        /// Number of constraints in output model
        /// </summary>
        public int Constraints { get; set; }

        /// <summary>
        /// Number of terms in output model
        /// </summary>
        public int Terms { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Path to a file consisting decision tree output rules</param>
        public ModelCreator( string path )
        {
            _path = path;
        }

        /// <summary>
        /// Creates and saves model to a path set in <see cref="GlobalVariables.GurobiModelPath"/>
        /// </summary>
        public void Create()
        {
            using ( var sr = new StreamReader(_path) )
            {
                while ( !sr.EndOfStream )
                    Rules.Add(new Rule(sr.ReadLine()));
            }

            // Item1 contains a list of unique Constraints and Item2 is a number of bool variables
            var tuple = Rules.ReturnUniqueSplitsWithBooleans();

            UniqueConstraints = tuple.Item1;

            var inputs = new string[GlobalVariables.Dimensions];

            for ( var i = 0; i < GlobalVariables.Dimensions; i++ )
            {
                inputs[i] = "x" + i;
            }

            // Create Gurobi Environment and model
            var env = new GRBEnv();
            Model = new GRBModel(env) { ModelName = "OneClassClassifier" };

            // Add continous variables
            var continousVariables = new GRBVar[inputs.Length];
            for ( var i = 0; i < continousVariables.Length; i++ )
            {
                continousVariables[i] = Model.AddVar(-GRB.INFINITY, GRB.INFINITY, 0, GRB.CONTINUOUS,
                    inputs[i]);
            }

            // Add binary variables
            var binaryVariables = new GRBVar[tuple.Item2];
            for ( var i = 0; i < binaryVariables.Length; i++ )
            {
                binaryVariables[i] = Model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, $"b{i}");
            }

            var constraintCounter = 0;
            var termsCounter = 0; // Because there is always 1 dimension variable

            // Add constraints
            foreach ( var constraint in UniqueConstraints )
            {
                termsCounter += 1;

                var valueEquals = constraint.Value;

                var expr = new GRBLinExpr();
                expr.AddTerm(1.0, continousVariables[constraint.Axis]);

                foreach ( var split in constraint.Splits )
                {
                    double m;
                    if ( constraint.Sign == split.OriginalSign )
                        m = split.OriginalSign == GRB.LESS_EQUAL ? GlobalVariables.M :
                            -GlobalVariables.M;
                    else
                        m = split.OriginalSign == GRB.LESS_EQUAL ? -GlobalVariables.M :
                            GlobalVariables.M;

                    if ( split.OneMinus )
                    {
                        valueEquals += m;
                        termsCounter += 2;
                    }
                    else
                    {
                        m = -m;
                        termsCounter++;
                    }

                    expr.AddTerm(m, binaryVariables[split.SplitNumber]);
                }

                Model.AddConstr(expr, constraint.Sign, valueEquals, $"c{constraintCounter++}");
            }

            Constraints = constraintCounter;
            Terms = termsCounter;

            Model.Write(GlobalVariables.GurobiModelPath);
        }
    }
}
