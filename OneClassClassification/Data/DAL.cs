using System;

namespace OneClassClassification.Data
{
    internal class DAL
    {
        private static readonly Lazy<DAL> Lazy = new Lazy<DAL>(() => new DAL());

        public double[][] TrainingData { get; set; }
        public double[][] TrainingFeasibleExamples { get; set; }
        public double[][] TrainingInfeasibleExamples { get; set; }

        public double[][] TestData { get; set; }

        private DAL() { }

        public static DAL Instance => Lazy.Value;
    }
}
