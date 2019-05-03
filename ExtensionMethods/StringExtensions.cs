using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using UrlMetadata.Dtos;
using UrlMetadata.Enums;

namespace UrlMetadata.ExtensionMethods
{
    public static class StringExtensions
    {
        public static string Or(this string str, string alternative)
        {
            return string.IsNullOrWhiteSpace(str) ? alternative : str;
        }

        public static string UrlCombine(this string str, string str2)
        {
            return (str ?? "").Trim().TrimEnd('/') + '/' + (str2 ?? "").Trim().TrimStart('/');
        }
        public static string RegexReplace(this string str, string regex, string replacement)
        {
            var reg = new Regex(regex);
            return reg.Replace(str ?? "", replacement);
        }
    }
}