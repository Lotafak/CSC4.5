using System;
using System.IO;
using ExperimentDatabase;
using Xunit;

namespace ExperimentDatabaseTest
{
    public class DatabaseTests : IDisposable
    {
        private readonly FileInfo dbFile = new FileInfo(
            Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\testDatabase")));
        private readonly Database db;
        private readonly Experiment experiment;

        public DatabaseTests()
        {
            if(dbFile.Exists)
                dbFile.Delete();

            File.Create(dbFile.FullName).Dispose();

            this.db = new Database(dbFile.FullName);
            //this.experiment = db.NewExperiment();
        }

        [Fact]
        public void TestMethod1()
        {

        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose( bool disposing )
        {
            this.experiment.Dispose();
            this.db.Dispose();
            this.dbFile.Delete();

            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        ~DatabaseTests()
        {
            this.Dispose(false);
        }
    }
}
