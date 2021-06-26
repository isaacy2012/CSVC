using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CSVC {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private List<Rule> rulesList { get; set; }
        private List<List<String>> csvLineList { get; set; }

        private bool ConfigLoaded { get; set; }
        private bool CsvLoaded { get; set; }
        private int Column { get; set; }

        public MainWindow() {
            InitializeComponent();
        }

        private void LoadConfigFileButton_OnClick(object sender, RoutedEventArgs e) {
            var configFileDialog = new OpenFileDialog();
            configFileDialog.DefaultExt = "csv";
            if (configFileDialog.ShowDialog() == true) {
                Debug.WriteLine(configFileDialog.FileName);
                rulesList = new List<Rule>();
                Column = ConfigParser.ParseConfigFile(File.ReadAllText(configFileDialog.FileName), rulesList);
                Debug.WriteLine($"Column: {Column}");
                RulesDataGrid.ItemsSource = rulesList;
                foreach (var rule in rulesList) {
                    Debug.WriteLine(rule);
                }
                // GetLines(configFileDialog.FileName);
            }

            ConfigLoaded = true;
        }

        private void LoadCSVFileButton_OnClick(object sender, RoutedEventArgs e) {
            var csvFileDialog = new OpenFileDialog();
            if (csvFileDialog.ShowDialog() == true) {
                csvLineList = new List<List<String>>();
                foreach (var line in File.ReadAllLines(csvFileDialog.FileName)) {
                    var columnList = new List<String>();
                    foreach (var col in line.Split(",")) {
                        columnList.Add(col);
                    }

                    csvLineList.Add(columnList);
                }
            }

            CsvLoaded = true;
        }

        private void makeReplacements() {
            foreach (var line in csvLineList) {
                foreach (var rule in rulesList) {
                    if (rule.Applies(line[Column])) {
                        line[Column] = rule.GetSubstitution();
                    }
                }
            }
        }

        private void save_to_CSV() {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "csv";
            
            if (saveFileDialog.ShowDialog() == true) {
                var outputLines = new string[csvLineList.Count];
                for (int row = 0; row < csvLineList.Count; row++) {
                    var sb = new StringBuilder();
                    for (int col = 0; col < csvLineList[row].Count; col++) {
                        if (col == 0) {
                            sb.Append(csvLineList[row][col]);
                        } else {
                            sb.Append(", ").Append(csvLineList[row][col]);
                        }
                        outputLines[row] = sb.ToString();
                    }
                }

                StreamWriter writer = new StreamWriter(saveFileDialog.OpenFile());
                foreach (var line in outputLines) {

                    writer.WriteLine(line);

                }
                writer.Dispose();
                writer.Close();
            }
        }

        private void ConvertButton_OnClick(object sender, RoutedEventArgs e) {
            if (ConfigLoaded == false || CsvLoaded == false || csvLineList == null) {
                return;
            }

            makeReplacements();

            save_to_CSV();
        }
    }
}