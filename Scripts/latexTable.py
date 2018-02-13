import sqlite3
import math
import numpy as np

ConstraintsStd = 'ConstraintsStd'
TermsStd = 'TermsStd'
JaccardStd = 'JaccardStd'
PrecisionStd = 'PrecisionStd'
RecallStd = 'RecallStd'
MeanAngleStd = 'MeanAngleStd'
cellBarMaxHeight = 7
tikzString = '\\begin{{tikzpicture}}[baseline=0.4pt]' \
             '\\draw[line width=2](0,0pt) -- (0,{}pt);' \
             '\\end{{tikzpicture}}'
avgStdStatisticsString = "AVG(Constraints), AVG(terms), AVG(HJaccard), " \
                         "AVG(CAST(HTP AS float)/(HTP+HFN)), AVG(CAST(HTP AS float)/(HTP+HFP)), " \
                         "AVG(Meanangle), ConstraintsStd, TermsStd, JaccardStd, PrecisionStd, " \
                         "RecallStd, MeanAngleStd"
t_threshold = 2.045

cccfactor = 0
cctfactor = 0
ccjfactor = 0
ccpfactor = 0
ccrfactor = 0
ccmfactor = 0


class StdevFunc:
    def __init__(self):
        self.M = 0.0
        self.S = 0.0
        self.k = 1

    def step(self, value):
        if value is None:
            return
        t_m = self.M
        self.M += (value - t_m) / self.k
        self.S += (value - t_m) * (value - self.M)
        self.k += 1

    def finalize(self):
        if self.k < 3:
            return None
        return math.sqrt(self.S / (self.k - 1))


def CreateIfColumnNotExists(c, columnName):
    c.execute("PRAGMA table_info(experiments)")
    name = c.fetchone()
    find = 0
    while name is not None:
        if name[1] == columnName:
            find = 1
        name = c.fetchone()
    if find == 0:
        c.execute("ALTER TABLE experiments ADD {} NUMERIC".format(columnName))


def CheckTable(c):
    CreateIfColumnNotExists(c, ConstraintsStd)
    CreateIfColumnNotExists(c, TermsStd)
    CreateIfColumnNotExists(c, JaccardStd)
    CreateIfColumnNotExists(c, PrecisionStd)
    CreateIfColumnNotExists(c, RecallStd)
    CreateIfColumnNotExists(c, MeanAngleStd)


def CalculateStdevAndSaveToDb(c, where_str, arr):
    c.execute("select stdev(Constraints), stdev(Terms), stdev(HJaccard), "
              "stdev(CAST(HTP AS float)/(HTP+HFN)), stdev(CAST(HTP AS float)/(HTP+HFP)), "
              "stdev(MeanAngle) from experiments WHERE {}".format(where_str), arr)
    results = list(c.fetchone())
    results = results + arr

    s = "UPDATE experiments SET {} = ?, {} = ?, {} = ?, {} = ?, {} = ?, " \
        "{} = ? WHERE {}".format(ConstraintsStd, TermsStd, JaccardStd, PrecisionStd,
                                 RecallStd, MeanAngleStd, where_str)
    c.execute(s, results)  # REQUIRES COMMIT


def CalculateTTestPValue(c_main, c_pruning, where_str, arr):
    query = "SELECT Constraints, Terms, HJaccard, (CAST(HTP AS float)/(HTP+HFN))," \
            "(CAST(HTP AS float)/(HTP+HFP)), MeanAngle FROM experiments WHERE {} ORDER BY Seed ASC" \
        .format(where_str)

    arr1 = np.array(c_main.execute(query, arr).fetchall(), dtype=np.float64)
    arr1[np.isnan(arr1)] = 0

    arr2 = np.array(c_pruning.execute(query, arr).fetchall(), dtype=np.float64)
    arr2[np.isnan(arr2)] = 0

    if np.shape(arr1)[0] != np.shape(arr2)[0]:
        return [False, False, False, False, False, False]

    diff_arr = np.subtract(arr1, arr2)
    mu_avg = np.average(diff_arr, axis=0)

    std_diff = np.std(diff_arr, axis=0)

    dof = np.shape(diff_arr)[0]
    t = abs(np.divide(mu_avg, (std_diff / math.sqrt(dof))))
    return [p > t_threshold for p in t]


def writeHeader(file):
    file.write('\\begin{tabular}{ccc}\n')


def writeStatisticsHeader(file):
    file.write('\\multicolumn{1}{c|}{\\begin{turn}{270}Constraints\\end{turn}} & ')
    file.write('\\multicolumn{1}{c|}{\\begin{turn}{270}Terms\\end{turn}} & ')
    file.write('\\multicolumn{1}{c|}{\\begin{turn}{270}Jaccard Index\\end{turn}} & ')
    file.write('\\multicolumn{1}{c|}{\\begin{turn}{270}Precision\\end{turn}} & ')
    file.write('\\multicolumn{1}{c|}{\\begin{turn}{270}Recall\\end{turn}} & ')
    file.write('\\multicolumn{1}{c|}{\\begin{turn}{270}Mean angle\\end{turn}}\t\\\\ \\hline\n')


def writeComponentsTableHeader(file, ben):
    file.write('\\setlength\\tabcolsep{1px}\n')
    file.write('\\begin{tabular}{|c|c||r|r|r|r|r|r|r|}\n')
    file.write('\\hline')
    file.write('\\multicolumn{{8}}{{|c|}}{{{}}}   \\\\ \\hline\n'.format(ben))
    file.write('\\begin{turn}{270}Dimensions\\end{turn} & ')
    file.write('\\begin{turn}{270}Components\\end{turn} & ')
    writeStatisticsHeader(file)


def writeTreeTableHeader(file, ben):
    file.write('\\setlength\\tabcolsep{0.7px}\n')
    file.write('\\begin{tabular}{|c|c|c||r|r|r|r|r|r|r|}\n')
    file.write('\\hline')
    file.write('\\multicolumn{{9}}{{|c|}}{{{}}}   \\\\ \\hline\n'.format(ben))
    file.write('\\begin{turn}{270}Dimensions\\end{turn} & ')
    file.write('\\begin{turn}{270}Join\\end{turn} & ')
    file.write('\\begin{turn}{270}Max Height\\end{turn} & ')
    writeStatisticsHeader(file)


def writeExamplesTableHeader(file, ben):
    file.write('\\setlength\\tabcolsep{0.7px}\n')
    file.write('\\begin{tabular}{|c|c||r|r|r|r|r|r|r|}\n')
    file.write('\\hline')
    file.write('\\multicolumn{{8}}{{|c|}}{{{}}}   \\\\ \\hline\n'.format(ben))
    file.write('\\begin{turn}{270}Dimensions\\end{turn} & ')
    file.write('\\begin{turn}{270}Feasible Examples\\end{turn} & ')
    writeStatisticsHeader(file)


def writeAvgRows(file, c, pagebreak):
    row = c.fetchone()
    while row:
        l = list(row)
        for i, x in enumerate(l):
            if x is None:
                l[i] = 0
        file.write(' & {:.0f} & {:.0f} & {:.3f} & {:.3f} & {:.3f} & {:.3f}'
                   .format(l[0],
                           l[1],
                           l[2],  # Jaccard
                           l[3] / (l[3] + l[5]) if (l[3] + l[5]) > 0 else 0.0,  # Precision  (HTP/(HTP+HFP))
                           l[3] / (l[3] + l[4]) if (l[3] + l[4]) > 0 else 0.0,  # Recall     (HTP/(HTP+HFN))
                           l[6]))
        row = c.fetchone()
    file.write('    \\\\')
    if pagebreak:
        file.write(' \\cline{2-9}')
    file.write('\n')


def writeAvgStdRows(f, c, pagebreak, max_stds, is_significant):
    row = c.fetchone()
    while row:
        l = list(row)
        for i, x in enumerate(l):
            if x is None:
                l[i] = 0

        i_significant = ' & \\cci{{{:.0f}}}{{\\underline{{{:.0f}}}}} {}'
        i_insignificant = ' & \\cci{{{:.0f}}}{{{:.0f}}} {}'
        f_significant = ' & \\ccf{{{:.0f}}}{{\\underline{{{:.3f}}}}} {}'
        f_insignificant = ' & \\ccf{{{:.0f}}}{{{:.3f}}} {}'

        s = i_significant if is_significant[0] else i_insignificant
        f.write(s.format(l[0] * cccfactor, l[0], tikzString.format((cellBarMaxHeight * l[6]) / max_stds[0])))

        s = i_significant if is_significant[1] else i_insignificant
        f.write(s.format(l[1] * cctfactor, l[1], tikzString.format((cellBarMaxHeight * l[7]) / max_stds[1])))

        s = f_significant if is_significant[2] else f_insignificant
        f.write(s.format(l[2] * ccjfactor, l[2], tikzString.format((cellBarMaxHeight * l[8]) / max_stds[2])))

        s = f_significant if is_significant[3] else f_insignificant
        f.write(s.format(l[3] * ccpfactor, l[3], tikzString.format((cellBarMaxHeight * l[9]) / max_stds[3])))

        s = f_significant if is_significant[4] else f_insignificant
        f.write(s.format(l[4] * ccrfactor, l[4], tikzString.format((cellBarMaxHeight * l[10]) / max_stds[4])))

        s = ' & \\cci{{{:.3f}}}{{\\underline{{{:.3f}}}}} {}' if is_significant[5] else ' & \\cci{{{:.3f}}}{{{:.3f}}} {}'
        f.write(s.format(l[5] * ccmfactor[benchmark_indi],
                         l[5],
                         tikzString.format(((cellBarMaxHeight * l[11]) / max_stds[5]) if max_stds[5] != 0.0 else 0.0)))
        row = c.fetchone()
    f.write('    \\\\')
    if pagebreak:
        f.write(' \\cline{2-9}')
    f.write('\n')


def writeFooter(file):
    file.write('\\end{tabular}\n')
    # file.write('}\n')
    # file.write('\\caption{Components table}\n')
    # file.write('\\end{table}\n')
    # file.write('\\end{document}')


def ComponentsTable():
    global cccfactor, cctfactor, ccjfactor, ccpfactor, ccrfactor, ccmfactor, benchmark_indi
    ccmfactor = [-1, -1, -1]
    benchmarks = ('circle', 'cube', 'simplex')
    multiplierArr = [0.5, 1, 2]

    conn = sqlite3.connect("testDatabase.sqlite")
    conn.create_aggregate("stdev", 1, StdevFunc)
    c = conn.cursor()
    CheckTable(c)

    benchmark_indi = -1

    # Calculate std Deviations for experiment
    for ben in benchmarks:
        benchmark_indi += 1

        for dimensions in xrange(3, 8):
            for m in multiplierArr:
                components = int(round(m * dimensions))

                CalculateStdevAndSaveToDb(c,
                                          "Benchmark = ? AND Dimensions = ? AND "
                                          "Components = ? AND Errors = '' AND "
                                          "ExperimentName LiKE '%Components%'",
                                          [ben, dimensions, components])
                conn.commit()

                c.execute("SELECT AVG(Constraints), AVG(Terms), AVG(MeanAngle) FROM experiments WHERE "
                          "Benchmark = ? AND Dimensions = ? AND Components = ? AND Errors = '' AND "
                          "ExperimentName LiKE '%Components%'",
                          [ben, dimensions, components])

                arr = c.fetchone()
                if cccfactor < arr[0]:  cccfactor = arr[0]
                if cctfactor < arr[1]:  cctfactor = arr[1]
                if ccmfactor[benchmark_indi] < arr[2]:  ccmfactor[benchmark_indi] = arr[2]

        ccmfactor[benchmark_indi] = 100.0 / ccmfactor[benchmark_indi]

    cccfactor = 100.0 / cccfactor
    cctfactor = 100.0 / cctfactor
    ccjfactor = 100
    ccpfactor = 100
    ccrfactor = 100

    f = open('ComponentsTable.tex', 'wb')

    writeHeader(f)

    max_stds = [10, 100, 0.5, 0.5, 0.5, 0.5]

    benchmark_indi = -1

    for ben in benchmarks:
        benchmark_indi += 1
        writeComponentsTableHeader(f, ben)

        for dimensions in xrange(3, 8):
            f.write('\\multirow{{{}}}{{*}}{{{}}}'.format(3, dimensions))

            for m in multiplierArr:
                components = int(round(m * dimensions))
                f.write(' & {}'.format(components))
                dim = (dimensions, components, ben)
                c.execute("SELECT {} FROM experiments WHERE Dimensions = ? AND Components = ? AND "
                          "Benchmark = ? AND ExperimentName LIKE '%Components%' AND Errors = ''"
                          .format(avgStdStatisticsString), dim)

                # writeAvgRows(f, c, 0)
                writeAvgStdRows(f, c, 0, max_stds, [False, False, False, False, False, False])

            f.write('\\hline')
            if dimensions != 7:
                f.write(' \\hline')
            f.write('\n')

        f.write('\\end{tabular}\n')
        if ben is not 'simplex':
            f.write('&\n')
    writeFooter(f)
    f.close()


def TreeTable(db, filename):
    global cccfactor, cctfactor, ccmfactor, ccjfactor, ccrfactor, ccpfactor, benchmark_indi
    ccmfactor = [-1, -1, -1]
    conn_main = sqlite3.connect(db)
    conn_pruning = sqlite3.connect('treePruningDb.sqlite')

    conn_main.create_aggregate("stdev", 1, StdevFunc)

    c1 = conn_main.cursor()
    c2 = conn_pruning.cursor()

    CheckTable(c1)
    benchmarks = ('circle', 'cube', 'simplex')
    f = open(filename, 'wb')
    multiplierArr = [1, 1.5, 2]

    filterString = "Benchmark = ? AND Dimensions = ? AND [Join] = ? AND MaxHeight = ? AND Errors = '' AND " \
                   "ExperimentName LIKE '%Tree%'"

    benchmark_indi = -1

    # Calculate std Deviations for experiment
    for ben in benchmarks:
        benchmark_indi += 1

        for dimensions in xrange(3, 8):
            for m1 in multiplierArr:
                join = int(round(m1 * dimensions))

                for m2 in multiplierArr:
                    maxHeight = int(round(m2 * join))
                    paramsArr = [ben, dimensions, join, maxHeight]

                    CalculateStdevAndSaveToDb(c1,
                                              filterString,
                                              paramsArr)

                    conn_main.commit()

                    c1.execute("SELECT AVG(Constraints), AVG(Terms), AVG(MeanAngle) FROM experiments WHERE "
                               "{}".format(filterString),
                               paramsArr)

                    arr = c1.fetchone()
                    if cccfactor < arr[0]:  cccfactor = arr[0]
                    if cctfactor < arr[1]:  cctfactor = arr[1]
                    if ccmfactor[benchmark_indi] < arr[2]:  ccmfactor[benchmark_indi] = arr[2]

        ccmfactor[benchmark_indi] = 100.0 / ccmfactor[benchmark_indi]

    cccfactor = 100.0 / cccfactor
    cctfactor = 100.0 / cctfactor
    ccjfactor = 100
    ccpfactor = 100
    ccrfactor = 100

    writeHeader(f)

    max_stds = [15, 350, 0.5, 0.5, 0.5, 0.5]

    benchmark_indi = -1

    for ben in benchmarks:
        benchmark_indi += 1
        writeTreeTableHeader(f, ben)

        for dimension in xrange(3, 8):
            f.write('\\multirow{{{}}}{{*}}{{{}}}'.format(9, dimension))

            for m1 in multiplierArr:
                join = int(round(m1 * dimension))
                f.write(' & \\multirow{{{}}}{{*}}{{{}}}'.format(3, join))
                indi = 1

                for m2 in multiplierArr:
                    maxHeight = int(round(m2 * join))
                    if indi != 1:
                        f.write(' & ')
                    indi = 0

                    dim = (ben, dimension, join, maxHeight)

                    is_significant = CalculateTTestPValue(c1, c2, filterString, dim)

                    f.write(' & {}'.format(maxHeight))
                    c1.execute("SELECT {} FROM experiments WHERE Benchmark = ? AND Dimensions = ? AND [Join] = ? "
                               "AND MaxHeight = ? AND ExperimentName LIKE '%Tree%' AND "
                               "Errors = ''".format(avgStdStatisticsString), dim)

                    pagebreak = 0
                    if maxHeight == 2 * join:
                        pagebreak = 1

                    writeAvgStdRows(f, c1, pagebreak, max_stds, is_significant)

            f.write('\\hline')
            if dimension != 7:
                f.write(' \\hline')
            f.write('\n')

        f.write('\\end{tabular}\n')
        if ben != 'simplex':
            f.write('&\n')
    writeFooter(f)
    f.close()


def ExamplesTable():
    global cccfactor, cctfactor, ccmfactor, ccjfactor, ccrfactor, ccpfactor, benchmark_indi
    ccmfactor = [-1, -1, -1]
    conn = sqlite3.connect("testDatabase.sqlite")
    conn.create_aggregate("stdev", 1, StdevFunc)
    c = conn.cursor()
    CheckTable(c)
    benchmarks = ('circle', 'cube', 'simplex')
    f = open('ExamplesTable.tex', 'wb')

    filter_string = "Benchmark = ? AND Dimensions = ? AND FeasibleExamples = ? AND Errors = '' AND " \
                    "ExperimentName LIKE '%Examples%'"

    benchmark_indi = -1

    # Calculate std Deviations for experiment
    for ben in benchmarks:
        benchmark_indi += 1

        for dimensions in xrange(3, 8):
            for examples in xrange(100, 501, 100):
                paramsArr = [ben, dimensions, examples]
                CalculateStdevAndSaveToDb(c,
                                          filter_string,
                                          paramsArr)
                conn.commit()

                c.execute("SELECT AVG(Constraints), AVG(Terms), AVG(MeanAngle) FROM experiments WHERE "
                          "{}".format(filter_string),
                          paramsArr)

                arr = c.fetchone()
                if cccfactor < arr[0]:  cccfactor = arr[0]
                if cctfactor < arr[1]:  cctfactor = arr[1]
                if ccmfactor[benchmark_indi] < arr[2]:  ccmfactor[benchmark_indi] = arr[2]

        ccmfactor[benchmark_indi] = 100.0 / ccmfactor[benchmark_indi]

    cccfactor = 100.0 / cccfactor
    cctfactor = 100.0 / cctfactor
    ccjfactor = 100
    ccpfactor = 100
    ccrfactor = 100

    writeHeader(f)

    max_stds = [13, 350, 0.5, 0.5, 0.5, 0.5]

    benchmark_indi = -1

    for ben in benchmarks:
        benchmark_indi += 1
        writeExamplesTableHeader(f, ben)

        for dimension in xrange(3, 8):
            f.write('\\multirow{{{}}}{{*}}{{{}}}'.format(5, dimension))

            for examples in xrange(100, 501, 100):
                f.write(' & {}'.format(examples))
                dim = (ben, dimension, examples)
                c.execute("SELECT {} FROM experiments WHERE {}".format(avgStdStatisticsString, filter_string),
                          dim)

                writeAvgStdRows(f, c, 0, max_stds, [False, False, False, False, False, False])

            f.write('\\hline')
            if dimension != 7:
                f.write(' \\hline')
            f.write('\n')

        f.write('\\end{tabular}\n')
        if ben != 'simplex':
            f.write('&\n')
    writeFooter(f)
    f.close()


if __name__ == "__main__":
    ComponentsTable()
    TreeTable('testDatabase.sqlite', 'TreeTable.tex')
    TreeTable('treePruningDb.sqlite', 'PrunedTreeTable.tex')
    ExamplesTable()
