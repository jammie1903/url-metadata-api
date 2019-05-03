using System;
using Microsoft.AspNetCore.Mvc;

namespace UrlMetadata.Attributes
{
    public class DescriptionAttribute : Attribute, IDescribed
    {
        public string Description { get; }

        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
