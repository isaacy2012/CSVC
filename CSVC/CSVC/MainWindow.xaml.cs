using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Printing;
using System.Text;

namespace CSVC
{


    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var configFileDialog = new OpenFileDialog();
            if (configFileDialog.ShowDialog() == true)
            {
                Debug.WriteLine(configFileDialog.FileName);
                var rulesList = new List<Rule>();
                ConfigParser.ParseConfigFile(File.ReadAllText(configFileDialog.FileName), rulesList);
                foreach (var rule in rulesList)
                {
                    Debug.WriteLine(rule);
                }
                // GetLines(configFileDialog.FileName);
                    
            }
        }

        public void GetLines(string filename)
        {
                foreach (var x in File.ReadAllLines(filename, Encoding.UTF8))
                {
                    Debug.WriteLine("LINES: " + x);
                }
        }
    }
}