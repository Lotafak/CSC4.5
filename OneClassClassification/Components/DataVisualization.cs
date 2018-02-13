using System.Diagnostics;
using OneClassClassification.Data;

namespace OneClassClassification.Components
{
    /// <summary>
    /// Used to visualize data
    /// </summary>
    class DataVisualization
    {
        public string FileName { get; set; } = "cmd.exe";
        public string Arguments { get; set; } = "/C http-server -p 8008 &";

        public string Url { get; set; }

        private string BaseUrl { get; } = "http://localhost:8008/Plotly";
        private readonly string _projectPath;

        public DataVisualization()
        {
            _projectPath = GlobalVariables.ProjectPath;
            Url = $"{BaseUrl}{GlobalVariables.BenchmarkName}.html";
        }

        public void ShowData()
        {
            // Creating server process
            var process = new Process();
            var startInfo = new ProcessStartInfo
            {
                FileName = FileName,
                WorkingDirectory = _projectPath,
                Arguments = Arguments,
                UseShellExecute = false
            };

            process.StartInfo = startInfo;
            process.Start();

            // Opening file in default browser
            Process.Start(Url);
        }
    }
}
