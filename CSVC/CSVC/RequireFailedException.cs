using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace CSVC
{
    public class RequireFailedException : SystemException
    {
        private readonly string[] _strings;

        static string GenerateErrorMessage(string[] strings, int i, string pattern) {
            return $"Error at token {i}, Expected \"{pattern}\" got \"{strings[i]}\"";
        }

        /// <summary>
        /// Constructor for RequireFailedException
        /// </summary>
        /// <param name="strings">the complete scanner strings</param>
        /// <param name="i">the current scanner i</param>
        /// <param name="pattern">the pattern regex that failed to match</param>
        public RequireFailedException(string[] strings, int i, Regex pattern) : base(GenerateErrorMessage(strings, i, pattern.ToString()))
        {
            _strings = strings;
        }

        /// <summary>
        /// Constructor for RequireFailedException
        /// </summary>
        /// <param name="strings">the complete scanner strings</param>
        /// <param name="i">the current scanner i</param>
        /// <param name="patternString">the pattern string that failed to match</param>
        public RequireFailedException(string[] strings, int i, string patternString) : base(GenerateErrorMessage(strings, i, patternString))
        {
            _strings = strings;
        }

    }
}