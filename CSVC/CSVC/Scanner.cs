using System.Text.RegularExpressions;

namespace CSVC
{
    public class Scanner
    {
        private int _i = -1;
        private readonly string[] strings;

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

        public bool HasNext(string patternString)
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
}