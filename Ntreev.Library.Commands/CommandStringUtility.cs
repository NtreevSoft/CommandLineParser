//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ntreev.Library.Commands
{
    public static class CommandStringUtility
    {
        private const string doubleQuotesPattern = "(?<![\\\\])[\"](?:.(?!(?<![\\\\])(?:(?<![\\\\])[\"])))*.?(?<![\\\\])[\"]";
        private const string singleQuotePattern = "(?<![\\\\])['](?:.(?!(?<![\\\\])(?:(?<![\\\\])['])))*.?(?<![\\\\])[']";
        private const string textPattern = "\\S+";

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
            return new string[] { name, arguments };
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
            var capturePattern = string.Format("((?<key>{2})=(?<value>{0})|(?<key>{2})=(?<value>{1})|(?<key>{2})=(?<value>.+))", doubleQuotesPattern, singleQuotePattern, textPattern);
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
            return Regex.IsMatch(argument, $"^{CommandSettings.Delimiter}{CommandSettings.SwitchPattern}$|^{CommandSettings.ShortDelimiter}{CommandSettings.ShortSwitchPattern}$");
        }

        //private static string ToLiteral(string input)
        //{
        //    throw new NotImplementedException();
        //    //using (var writer = new StringWriter())
        //    //{
        //    //    using (var provider = CodeDomProvider.CreateProvider("CSharp"))
        //    //    {
        //    //        provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
        //    //        return writer.ToString();
        //    //    }
        //    //}
        //}

        public static string ToSpinalCase(string text)
        {
            return Regex.Replace(text, @"([a-z])([A-Z])", "$1-$2").ToLower();
        }

        public static string ToSpinalCase(Type type)
        {
            var name = Regex.Replace(type.Name, @"(Command)$", string.Empty);
            return Regex.Replace(name, @"([a-z])([A-Z])", "$1-$2").ToLower();
        }

        public static IDictionary<string, object> ArgumentsToDictionary(string[] arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            var properties = new Dictionary<string, object>(arguments.Length);
            foreach (var item in arguments)
            {
                var text = IsWrappedOfQuote(item) ? TrimQuot(item) : item;
                text = Regex.Unescape(text);

                if (CommandStringUtility.TryGetKeyValue(text, out var key, out var value) == true)
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
