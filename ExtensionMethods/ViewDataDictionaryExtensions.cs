using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace UrlMetadata.ExtensionMethods
{
    public static class ViewDataDictionaryExtensions
    {
        public static ViewDataDictionary Extend(this ViewDataDictionary dictionary, string key, object value)
        {
            var returnValue = new ViewDataDictionary(dictionary);
            returnValue.Remove(key);
            returnValue[key] = value;
            return returnValue;
        }
    }
}
