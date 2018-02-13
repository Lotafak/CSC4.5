using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneClassClassification.Data;
using OneClassClassification.Utils;

namespace OneClassClassificationTests.Utils
{
    [TestClass()]
    public class DatabaseUtilsTests
    {
        //[TestMethod()]
        //public void CheckIfExistsTest()
        //{
        //    GlobalVariables.Dbpath = @"C:\Users\patry\Documents\Visual Studio 2015\Projects\OneClassClassification\OneClassClassification\bin\Release\testDatabase.sqlite";

        //    var args1 = new[] { "10000", "7", "0.4", "10", "10", "5", "cube", "10", "Components" };
        //    var args2 = new[] { "10000", "7", "0.4", "10", "10", "5", "cube", "10", "" };
        //    var args3 = new[] { "10000", "7", "0.4", "10", "10", "5", "simplex", "10", "" };
        //    var args4 = new[] { "10000", "7", "0.4", "10", "11", "5", "simplex", "10", "" };

        //    //    var args2 = new[] {"1000", "2", "0.4", "2", "4", "1", "simplex", "1", "Experiment 1"};
        //    //    var args3 = new[] {"1000", "2", "0.4", "2", "4", "1", "simplex", "1", "Experiment 2"};

        //    //    var args4 = new[] {"95", "3", "0.2", "10", "10", "1", "circle", "1"};
        //    //    var args5 = new[] {"90", "3", "0.3", "10", "10", "1", "circle", "1"};
        //    //    var args6 = new[] {"90", "3", "0.2", "10", "10", "1", "circle", "3"};
        //    //    var args7 = new[] { "80", "3", "0.2", "10", "10", "1", "circle", "1" };
        //    //    var args8 = new[] { "70", "3", "0.2", "10", "10", "1", "circle", "1" };
        //    //    var args9 = new[] { "60", "3", "0.2", "10", "10", "1", "circle", "1" };

        //    const string seed = "";

        //    Assert.AreEqual(true, DatabaseUtils.CheckIfExists(args1), args1.Aggregate(seed, (str, s) => str + $"{s} "));
        //    Assert.AreEqual(true, DatabaseUtils.CheckIfExists(args2), args2.Aggregate(seed, (str, s) => str + $"{s} "));
        //    Assert.AreEqual(true, DatabaseUtils.CheckIfExists(args3), args3.Aggregate(seed, (str, s) => str + $"{s} "));
        //    Assert.AreEqual(false, DatabaseUtils.CheckIfExists(args4), args4.Aggregate(seed, (str, s) => str + $"{s} "));
        //    //    Assert.AreEqual(true, DatabaseUtils.CheckIfExists(args2), args2.Aggregate(seed, (str, s) => str + $"{s} "));
        //    //    Assert.AreEqual(true, DatabaseUtils.CheckIfExists(args3), args3.Aggregate(seed, (str, s) => str + $"{s} "));

        //    //    Assert.AreEqual(false, DatabaseUtils.CheckIfExists(args4), args4.Aggregate(seed, (str, s) => str + $"{s} "));
        //    //    Assert.AreEqual(false, DatabaseUtils.CheckIfExists(args5), args5.Aggregate(seed, (str, s) => str + $"{s} "));
        //    //    Assert.AreEqual(false, DatabaseUtils.CheckIfExists(args6), args6.Aggregate(seed, (str, s) => str + $"{s} "));
        //    //    Assert.AreEqual(false, DatabaseUtils.CheckIfExists(args7), args7.Aggregate(seed, (str, s) => str + $"{s} "));
        //    //    Assert.AreEqual(false, DatabaseUtils.CheckIfExists(args8), args8.Aggregate(seed, (str, s) => str + $"{s} "));
        //    //    Assert.AreEqual(false, DatabaseUtils.CheckIfExists(args9), args9.Aggregate(seed, (str, s) => str + $"{s} "));
        //}
}
}