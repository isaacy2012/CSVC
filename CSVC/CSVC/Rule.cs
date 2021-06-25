using System.Transactions;

namespace CSVC
{
    public abstract class Rule
    {
        protected readonly string match;
        protected readonly string substitution;

        protected Rule(string match, string substitution)
        {
            this.match = match;
            this.substitution = substitution;
        }

        public string GetSubstitution()
        {
            return substitution;
        }

        public abstract bool Applies(string input);

        public override string ToString()
        {
            return $"{match} -> {substitution}";
        }
    }

    public class EqualsRule : Rule
    {
        public EqualsRule(string match, string substitution) : base(match, substitution)
        {
        }

        public override bool Applies(string input)
        {
            return input.Equals(match);
        }

        public override string ToString()
        {
            return $"equals: {base.ToString()}";
        }
    }
    
    public class ContainsRule : Rule
    {
        public ContainsRule(string match, string substitution) : base(match, substitution)
        {
        }

        public override bool Applies(string input)
        {
            return input.Contains(match);
        }
        
        public override string ToString()
        {
            return $"equals: {base.ToString()}";
        }
    }
}