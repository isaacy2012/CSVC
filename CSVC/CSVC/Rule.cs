using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CSVC {

    public enum RuleType {
        EqualsRule,
        ContainsRule
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
        
        public string GetSubstitution() {
            return Substitution;
        }

        public abstract bool Applies(string input);

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
            return $"equals: {base.ToString()}";
        }
    }
}