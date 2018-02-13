using System.Collections.Generic;

namespace OneClassClassification.Rescalers
{
    public abstract class BoundryRescaler
    {
        public abstract double Eps { get; set; }
        public abstract double[,] Rescale(List<double[]> feasibles);
    }
}
