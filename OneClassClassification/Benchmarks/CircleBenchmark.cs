using System;
using System.Linq;
using OneClassClassification.Data;
using OneClassClassification.Models;

namespace OneClassClassification.Benchmarks
{
    /// <summary>
    /// Balln implementation of benchmark
    /// </summary>
    public class CircleBenchmark : Benchmark
    {
        public CircleBenchmark( int dimensions ) : base(dimensions)
        {
            var counter = 1;
            var constraints = new double[1][];
            constraints[0] = new double[Dimensions * 2];
            for ( var i = 0; i < Dimensions * 2; i = i + 2 )
            {
                constraints[0][i] = 1;
                constraints[0][i + 1] = -2 * counter++;
            }

            // One quadratic contraint in form of [x^2, -2x, y^2, -4x, ..., z^2, -2*dimz]
            Constraints = constraints;
        }

        public override bool AssignExampleBool( double[] values )
        {
            var sum = 0.0;

            for ( var i = 1; i < values.Length + 1; i++ )
            {
                sum += Math.Pow(values[i-1] - i, 2);
            }
            return Math.Pow(GlobalVariables.R, 2) > sum;;
        }

        /// <summary>
        /// Generate example from within [i - 2r, i + 2r]
        /// </summary>
        /// <param name="rng"></param>
        /// <returns></returns>
        public override double[] GenerateExample( MersenneTwister rng )
        {
            var x = new double[Dimensions];
            return x.Select(
                    (v, i) =>
                        rng.NextDouble() * GlobalVariables.R * 4 + (i+1) - 2 * GlobalVariables.R )
                    .ToArray();
        }

        /// <summary>
        /// Generates the vector in form of [0, 1.0, 0, 1.0, ..., 1.0]
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public override double[] GetConstraintForCalculations( Constraint constraint )
        {
            var tmp = new double[2 * Dimensions];
            tmp[2 * constraint.Axis + 1] = 1.0;
            return tmp;
        }
    }
}