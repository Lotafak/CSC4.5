using System.Collections.Generic;

namespace OneClassClassification.Models
{
    /// <summary>
    /// Represents MILP constraint
    /// </summary>
    public class Constraint
    {
        /// <summary>
        /// Equations sign. Can be '&lt;' or '&gt;'
        /// </summary>
        public char Sign { get; set; }

        /// <summary>
        /// Variable's weight
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Variable indicator
        /// </summary>
        public int Axis { get; set; }

        /// <summary>
        /// Auxiliary binary variables
        /// </summary>
        public List<Split> Splits { get; set; } = new List<Split>();

        /// <summary>
        /// By default we compare Value and Axis for equality. Different equality comparison method is
        /// implemented in <see cref="Utils.CompareWithSign"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var y = obj as Constraint;
            return ( Value == y.Value && Axis == y.Axis );
        }

        public override int GetHashCode()
        {
            return $"{Value}{Axis}".GetHashCode();
        }
    }
}
