<h1>Python scripts for generating tables in LaTeX and slurm tasks</h1>
<h2>Prerequisites</h2>

- Python in version 2.7.13 with sqlite3 and numpy packages are required.

To run the scripts without modifications the databases with the experimental results must be included in the same directory:
- testDatabase.sqlite - for main experiments database
- treePruningDb.sqlite - for database with results from tree pruning experiment

<h2>Usage</h2>
To run script from command line simply execute

```
python (script_name)
```

<h2>Generating tables - latexTable.py</h2>

Script used to generate tables for three experiments conducted in the project: Ball _n_, Cube _n_, and Simplex _n_. 

<h2>Generating slurm tasks - main.py</h2>

Script used to generate tasks to run experiments in the <a href="https://slurm.schedmd.com/">slurm</a> environment.
