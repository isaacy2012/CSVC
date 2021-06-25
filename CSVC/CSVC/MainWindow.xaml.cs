﻿using System;
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
                long column = ConfigParser.ParseConfigFile(File.ReadAllText(configFileDialog.FileName), rulesList);
                Debug.WriteLine($"Column: {column}");
                foreach (var rule in rulesList)
                {
                    Debug.WriteLine(rule);
                }
                // GetLines(configFileDialog.FileName);
            }
        }
    }
}