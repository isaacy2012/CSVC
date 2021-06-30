using System.Text.RegularExpressions;

namespace CSVC {
    public class Scanner {
        private int _i = -1;
        private readonly string[] _strings;

        public Scanner(string[] strings) {
            this._strings = strings;
        }

        public string Next() {
            return _strings[++_i];
        }

        public bool HasNext(Regex pattern) {
            if (HasNext() == false) {
                return false;
            }

            return pattern.IsMatch(_strings[_i + 1]);
        }

        public bool HasNext(string patternString) {
            if (HasNext() == false) {
                return false;
            }

            return patternString.Equals(_strings[_i + 1]);
        }

        public bool HasNext() {
            return _i != _strings.Length - 1;
        }

        public void Require(Regex pattern) {
            RequireNext(pattern);
            _i++;
        }

        public void RequireNext(Regex pattern) {
            if (HasNext(pattern) == false) {
                throw new RequireFailedException(_strings, _i + 1, pattern);
            }
        }

        public void Require(string patternString) {
            RequireNext(patternString);
            _i++;
        }

        public void RequireNext(string patternString) {
            if (HasNext(patternString) == false) {
                throw new RequireFailedException(_strings, _i + 1, patternString);
            }
        }

        public bool ConsumeIf(Regex pattern) {
            if (HasNext(pattern)) {
                _i++;
                return true;
            }
            return false;
        }

        public bool ConsumeIf(string patternString) {
            if (HasNext(patternString)) {
                _i++;
                return true;
            }
            return false;
        }
    }
}