using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CSVC {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private List<Rule> RulesList { get; set; }
        private List<List<string>> CsvLineList { get; set; }

        private bool ConfigLoaded { get; set; }
        private bool CsvLoaded { get; set; }
        private int Column { get; set; }

        public MainWindow() {
            InitializeComponent();
        }

        private void LoadConfigFileButton_OnClick(object sender, RoutedEventArgs e) {
            var configFileDialog = new OpenFileDialog {DefaultExt = "csv"};
            if (configFileDialog.ShowDialog() == true) {
                Debug.WriteLine(configFileDialog.FileName);
                RulesList = new List<Rule>();
                try {
                    Column = ConfigParser.ParseConfigFile(File.ReadAllText(configFileDialog.FileName), RulesList);
                } catch (RequireFailedException exception) {
                    MessageBox.Show(exception.Message);
                }

                RulesList.Sort();
                Debug.WriteLine($"Column: {Column}");
                RulesDataGrid.ItemsSource = RulesList;
                foreach (var rule in RulesList) {
                    Debug.WriteLine(rule);
                }
                // GetLines(configFileDialog.FileName);
            }

            ConfigInfoTextBlock.Text = $"Config Loaded: \"{configFileDialog.FileName}\"";
            ConfigLoaded = true;
        }

        private void LoadCSVFileButton_OnClick(object sender, RoutedEventArgs e) {
            var csvFileDialog = new OpenFileDialog();
            if (csvFileDialog.ShowDialog() == true) {
                CsvLineList = new List<List<string>>();
                foreach (var line in File.ReadAllLines(csvFileDialog.FileName)) {
                    var columnList = line.Split(",").ToList();
                    CsvLineList.Add(columnList);
                }

            }

            CsvInfoTextBlock.Text = $"CSV Loaded: \"{csvFileDialog.FileName}\"";
            CsvLoaded = true;
        }

        private void MakeReplacements() {
            foreach (var line in CsvLineList) {
                foreach (var rule in RulesList.Where(rule => rule.Applies(line[Column]))) {
                    line[Column] = rule.GetSubstitution();
                }
            }
        }

        private void save_to_CSV() {
            var saveFileDialog = new SaveFileDialog {DefaultExt = "csv"};

            if (saveFileDialog.ShowDialog() != true) return;
            var outputLines = new string[CsvLineList.Count];
            for (var row = 0; row < CsvLineList.Count; row++) {
                var sb = new StringBuilder();
                for (var col = 0; col < CsvLineList[row].Count; col++) {
                    if (col == 0) {
                        sb.Append(CsvLineList[row][col]);
                    } else {
                        sb.Append(", ").Append(CsvLineList[row][col]);
                    }

                    outputLines[row] = sb.ToString();
                }
            }

            var writer = new StreamWriter(saveFileDialog.OpenFile());
            foreach (var line in outputLines) {
                writer.WriteLine(line);
            }

            writer.Dispose();
            writer.Close();
        }

        private void ConvertButton_OnClick(object sender, RoutedEventArgs e) {
            if (ConfigLoaded == false || CsvLoaded == false || CsvLineList == null || CsvLineList.Count == 0) {
                MessageBox.Show($"Error, ensure you have loaded all the appropriate files");
                return;
            }
            if (Column >= CsvLineList[0].Count || Column < 0) {
                MessageBox.Show($"Error, Column index {Column} out of bounds for CSV file with {CsvLineList[0].Count} columns.");
                return;
            }


            MakeReplacements();

            save_to_CSV();
        }
    }
}