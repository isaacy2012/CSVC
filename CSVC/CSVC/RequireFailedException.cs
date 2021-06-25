using System;
using System.Text.RegularExpressions;

namespace CSVC
{
    public class RequireFailedException : SystemException
    {
        private readonly string[] _strings;
        private readonly int _i;
        private readonly string _patternString;

        /// <summary>
        /// Constructor for RequireFailedException
        /// </summary>
        /// <param name="strings">the complete scanner strings</param>
        /// <param name="i">the current scanner i</param>
        /// <param name="pattern">the pattern regex that failed to match</param>
        public RequireFailedException(string[] strings, int i, Regex pattern)
        {
            _strings = strings;
            _i = i;
            _patternString = pattern.ToString();
        }

        /// <summary>
        /// Constructor for RequireFailedException
        /// </summary>
        /// <param name="strings">the complete scanner strings</param>
        /// <param name="i">the current scanner i</param>
        /// <param name="patternString">the pattern string that failed to match</param>
        public RequireFailedException(string[] strings, int i, string patternString)
        {
            _strings = strings;
            _i = i;
            _patternString = patternString;
        }

        public override string ToString()
        {
            return $"Failed to match {_strings[_i]} and {_patternString} on {_i}";
        }
    }
}