using System.Collections.Generic;
using UrlMetadata.Attributes;

namespace UrlMetadata.Dtos
{
    public class RawMetadataDto
    {
        [Description("All the meta tag entries with both `content` and `name/property` attributes")]
        public IDictionary<string, string> Meta { get; set; }

        [Description("All the link tag entries with both `href` and `rel` attributes")]
        public IDictionary<string, string> Links { get; set; }
    }
}
