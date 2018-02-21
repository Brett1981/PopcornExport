using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PopcornExport.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<string> GetMatches(this string text, string startString, string endString)
        {
            List<string> matched = new List<string>();
            int indexStart = 0, indexEnd = 0;
            bool exit = false;
            while (!exit)
            {
                indexStart = text.IndexOf(startString);
                indexEnd = text.IndexOf(endString);
                if (indexStart != -1 && indexEnd != -1)
                {
                    matched.Add(text.Substring(indexStart + startString.Length,
                        indexEnd - indexStart - startString.Length));
                    text = text.Substring(indexEnd + endString.Length);
                }
                else
                    exit = true;
            }
            return matched;
        }
    }
}
