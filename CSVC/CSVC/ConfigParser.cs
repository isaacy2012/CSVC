using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace CSVC
{
    public class RequireFailedException : SystemException
    {
        private readonly string[] strings;
        private readonly int i;
        private readonly string pattern;

        public RequireFailedException(string[] strings, int i, Regex pattern)
        {
            this.strings = strings;
            this.i = i;
            this.pattern = pattern.ToString();
        }

        public RequireFailedException(string[] strings, int i, string pattern)
        {
            this.strings = strings;
            this.i = i;
            this.pattern = pattern;
        }

        public override string ToString()
        {
            return $"Failed to match {strings[i]} and {pattern} on {i}";
        }
    }

    public class Scanner
    {
        private int _i = -1;
        private string[] strings;

        public Scanner(string[] strings)
        {
            this.strings = strings;
        }

        public string Next()
        {
            return strings[++_i];
        }

        public bool HasNext(Regex pattern)
        {
            if (HasNext() == false)
            {
                return false;
            }

            return pattern.IsMatch(strings[_i + 1]);
        }

        public bool HasNext(String patternString)
        {
            if (HasNext() == false)
            {
                return false;
            }

            return patternString.Equals(strings[_i + 1]);
        }

        public bool HasNext()
        {
            return _i != strings.Length - 1;
        }

        public void Require(Regex pattern)
        {
            if (HasNext(pattern) == false)
            {
                throw new RequireFailedException(strings, _i + 1, pattern);
            }

            _i++;
        }

        public void Require(string patternString)
        {
            if (HasNext(patternString) == false)
            {
                throw new RequireFailedException(strings, _i + 1, patternString);
            }

            _i++;
        }
        
        public bool ConsumeIf(Regex pattern)
        {
            if (HasNext(pattern))
            {
                _i++;
                return true;
            }
            return false;
        }

        public bool ConsumeIf(string patternString)
        {
            if (HasNext(patternString))
            {
                _i++;
                return true;
            }
            return false;
        }
    }

    public static class ConfigParser
    {
        private const string Openbrace = "{";
        private const string Closebrace = "}";
        private const string Semicolon = ";";
        private const string Category = "category";
        private const string Column = "column";
        private static readonly Regex RuleType = new("equals|contains");

        public static long ParseConfigFile(string configFileString, List<Rule> list)
        {
            long column = 0;
            var arr = Regex.Split(configFileString, "(\".*?\")|\\s+|(?=[{}(),;])|(?<=[{}(),;])")
                .Where(str => str != String.Empty).ToArray();
            var s = new Scanner(arr);
            if (s.ConsumeIf(Column))
            {
                column = Convert.ToInt64(s.Next());
                s.Require(Semicolon);
            }
            while (s.ConsumeIf(Category))
            {
                ParseCategory(s, list);
            }

            return column;
        }

        private static void ParseCategory(Scanner s, List<Rule> list)
        {
            var categoryName = s.Next();
            s.Require(Openbrace);
            while (s.HasNext(RuleType))
            {
                if (s.ConsumeIf("equals"))
                {
                    ParseEquals(categoryName, s, list);
                }
                else if (s.ConsumeIf("contains"))
                {
                    ParseContains(categoryName, s, list);
                }
            }
            s.Require(Closebrace);
        }

        private static void ParseContains(string categoryName, Scanner s, List<Rule> list)
        {
            s.Require(Openbrace);
            while (s.HasNext(Closebrace) == false)
            {
                var equals = new ContainsRule(StripQuotes(s.Next()), categoryName);
                s.Require(Semicolon);
                list.Add(equals);
            }
            s.Require(Closebrace);
        }

        private static void ParseEquals(string categoryName, Scanner s, List<Rule> list)
        {
            s.Require(Openbrace);
            while (s.HasNext(Closebrace) == false)
            {
                var equals = new EqualsRule(StripQuotes(s.Next()), categoryName);
                s.Require(Semicolon);
                list.Add(equals);
            }
            s.Require(Closebrace);
        }

        private static string StripQuotes(string str)
        {
            return str.Substring(1, str.Length - 2);
        }
    }
}