using Microsoft.AspNetCore.Mvc;

namespace UrlMetadata.Attributes
{
    public class RouteDescribedAttribute : RouteAttribute, IDescribed
    {
        public string Description { get; set; }

        public RouteDescribedAttribute(string template, string description = null) : base(template)
        {
            Description = description;
        }
    }
}
