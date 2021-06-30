using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CSVC;

namespace CSVC {
    public static class ConfigParser {
        private const string Openbrace = "{";
        private const string Closebrace = "}";
        private const string Semicolon = ";";
        private const string Comma = ",";
        private const string Category = "category";
        private const string Column = "column";
        private const string NoHeader = "noheader";
        private const string Append = "append";
        private const string Insert = "insert";
        private const string Replace = "replace";
        private const string Contains = "contains";
        private const string Matches = "matches";
        private static readonly Regex RuleType = new("equals|is|contains|matches");
        private static readonly Regex IsEquals = new Regex("equals|is");
        private static readonly Regex Quoted = new("\".*\"");

        private delegate Rule RuleSupplier(string match, string substitution);

        /// <summary>
        /// Parse the Config File
        /// </summary>
        /// <param name="configFileString">The entire contents of the config file as a string</param>
        /// <param name="list">The list of rules to add to</param>
        /// <returns>The column of interest</returns>
        public static ConfigParams ParseConfigFile(string configFileString, List<Rule> list) {
            string[] arr = Regex.Split(configFileString, "(\".*?\")|\\s+|(?=[{}(),;])|(?<=[{}(),;])")
                .Where(str => str != String.Empty).ToArray();
            var s = new Scanner(arr);

            var configParams = ParseConfigParams(s);

            while (s.ConsumeIf(Category)) {
                ParseCategory(s, list);
            }

            return configParams;
        }

        /// <summary>
        /// Parse Config Parameters
        /// </summary>
        /// <param name="s">Scanner</param>
        /// <returns>The ConfigParams</returns>
        private static ConfigParams ParseConfigParams(Scanner s) {
            bool header = !s.ConsumeIf(NoHeader);
            s.ConsumeIf(Semicolon);
            s.Require(Column);
            var readColumns = ParseList(s);
            s.ConsumeIf(Semicolon);
            var lineWriter = ParseLineWriter(s);
            return new ConfigParams(lineWriter, readColumns, header);
        }

        /// <summary>
        /// Parse Line Writer
        /// </summary>
        /// <param name="s"></param>
        /// <returns>ILineWriter</returns>
        private static ILineWriter ParseLineWriter(Scanner s) {
            if (s.ConsumeIf(Insert)) {
                int writeColumn = Convert.ToInt32(s.Next());
                string columnHeader = string.Empty;
                if (s.HasNext(Quoted)) {
                    columnHeader = StripQuotes(s.Next());
                }
                s.ConsumeIf(Semicolon);
                return new Insert(writeColumn, columnHeader);
            }
            if (s.ConsumeIf(Replace)) {
                int writeColumn = Convert.ToInt32(s.Next());
                s.ConsumeIf(Semicolon);
                return new Replace(writeColumn);
            }
            s.ConsumeIf(Append);
            s.ConsumeIf(Semicolon);
            return new Append();
        }

        /// <summary>
        /// Parse List
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static List<int> ParseList(Scanner s) {
            var ret = new List<int> {Convert.ToInt32(s.Next())};
            while (s.ConsumeIf(Comma)) {
                ret.Add(Convert.ToInt32(s.Next()));
            }
            return ret;
        }

        /// <summary>
        /// Parse a category of rules
        /// </summary>
        /// <param name="s">The scanner</param>
        /// <param name="list">The list to add to</param>
        private static void ParseCategory(Scanner s, List<Rule> list) {
            s.RequireNext(Quoted);
            string categoryName = StripQuotes(s.Next());
            s.Require(Openbrace);
            while (s.HasNext(RuleType)) {
                if (s.ConsumeIf(IsEquals)) {
                    ParseRuleSet(categoryName, s, list, (x, y) => new EqualsRule(x, y));
                } else if (s.ConsumeIf(Contains)) {
                    ParseRuleSet(categoryName, s, list, (x, y) => new ContainsRule(x, y));
                } else if (s.ConsumeIf(Matches)) {
                    ParseRuleSet(categoryName, s, list, (x, y) => new MatchesRule(x, y));
                }
            }

            s.Require(Closebrace);
        }


        /// <summary>
        /// Parse Contains Rule
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="s"></param>
        /// <param name="list"></param>
        /// <param name="ruleSupplier">Rule Supplier</param>
        private static void ParseRuleSet(string categoryName, Scanner s, List<Rule> list, RuleSupplier ruleSupplier) {
            s.Require(Openbrace);
            while (s.HasNext(Closebrace) == false) {
                s.RequireNext(Quoted);
                var rule = ruleSupplier(StripQuotes(s.Next()), categoryName);
                s.ConsumeIf(Semicolon);
                list.Add(rule);
            }
            
            s.Require(Closebrace);
        }


        private static string StripQuotes(string str) {
            return str.Substring(1, str.Length - 2);
        }
    }
}