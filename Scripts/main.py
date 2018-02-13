import os
from os.path import isfile, join

tasks = 1
partition = 'lab-44-student'
exePath = '~/Release/OneClassClassification.exe'
scriptsPath = './scripts'

# Applicaction configuation
feasibleExamples = 500
k = 1
treeJoin = 10
treeHeight = 10
benchmarks = {'circle', 'cube', 'simplex'}


def ComponentExperiment():
    # slurm configuration
    experiment_name = 'Components'
    components_arr = [0.5, 1, 2]

    for seed in xrange(0, 30):
        for ben in benchmarks:
            for dimensions in xrange(3, 8, 1):
                for c in components_arr:
                    components = int(round(dimensions * c))
                    f = open('cmpEx/scripts/{}_{}_{}_{}_{}.sh'.format("cmp", ben, dimensions, components, seed),
                             'wb')

                    f.write('#!/bin/bash\n')

                    # sbatch
                    f.write('#SBATCH -p {}\n'.format(partition))

                    f.write('\nexport LD_LIBRARY_PATH=~/gurobi702/linux64/lib/\n')
                    f.write('export GRB_LICENSE_FILE=~/gurobi-$(hostname).lic\n')

                    f.write('\nsrun mono {} '.format(exePath))
                    f.write('{} '.format(feasibleExamples))
                    f.write('{} '.format(dimensions))
                    f.write('{} '.format(k))
                    f.write('{} '.format(treeJoin))
                    f.write('{} '.format(treeHeight))
                    f.write('{} '.format(seed))
                    f.write('{} '.format(ben))
                    f.write('{} '.format(components))
                    f.write('{}'.format(experiment_name))

                    f.close()


def TreeParametersExperiment(experiment_name):
    multiplier_arr = [1, 1.5, 2]

    for seed in xrange(0, 30):
        for ben in benchmarks:
            for dimension in xrange(3, 8, 1):
                for m1 in multiplier_arr:
                    j = int((round(m1 * dimension)))

                    for m2 in multiplier_arr:
                        max_height = int(round(m2 * j))
                        f = open('treeEx/scripts/{}_{}_{}_{}_{}.sh'.format(ben, dimension, j, max_height, seed),
                                 'wb')

                        f.write('#!/bin/bash\n')

                        # sbatch
                        f.write('#SBATCH -p {}\n'.format(partition))

                        f.write('\nexport LD_LIBRARY_PATH=~/gurobi702/linux64/lib/\n')
                        f.write('export GRB_LICENSE_FILE=~/gurobi-$(hostname).lic\n')

                        f.write('\nsrun mono {} '.format(exePath))
                        f.write('{} '.format(feasibleExamples))
                        f.write('{} '.format(dimension))
                        f.write('{} '.format(k))
                        f.write('{} '.format(j))
                        f.write('{} '.format(max_height))
                        f.write('{} '.format(seed))
                        f.write('{} '.format(ben))

                        components = dimension

                        f.write('{} '.format(components))
                        f.write('"{}"'.format(experiment_name))


def ExmaplesExperiment():
    experiment_name = 'Examples'

    for seed in xrange(0, 30):
        for ben in benchmarks:
            for dimension in xrange(3, 8, 1):
                for examples in xrange(100, 501, 100):
                    f = open('examplesEx/scripts/{}_{}_{}_{}.sh'.format(ben, dimension, examples, seed),
                             'wb')

                    j = int(round(dimension*2.0))
                    height = int(round(j*2.0))

                    f.write('#!/bin/bash\n')

                    # sbatch
                    f.write('#SBATCH -p {}\n'.format(partition))

                    f.write('\nexport LD_LIBRARY_PATH=~/gurobi702/linux64/lib/\n')
                    f.write('export GRB_LICENSE_FILE=~/gurobi-$(hostname).lic\n')

                    f.write('\nsrun mono {} '.format(exePath))
                    f.write('{} '.format(examples))
                    f.write('{} '.format(dimension))
                    f.write('{} '.format(k))
                    f.write('{} '.format(j))
                    f.write('{} '.format(height))
                    f.write('{} '.format(seed))
                    f.write('{} '.format(ben))

                    components = dimension

                    f.write('{} '.format(components))
                    f.write('"{}"'.format(experiment_name))


def MainScriptComponents():
    filename = open('./cmpEx/mainComponents1.sh', 'wb')
    scripts_path = './cmpEx/scripts'
    i = 0
    counter = 1

    onlyfiles = [f for f in os.listdir(scripts_path) if isfile(join(scripts_path, f))]
    for f in onlyfiles:
        if (i >= (counter - 1) * 1000) and (i < counter * 1000):
            filename.write('sbatch ./scripts/{}\n'.format(f))
        i += 1
        if i >= counter * 1000:
            counter += 1
            filename = open('./treeEx/mainTreeParameters{}.sh'.format(counter), 'wb')


def MainScriptTreeParameters():
    fa = open('./treeEx/mainTreeParameters1.sh', 'wb')
    scripts_path = './treeEx/scripts'
    i = 0
    counter = 1

    onlyfiles = [f for f in os.listdir(scripts_path) if isfile(join(scripts_path, f))]
    for f in onlyfiles:
        if (i >= (counter - 1) * 1000) and (i < counter * 1000):
            fa.write('sbatch ./scripts/{}\n'.format(f))
        i += 1
        if i >= counter * 1000:
            counter += 1
            fa = open('./treeEx/mainTreeParameters{}.sh'.format(counter), 'wb')



def MainScriptExamples():
    fa = open('./examplesEx/mainExamples1.sh', 'wb')
    scripts_path = './examplesEx/scripts'
    i = 0
    counter = 1

    onlyfiles = [f for f in os.listdir(scripts_path) if isfile(join(scripts_path, f))]
    for f in onlyfiles:
        if (i >= (counter - 1) * 1000) and (i < counter * 1000):
            fa.write('sbatch ./scripts/{}\n'.format(f))
        i += 1
        if i >= counter * 1000:
            counter += 1
            fa = open('./examplesEx/mainExamples{}.sh'.format(counter), 'wb')


if __name__ == "__main__":
    ComponentExperiment()
    # TreeParametersExperiment('PrunedTree')
    # TreeParametersExperiment('Tree')
    # ExmaplesExperiment()

    MainScriptComponents()
    # MainScriptTreeParameters()
    # MainScriptExamples()
