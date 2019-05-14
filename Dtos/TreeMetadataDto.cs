using System.Collections.Generic;
using UrlMetadata.Attributes;

namespace UrlMetadata.Dtos
{
    public class TreeMetadataDto
    {
        [Description("The title tag of the url")]
        public string Title { get; set; }

        [Description("All the meta tag entries with both `content` and `name/property` attributes formatted in to a tree structure")]
        public object Meta { get; set; }

        [Description("All the link tag entries with both `href` and `rel` attributes formatted in to a tree structure")]
        public object Links { get; set; }
    }
}
