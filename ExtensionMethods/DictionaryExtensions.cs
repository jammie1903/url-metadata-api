using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UrlMetadata.ExtensionMethods
{
    public static class DictionaryExtensions
    {
        public static ViewDataDictionary Extend(this ViewDataDictionary dictionary, string key, object value)
        {
            var returnValue = new ViewDataDictionary(dictionary);
            returnValue.Remove(key);
            returnValue[key] = value;
            return returnValue;
        }

        public static void ForceAdd<TValue>(this IDictionary<string, TValue> dictionary, string key, TValue value)
        {
            var index = 0;
            while (dictionary.ContainsKey(key + (index > 0 ? index.ToString() : ""))) index++;
            dictionary[key + (index > 0 ? index.ToString() : "")] = value;
        }
    }
}
