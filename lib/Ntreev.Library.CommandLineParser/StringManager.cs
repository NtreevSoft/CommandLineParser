using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Library
{
    static class StringManager
    {
        public static string ToSpinalCase(this string text)
        {
            return Regex.Replace(text, @"([a-z])([A-Z])", "$1-$2").ToLower();
        }
    }
}
