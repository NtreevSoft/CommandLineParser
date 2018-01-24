using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public static class CommandStringUtility
    {
        private const string doubleQuotesPattern = "(?<![\\\\])[\"](?:.(?!(?<![\\\\])(?:(?<![\\\\])[\"])))*.?(?<![\\\\])[\"]";
        private const string singleQuotePattern = "(?<![\\\\])['](?:.(?!(?<![\\\\])(?:(?<![\\\\])['])))*.?(?<![\\\\])[']";
        private const string textPattern = "\\S+";
        //private const string pattern = "((?<!\")\"(?:\"(?=\")|(?<=\")\"|[^\"])+\"*(?=\\s*)|\\S+)";
        //private const string completionPattern = "((?<!\")\"(?:\"(?=\")|(?<=\")\"|[^\"])+\"*(?=\\s*)|\\S+|\\s+$)";

        private readonly static string fullPattern;
        private readonly static string completionPattern;

        static CommandStringUtility()
        {
            fullPattern = string.Format("({0}|{1}|{2}={0}|{2}={1}|{2}={2}|{2})", doubleQuotesPattern, singleQuotePattern, textPattern);
            completionPattern = string.Format("({0}|{1}|{2}|\\s+$)", doubleQuotesPattern, singleQuotePattern, textPattern);
        }

        public static string[] Split(string text)
        {
            var match = Regex.Match(text, fullPattern);
            var name = TrimQuot(match.Value);
            var arguments = text.Substring(match.Length).Trim();
            return new string[] { name, arguments, };
        }

        public static string[] SplitAll(string text)
        {
            return SplitAll(text, true);
        }

        public static string[] SplitAll(string text, bool removeQuote)
        {
            var matches = Regex.Matches(text, fullPattern);
            var argList = new List<string>();

            foreach (Match item in matches)
            {
                if (removeQuote == true)
                {
                    argList.Add(TrimQuot(item.Value));
                }
                else
                {
                    argList.Add(item.Value);
                }
            }

            return argList.ToArray();
        }

        /// <summary>
        /// a=1, a="123", a='123' 과 같은 문자열을 키와 값으로 분리하는 메소드
        /// </summary>
        public static bool TryGetKeyValue(string text, out string key, out string value)
        {
            var capturePattern = string.Format("((?<key>{2})=(?<value>{0})|(?<key>{2})=(?<value>{1})|(?<key>{2})=(?<value>{2}))", doubleQuotesPattern, singleQuotePattern, textPattern);
            var match = Regex.Match(text, capturePattern, RegexOptions.ExplicitCapture);
            if (match.Success)
            {
                key = match.Groups["key"].Value;
                value = match.Groups["value"].Value;
                return true;
            }
            key = null;
            value = null;
            return false;
        }

        public static Match[] MatchAll(string text)
        {
            var matches = Regex.Matches(text, fullPattern);
            var argList = new List<Match>();

            foreach (Match item in matches)
            {
                argList.Add(item);
            }

            return argList.ToArray();
        }

        public static Match[] MatchCompletion(string text)
        {
            var matches = Regex.Matches(text, completionPattern);
            var argList = new List<Match>();

            foreach (Match item in matches)
            {
                argList.Add(item);
            }

            return argList.ToArray();
        }

        public static string WrapSingleQuot(string text)
        {
            if (Regex.IsMatch(text, "^" + singleQuotePattern) == true || Regex.IsMatch(text, "^" + doubleQuotesPattern) == true)
                throw new ArgumentException(nameof(text));
            return string.Format("'{0}'", text);
        }

        public static string WrapDoubleQuote(string text)
        {
            if (Regex.IsMatch(text, "^" + singleQuotePattern) == true || Regex.IsMatch(text, "^" + doubleQuotesPattern) == true)
                throw new ArgumentException(nameof(text));
            return string.Format("\"{0}\"", text);
        }

        public static bool IsWrappedOfSingleQuot(string text)
        {
            return Regex.IsMatch(text, "^" + singleQuotePattern);
        }

        public static bool IsWrappedOfDoubleQuote(string text)
        {
            return Regex.IsMatch(text, "^" + doubleQuotesPattern);
        }

        public static bool IsWrappedOfQuote(string text)
        {
            return IsWrappedOfSingleQuot(text) || IsWrappedOfDoubleQuote(text);
        }

        public static string TrimQuot(string text)
        {
            if (IsWrappedOfSingleQuot(text) == true || IsWrappedOfDoubleQuote(text) == true)
            {
                text = text.Substring(1);
                text = text.Remove(text.Length - 1);
            }
            return text;
        }

        public static bool IsSwitch(string argument)
        {
            if (argument == null)
                return false;
            return Regex.IsMatch(argument, $"^{CommandSettings.Delimiter}{CommandSettings.SwitchPattern}|^{CommandSettings.ShortDelimiter}{CommandSettings.ShortSwitchPattern}");
        }

        public static IDictionary<string, object> ArgumentsToDictionary(string[] arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            var properties = new Dictionary<string, object>(arguments.Length);
            foreach (var item in arguments)
            {
                if (CommandStringUtility.TryGetKeyValue(item, out var key, out var value) == true)
                {
                    if (CommandStringUtility.IsWrappedOfQuote(value))
                    {
                        properties.Add(key, CommandStringUtility.TrimQuot(value));
                    }
                    else if (decimal.TryParse(value, out decimal l) == true)
                    {
                        properties.Add(key, l);
                    }
                    else if (bool.TryParse(value, out bool b) == true)
                    {
                        properties.Add(key, b);
                    }
                    else
                    {
                        properties.Add(key, value);
                    }
                }
                else
                {
                    throw new ArgumentException($"Invalid argument: '{item}'");
                }
            }
            return properties;
        }
    }
}
