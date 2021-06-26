namespace CSVC
{
    public abstract class Rule
    {
        public string Type => GetType().Name.Replace("Rule", "");
        public string Match { get; set; }
        public string Substitution { get; set; }


        protected Rule(string match, string substitution)
        {
            Match = match;
            Substitution = substitution;
        }

        public string GetSubstitution()
        {
            return Substitution;
        }

        public abstract bool Applies(string input);

        public override string ToString()
        {
            return $"{Match} -> {Substitution}";
        }
    }

    public class EqualsRule : Rule
    {
        public EqualsRule(string match, string substitution) : base(match, substitution)
        {
        }

        public override bool Applies(string input)
        {
            return input.Equals(Match);
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
            return input.Contains(Match);
        }

        public override string ToString()
        {
            return $"equals: {base.ToString()}";
        }
    }
}