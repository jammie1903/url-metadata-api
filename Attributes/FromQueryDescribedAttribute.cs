using Microsoft.AspNetCore.Mvc;

namespace UrlMetadata.Attributes
{
    public class FromQueryDescribedAttribute : FromQueryAttribute, IDescribed
    {
        public string Description { get; set; }

        public FromQueryDescribedAttribute(string name, string description = null)
        {
            Name = name;
            Description = description;
        }
    }
}
