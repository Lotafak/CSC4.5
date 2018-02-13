using System;
using System.Linq;
using Accord.Math;
using OneClassClassification.Data;
using OneClassClassification.Models;
using OneClassClassification.Utils;

namespace OneClassClassification.Benchmarks
{
    public class SimplexBenchmark : Benchmark
    {
        private readonly double _simplexCot = MathHelper.Cot(Math.PI / 12);
        private readonly double _simplexTan = Math.Tan(Math.PI / 12);

        public SimplexBenchmark(int dimensions) : base(dimensions)
        {
            // for 3 dimensions pairs (1,2), (1,3), (2,3) - so it's the count of elements of upper triangle matrix
            var length = Matrix.Create(dimensions, dimensions, 1).GetUpperTriangle().Sum();
            // 2 times iteration + one for constant term
            var constraints = new double[2 * length + 1][];

            var counter = 0;
            for (var i = 0; i < dimensions; i++)
            {
                for (var j = i + 1; j < dimensions; j++)
                {
                    var tmp = new double[dimensions];
                    tmp[i] = _simplexCot;
                    tmp[j] = -_simplexTan;
                    constraints[counter++] = tmp;

                    // Opposed constraints
                    tmp = new double[dimensions];
                    tmp[i] = -_simplexTan;
                    tmp[j] = _simplexCot;
                    constraints[counter++] = tmp;
                }
            }

            var last = new double[dimensions];
            last = last.Select(x => 1.0).ToArray();
            constraints[counter] = last;

            Constraints = constraints;
        }

        public override bool AssignExampleBool(double[] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                for (var j = i + 1; j < values.Length; j++)
                {
                    if (values[i] * _simplexCot - values[j] * _simplexTan < 0 ||
                        values[j] * _simplexCot - values[i] * _simplexTan < 0)
                        return false;
                }
            }
            return (values.Sum(x => x) < GlobalVariables.R);
        }

        public override double[] GenerateExample(MersenneTwister rng)
        {
            var x = new double[Dimensions];
            return x.Select(
                v =>
                    rng.NextDouble() * (3 + GlobalVariables.R) - 1).ToArray();
        }

        public override double[] GetConstraintForCalculations(Constraint constraint)
        {
            var tmp = new double[Dimensions];
            tmp[constraint.Axis] = 1.0;
            return tmp;
        }
    }
}
