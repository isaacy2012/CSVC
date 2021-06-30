using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;

namespace CSVC {
    public static class Csv {

        /// <summary>
        /// Make replacements on OutputCsvLineList
        /// </summary>
        public static List<List<string>> MakeReplacements(
            ConfigParams parameters,
            List<Rule> rulesList,
            List<List<string>> csvLineList
        ) {
            var outputCsvLineList = new List<List<string>>();
            bool first = true;
            foreach (var line in csvLineList) {
                var outputLine = line.ToList();
                if (first && parameters.Header) {
                    first = false;
                    parameters.LineWriter.WriteHeader(outputLine);
                    goto outer;
                }
                foreach (var rule in rulesList.Where(rule => rule.Applies(line, parameters.ReadColumns))) {
                    parameters.LineWriter.Write(outputLine, rule.Substitution);
                    goto outer;
                }
                
                parameters.LineWriter.Write(outputLine, string.Empty);

                outer: {}
                outputCsvLineList.Add(outputLine);
            }
            return outputCsvLineList;
        }

        /// <summary>
        /// Parse a CSV from the filename
        /// </summary>
        /// <param name="filename">The filename</param>
        /// <returns>A List of a List of Strings</returns>
        public static List<List<string>> ParseCsv(string filename) {
            var csvLineList = new List<List<string>>();
            foreach (string line in File.ReadAllLines(filename)) {
                var columnList = line.Split(",").ToList();
                csvLineList.Add(columnList);
            }
            return csvLineList;
        }

        /// <summary>
        /// Save a stream to a Csv
        /// </summary>
        /// <param name="s"></param>
        /// <param name="outputCsvLineList"></param>
        public static void SaveToCsv(Stream s, List<List<string>> outputCsvLineList) {
            string[] outputLines = new string[outputCsvLineList.Count];
            for (int row = 0; row < outputCsvLineList.Count; row++) {
                var sb = new StringBuilder();
                for (int col = 0; col < outputCsvLineList[row].Count; col++) {
                    if (col == 0) {
                        sb.Append(outputCsvLineList[row][col]);
                    } else {
                        sb.Append(", ").Append(outputCsvLineList[row][col]);
                    }

                    outputLines[row] = sb.ToString();
                }
            }

            var writer = new StreamWriter(s);
            foreach (string line in outputLines) {
                writer.WriteLine(line);
            }

            writer.Dispose();
            writer.Close();
        }
    }
}