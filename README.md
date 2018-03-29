<h1>Constraint Synthesis with C4.5 (CSC4.5) algorithm and testing environment</h1>

<h2>Corresponding article</h2>
CSC4.5 was created in research reported in the article
Patryk Kudła, Tomasz P. Pawlak, <a href="https://www.sciencedirect.com/science/article/pii/S1568494618301479">One-class synthesis of constraints for Mixed-Integer Linear Programming with C4.5 decision trees</a>, Applied Soft Computing, Elsevier, 2018.

Bibtex:
```
@article{Kudla2018,
title = "One-class synthesis of constraints for Mixed-Integer Linear Programming with C4.5 decision trees ",
journal = "Applied Soft Computing ",
volume = "",
number = "",
pages = "",
year = "2018",
note = " (in press)",
issn = "1568-4946",
doi = "https://doi.org/10.1016/j.asoc.2018.03.025",
url = "https://www.sciencedirect.com/science/article/pii/S1568494618301479",
author = "Patryk Kud\l{}a and Tomasz P. Pawlak",
keywords = "Model acquisition",
keywords = "Constraint synthesis",
keywords = "Mathematical programming",
keywords = "One-class classification",
keywords = "Business process "
}
```

<h2>Prerequisites</h2>

CSC4.5 was created with the Visual Studio 2015 using .NET Framework v4.5.2. Project dependencies (such as Accord.NET) are managed by the NuGet package manager.

The branches of this repository reflect tasks conducted in the corresponding article. Therefore, Git tools are needed to handle different tasks. See <a href="https://git-scm.com/">git command line tools</a> or <a href="https://desktop.github.com/">git desktop app</a>.

<h2>Structure of the project</h2>

Main files are in `OneClassClassification` directory. Project output by default is in `\OneClassClassification\bin\(Debug|Release)`.

Test files are included in `OneClassClassificationTests` directory.

Additional scripts are included in the `Scripts` directory.

Project contains different version on separate git branches:
- master - contains the main experiments conducted in the research,
- tree-pruning - the version prepared for tree-pruning experiment,
- case-study - the version prepared for handling case-study data.

Databases with experimental results are included in `Databases` directory.

`Case study` directory contains the best synthesized MIQP models, training and test datasets for the wine-red and the wine-white input datasets.