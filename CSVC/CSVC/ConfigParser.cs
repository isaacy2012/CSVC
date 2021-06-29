using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace CSVC {
    public static class ConfigParser {
        private const string Openbrace = "{";
        private const string Closebrace = "}";
        private const string Semicolon = ";";
        private const string Category = "category";
        private const string Column = "column";
        private static readonly Regex RuleType = new("equals|contains");

        /// <summary>
        /// Parse the Config File
        /// </summary>
        /// <param name="configFileString">The entire contents of the config file as a string</param>
        /// <param name="list">The list of rules to add to</param>
        /// <returns>The column of interest</returns>
        public static int ParseConfigFile(string configFileString, List<Rule> list) {
            int column = 0;
            var arr = Regex.Split(configFileString, "(\".*?\")|\\s+|(?=[{}(),;])|(?<=[{}(),;])")
                .Where(str => str != String.Empty).ToArray();
            var s = new Scanner(arr);
            if (s.ConsumeIf(Column)) {
                column = Convert.ToInt32(s.Next());
                s.Require(Semicolon);
            }

            while (s.ConsumeIf(Category)) {
                ParseCategory(s, list);
            }

            return column;
        }

        /// <summary>
        /// Parse a category of rules
        /// </summary>
        /// <param name="s">The scanner</param>
        /// <param name="list">The list to add to</param>
        private static void ParseCategory(Scanner s, List<Rule> list) {
            var categoryName = s.Next();
            s.Require(Openbrace);
            while (s.HasNext(RuleType)) {
                if (s.ConsumeIf("equals")) {
                    ParseEquals(categoryName, s, list);
                }
                else if (s.ConsumeIf("contains")) {
                    ParseContains(categoryName, s, list);
                }
            }

            s.Require(Closebrace);
        }

        private static void ParseContains(string categoryName, Scanner s, List<Rule> list) {
            s.Require(Openbrace);
            while (s.HasNext(Closebrace) == false) {
                var equals = new ContainsRule(StripQuotes(s.Next()), categoryName);
                s.Require(Semicolon);
                list.Add(equals);
            }

            s.Require(Closebrace);
        }

        private static void ParseEquals(string categoryName, Scanner s, List<Rule> list) {
            s.Require(Openbrace);
            while (s.HasNext(Closebrace) == false) {
                var equals = new EqualsRule(StripQuotes(s.Next()), categoryName);
                s.Require(Semicolon);
                list.Add(equals);
            }

            s.Require(Closebrace);
        }

        private static string StripQuotes(string str) {
            return str.Substring(1, str.Length - 2);
        }
    }
}