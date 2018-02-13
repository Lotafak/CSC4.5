using System.Linq;
using OneClassClassification.Data;
using OneClassClassification.Models;

namespace OneClassClassification.Benchmarks
{
    /// <summary>
    /// Cuben implementation of benchmark
    /// </summary>  
    public class CubeBenchmark : Benchmark
    {
        public CubeBenchmark( int dimensions ) : base(dimensions)
        {
            var constraints = new double[dimensions * 2][];
            var dimCounter = 0;

            for ( var i = 0; i < constraints.Length; i = i + 2 )
            {
                constraints[i] = GetConstraintForCalculations(new Constraint { Axis = dimCounter });
                constraints[i + 1] = GetConstraintForCalculations(new Constraint { Axis = dimCounter++ });
            }

            // 2 the same contraints for each axis
            Constraints = constraints;
        }

        public override bool AssignExampleBool( double[] values )
        {
            for ( int i = 1; i < values.Length + 1; i++ )
            {
                if ( values[i - 1] < i || values[i - 1] > i + GlobalVariables.R * i )
                    return false;
            }

            return true;
        }

        public override double[] GenerateExample( MersenneTwister rng )
        {
            var x = new double[Dimensions];
            return x.Select(( v, i ) => rng.NextDouble() * ( GlobalVariables.Range * ( i + 1 ) ) + ( i + 1 ) - ( GlobalVariables.R * ( i + 1 ) ))
                            .ToArray();
        }

        public sealed override double[] GetConstraintForCalculations( Constraint constraint )
        {
            var tmp = new double[Dimensions];
            tmp[constraint.Axis] = 1.0;
            return tmp;
        }
    }
}