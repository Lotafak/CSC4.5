using System;

namespace OneClassClassification.Utils
{
    public static class MathHelper
    {
        /// <summary>
        /// Return cotangent of the specified angle
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Cot(double x)
        {
            return 1/Math.Tan(x);
        }
    }
}
