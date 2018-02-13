using System.Collections.Generic;

namespace OneClassClassification.Models
{
    /// <summary>
    /// Refers to single Decision from <see cref="Accord.MachineLearning.DecisionTrees.Rules.DecisionSet"/>
    /// </summary>
    public class Rule
    {
        public int Class { get; set; }
        public List<Constraint> Constraints { get; set; } = new List<Constraint>();

        /// <summary>
        /// Parse <see cref="Accord.MachineLearning.DecisionTrees.DecisionTree.ToRules"/>  
        /// output to <see cref="Constraint"/> list.
        /// </summary>
        /// <param name="line">One line of rules from 
        /// <see cref="Accord.MachineLearning.DecisionTrees.DecisionTree.ToRules"/> output string</param>
        public Rule(string line)
        {
            var array = line.Split(' ');
            Class = int.Parse(array[0]);
            for (var i = 2; i < array.Length; i = i+2)
            {
                if (!array[i].StartsWith("(")) continue;
                
                var axis = int.Parse(array[i].Substring(2));
                var sign = array[i + 1];
                var value = double.Parse(array[i + 2].TrimEnd(')'));
                Constraints.Add(new Constraint
                {
                    Axis = axis,
                    Value = value,
                    Sign = sign[0]  // Skip equal sign in <= or >=
                });
            }
        }
    }
}
