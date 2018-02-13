namespace OneClassClassification.Models
{
    /// <summary>
    /// Represents split point (Auxiliary binary variable)
    /// </summary>
    public class Split
    {
        /// <summary>
        /// Indicated whether split output should be M(1 - bx) or Mbx
        /// </summary>
        public bool OneMinus { get; set; }

        /// <summary>
        /// Sign in the decision tree <see cref="Rule"/>s output
        /// </summary>
        public char OriginalSign { get; set; }

        /// <summary>
        /// Value of the split
        /// </summary>
        public int SplitNumber { get; set; }
    }
}
