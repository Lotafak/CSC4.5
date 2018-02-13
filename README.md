<h1>OneClassClassification project - Constraint Synthesis with C4.5 algorithm and testing environment</h1>

<h2>Prerequisites</h2>

Project created with Visual Studio 2015 using .NET Framework v4.5.2. To compile the solution given environment is necessary as msbuild used to build Visual Studio projects is no longer included in .NET Framework (<a href="https://blogs.msdn.microsoft.com/visualstudio/2013/07/24/msbuild-is-now-part-of-visual-studio/">more here</a>). Projects dependencies (such as Accord.NET) are managed by NuGet package manager.

Projects are in different version for different tasks (main experiments, tree-pruning, case-study) which are available under different branches on the repository, therefor to work with project Git tools are needed. See <a href="https://git-scm.com/">git command line tools</a> or <a href="https://desktop.github.com/">git desktop app</a>.

<h2>Project's structure</h2>

Main project's files are in `OneClassClassification` folder. Project output by default is in `\OneClassClassification\bin\(Debug|Release)`.

Project's test files are included in `OneClassClassificationTests` folder.

Additional script are included in `Scripts` folder in the main project's folder.

Project contains different version on separate git branches:
- master - contains main experiments version of the project
- tree-pruning - version prepared for tree-pruning experiment
- case-study - version prepared for handling case-study data

Databases experiments containing statistics and data are included in `Databases` folder.

`Case study` folder contains best output models, training and test datasets for wine-red and wine-white input datasets.