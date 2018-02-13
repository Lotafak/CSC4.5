using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gurobi;
using OneClassClassification.Models;

namespace OneClassClassification.Utils
{
    public static class ConstraintExtension
    {
        /// <summary>
        /// Method is creating list of unique constraints with number of splits (boolean variables) in list
        /// </summary>
        /// <param name="rules">List of rules. Positive rules will be extracted (Can also be only positive)</param>
        /// <returns>Item1 is a list of <see cref="Constraint"/> and Item2 is number of bool variables</returns>
        public static Tuple<List<Constraint>, int> ReturnUniqueSplitsWithBooleans( this List<Rule> rules )
        {
            var constraints = new List<Constraint>();
            var first = true;

            // Get postive rules only
            var positiveRules = rules.FindAll(x => x.Class == 1);

            if (positiveRules.Count == 0)
            {
                throw new ArgumentNullException(nameof(rules), $"There were no positive rules in Rules set\nClass: {MethodBase.GetCurrentMethod()?.DeclaringType?.Name}\nMethod: {MethodBase.GetCurrentMethod().Name}");
            }

            // Search for constraint splits ( Sign doesn't matter )
            var splits = SearchSplits(positiveRules);

            // Adds dictionary to indicate split number 
            var boolCounter = 0;
            var splitDictionary = splits.ToDictionary(split => split, split => boolCounter++);

            foreach ( var rule in positiveRules )
            {
                for ( var i = 0; i < rule.Constraints.Count; i++ )
                {
                    if ( !splits.Contains(rule.Constraints[i]) ) continue;

                    if ( first )
                    {
                        constraints.AddRange(rule.Constraints.GetRange(0, i));
                        first = false;
                    }

                    constraints = constraints
                        .Union(
                            CopyToList(
                                rule, 
                                i, 
                                splitDictionary[rule.Constraints[i]]
                            ), new CompareWithSign()
                        ).ToList();
                }
            }

            if(first)
                constraints.AddRange(positiveRules[0].Constraints);

            return new Tuple<List<Constraint>, int>(constraints, splits.Count);
        }

        /// <summary>
        /// Method looking for tree splits among set of <see cref="Rule"/>s
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        private static HashSet<Constraint> SearchSplits( this IReadOnlyCollection<Rule> rules )
        {
            var splits = new HashSet<Constraint>();

            if ( rules.Count == 0 )
                return splits;

            // Get longest rule count
            var max = rules.Max(x => x.Constraints.Count);

            for ( var column = 0; column < max; column++ )
            {
                var tempConstraints = new List<Constraint>();

                foreach ( var t in rules )
                {
                    if ( t.Constraints.Count <= column ) continue;

                    var firstConstraint = t.Constraints[column];

                    foreach ( var constraint in tempConstraints )
                    {
                        // Split is unique and need to have the same value, but different sign
                        if ( splits.Contains(constraint) ||
                             constraint.Value != firstConstraint.Value ||
                             constraint.Sign == firstConstraint.Sign ) continue;

                        splits.Add(constraint);
                    }

                    tempConstraints.Add(t.Constraints[column]);
                }
            }
            return splits;
        }

        private static List<Constraint> CopyToList( Rule rule, int column, int splitNumber )
        {
            var constraints = new List<Constraint>();
            var oneMinus = rule.Constraints[column].Sign != GRB.GREATER_EQUAL;

            for ( var i = column; i < rule.Constraints.Count; i++ )
            {
                rule.Constraints[i].Splits.Add(new Split
                {
                    OriginalSign = rule.Constraints[i].Sign,
                    OneMinus = oneMinus,
                    SplitNumber = splitNumber
                });
                constraints.Add(rule.Constraints[i]);
            }

            return constraints;
        }
    }
}