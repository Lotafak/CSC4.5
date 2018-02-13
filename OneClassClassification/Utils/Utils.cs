using System;
using System.Collections.Generic;
using System.Linq;

namespace OneClassClassification.Utils
{
    public static class Utils
    {
        ///<summary>
        /// Calculates entropy for one class 
        /// </summary>
        public static double Entropy( List<int> leaves, int[] nodes, double distribution )
        {
            var p1 = ( (double)nodes.Count(x => x == 1) / leaves.Count(x => x == 1) ) *
                 ( (double)leaves.Count(x => x == 0) / nodes.Count(x => x == 0) ) *
                 distribution;
            p1 = p1 > 1 ? 1 : p1;
            var p2 = 1 - p1;

            if ( p1 == 0 )
                return -p2 * Math.Log(p2, 2);
            if ( p2 == 0 )
                return -p1 * Math.Log(p1, 2);
            return -p1 * Math.Log(p1, 2) - p2 * Math.Log(p2, 2);
        }

        /// <summary>
        /// Extends array with column of specified values
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<double[]> AddColumnWithValues(this List<double[]> list, double value)
        {
            return list.Select(element => element.ExtendArrayWithValue(value)).ToList();
        }

        /// <summary>
        /// Adds a value on the end of the array
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double[] ExtendArrayWithValue(this double[] list, double value)
        {
            var newArr = new double[list.Length + 1];
            for ( var i = 0; i < list.Length; i++ )
            {
                newArr[i] = list[i];
            }

            newArr[list.Length] = value;
            return newArr;
        }
    }
}
