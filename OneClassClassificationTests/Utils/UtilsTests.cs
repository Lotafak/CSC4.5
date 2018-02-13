using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneClassClassification.Utils;

namespace OneClassClassificationTests.Utils
{
    [TestClass()]
    public class UtilsTests
    {
        [TestMethod()]
        public void AddColumnWithValuesTest()
        {
            const double value = 1.0;

            var list = new List<double[]>
            {
                new[] {2.0, 3.0},
                new[] {3.0, 3.0}
            };

            var resultList = list.AddColumnWithValues(value);

            Assert.IsTrue(resultList.All(x => x[x.Length-1] == value));
            
        }

        [TestMethod()]
        public void ExtendArrayWithValueTest()
        {
            var inputArr1 = new[] {1.0, 1.0, 2.0};
            var testArr1 = new[] { 1.0, 1.0, 2.0, 1.0 };

            var inputArr2 = new[] {1.0, 1.0, 2.0, 3.0, 101.0};
            var testArr2 = new[] { 1.0, 1.0, 2.0, 3.0, 101.0, 857.0 };

            for (var i = 0; i < testArr1.Length; i++)
            {
                Assert.AreEqual(inputArr1.ExtendArrayWithValue(1.0)[i], testArr1[i]);
            }


            for ( var i = 0; i < testArr2.Length; i++ )
            {
                Assert.AreEqual(inputArr2.ExtendArrayWithValue(857.0)[i], testArr2[i]);
            }
        }
    }
}