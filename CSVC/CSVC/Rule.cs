using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSVC {
    public enum RuleType {
        EqualsRule,
        ContainsRule,
        MatchesRule
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    public abstract class Rule : IComparable<Rule> {
        public RuleType Type { get; }
        public string Match { get; }
        public string Substitution { get; }

        protected Rule(string match, string substitution, RuleType type) {
            Match = match;
            Substitution = substitution;
            Type = type;
        }

        public abstract bool Applies(string input);

        public bool Applies(List<string> input, List<int> indices) {
            return indices.Any(index => Applies(input[index]));
        }

        public int CompareTo(Rule other) {
            return Type.Equals(other.Type) ? other.Match.Length.CompareTo(Match.Length) : Type.CompareTo(other.Type);
        }

        public override string ToString() {
            return $"{Match} -> {Substitution}";
        }
    }

    public class EqualsRule : Rule {
        public EqualsRule(string match, string substitution) : base(match, substitution, RuleType.EqualsRule) {
        }

        public override bool Applies(string input) {
            return input.Equals(Match);
        }

        public override string ToString() {
            return $"equals: {base.ToString()}";
        }
    }

    public class ContainsRule : Rule {
        public ContainsRule(string match, string substitution) : base(match, substitution, RuleType.ContainsRule) {
        }

        public override bool Applies(string input) {
            return input.Contains(Match);
        }

        public override string ToString() {
            return $"contains: {base.ToString()}";
        }
    }

    public class MatchesRule : Rule {
        private readonly Regex _matcher;
        
        public MatchesRule(string match, string substitution) : base(match, substitution, RuleType.MatchesRule) {
            _matcher = new Regex(match);
        }
        
        public override bool Applies(string input) {
            return _matcher.IsMatch(input);
        }

        public override string ToString() {
            return $"matches: {base.ToString()}";
        }
        
    }
}