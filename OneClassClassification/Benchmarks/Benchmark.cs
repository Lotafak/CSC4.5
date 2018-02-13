using System.Collections.Generic;
using System.Linq;
using OneClassClassification.Models;

namespace OneClassClassification.Benchmarks
{
    /// <summary>
    /// Base class for benchmark desciption
    /// </summary>
    public abstract class Benchmark
    {
        /// <summary>
        /// List of constraints in form of matrix. Row is representing one contsraint
        /// </summary>
        public double[][] Constraints { get; set; }

        /// <summary>
        /// Determine class of given point
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public abstract bool AssignExampleBool(double[] values);

        /// <summary>
        /// Generates vector with length of <see cref="Dimensions"/> from given boundaries
        /// </summary>
        /// <param name="rng"></param>
        /// <returns></returns>
        public abstract double[] GenerateExample(MersenneTwister rng);

        /// <summary>
        /// Simplified version of contraint for mean angle measure
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public abstract double[] GetConstraintForCalculations( Constraint constraint );

        protected int Dimensions;

        protected Benchmark(int dimensions)
        {
            Dimensions = dimensions;
        }

        /// <summary>
        /// Return the class for single example
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public double AssignExample( double[] values )
        {
            return AssignExampleBool(values) ? 1.0 : 0.0;
        }

        /// <summary>
        /// Return the class for list of examples
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public double[] AssignExamples(double[][] values )
        {
            return values.Select(AssignExample).ToArray();
        }

        /// <summary>
        /// Wrapper around <see cref="GetConstraintForCalculations(Constraint)"/> for list of <see cref="Constraint"/>
        /// </summary>
        /// <param name="constraints"></param>
        /// <returns></returns>
        public double[][] GetConstraintsForCalculations(List<Constraint> constraints)
        {
            var array = new double[constraints.Count][];

            for ( var i = 0; i < constraints.Count; i++ )
            {
                array[i] = GetConstraintForCalculations(constraints[i]);
            }

            return array;
        }
    }
}