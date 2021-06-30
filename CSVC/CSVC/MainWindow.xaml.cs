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
using System.Windows.Media.Effects;

namespace CSVC {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private List<Rule> RulesList { get; set; }
        private List<List<string>> CsvLineList { get; set; }
        private bool ConfigLoaded { get; set; }
        private bool CsvLoaded { get; set; }
        private ConfigParams Params { get; set; }

        public MainWindow() {
            InitializeComponent();
        }

        private void LoadConfigFileButton_OnClick(object sender, RoutedEventArgs e) {
            var configFileDialog = new OpenFileDialog {Filter = "Text files (*.txt)|*.txt"};
            if (configFileDialog.ShowDialog() == true) {
                Debug.WriteLine(configFileDialog.FileName);
                RulesList = new List<Rule>();
                try {
                    Params = ConfigParser.ParseConfigFile(File.ReadAllText(configFileDialog.FileName), RulesList);
                    RulesList.Sort();
                    Debug.WriteLine($"Columns: {Params.ReadColumns}");
                    RulesDataGrid.ItemsSource = RulesList;
                    foreach (var rule in RulesList) {
                        Debug.WriteLine(rule);
                    }
                } catch (RequireFailedException exception) {
                    MessageBox.Show(exception.Message);
                    return;
                }
            }

            ConfigInfoTextBlock.Text = $"Config Loaded: \"{configFileDialog.SafeFileName}\"\n{Params}";
            ConfigLoaded = true;
        }

        private void LoadCSVFileButton_OnClick(object sender, RoutedEventArgs e) {
            var csvFileDialog = new OpenFileDialog {Filter = "Comma Seperated Values (*.csv)|*.csv"};
            if (csvFileDialog.ShowDialog() != true) return;

            try {
                CsvLineList = Csv.ParseCsv(csvFileDialog.FileName);
            } catch (IOException) {
                MessageBox.Show("Error, file currently in use. Try closing the file elsewhere before " +
                                "opening");
            }

            CsvInfoTextBlock.Text = $"CSV Loaded: \"{csvFileDialog.SafeFileName}\"";
            CsvLoaded = true;
        }


        private void ConvertButton_OnClick(object sender, RoutedEventArgs e) {
            if (ConfigLoaded == false || CsvLoaded == false || CsvLineList == null || CsvLineList.Count == 0) {
                MessageBox.Show($"Error, ensure you have loaded all the appropriate files");
                return;
            }
            if (Params.ReadColumns.Any(col => col >= CsvLineList[0].Count || col < 0)) {
                MessageBox.Show(
                    $"Error, read column indices {Params.ReadColumns} out of bounds for CSV file with {CsvLineList[0].Count} columns.");
                return;
            }
            try {
                Params.LineWriter.CheckBounds(CsvLineList[0].Count);
            } catch (IndexOutOfRangeException exception) {
                MessageBox.Show(exception.Message);
                return;
            }


            var outputCsvLineList = Csv.MakeReplacements(Params, RulesList, CsvLineList);

            var saveFileDialog = new SaveFileDialog {Filter = "Comma Seperated Values (*.csv)|*.csv"};
            if (saveFileDialog.ShowDialog() != true) return;
            try {
                Csv.SaveToCsv(saveFileDialog.OpenFile(), outputCsvLineList);
                MessageBox.Show($"Saved to {saveFileDialog.FileName}");
            } catch (IOException) {
                MessageBox.Show("Error, file currently in use. Try closing the file elsewhere before " +
                                "saving");
            }
            
        }
    }
}