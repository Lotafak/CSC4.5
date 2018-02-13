using System.Collections.Generic;
using System.Linq;
using OneClassClassification.Data;

namespace OneClassClassification.Rescalers
{
    public class MinMaxBoundryRescaler : BoundryRescaler
    {
        public override double Eps { get; set; }

        public override double[,] Rescale(List<double[]> feasibles)
        {
            var newBoundries = new double[GlobalVariables.Dimensions, 2];

            // Calculate min and max value for each dimension
            for ( var i = 0; i < GlobalVariables.Dimensions; i++ )
            {
                newBoundries[i, 0] = feasibles.Min(x => x[i]);
                newBoundries[i, 1] = feasibles.Max(x => x[i]);
            }

            // Rescale newBoundries according to K input parameter
            for ( var i = 0; i < newBoundries.GetLength(0); i++ )
            {
                Eps = GlobalVariables.K * ( newBoundries[i, 1] - newBoundries[i, 0] );
                newBoundries[i, 0] = newBoundries[i, 0] - Eps;
                newBoundries[i, 1] = newBoundries[i, 1] + Eps;
            }

            return newBoundries;
        }
    }
}