using System.Collections.Generic;
using Newtonsoft.Json;
using UrlMetadata.Attributes;

namespace UrlMetadata.Dtos
{
    public class UrlMetadataDto
    {
        [Description("The endpoint title based upon the selected preference. `title`, `meta og:title` or `meta twitter:title`")]
        public string Title { get; set; }

        [Description("The domain of the endpoint based on the `meta twitter:domain` attribute")]
        public string Domain { get; set; }

        [Description("The name of the site based on the `meta og:site_name` attribute")]
        public string SiteName { get; set; }

        [Description("The endpoint description based upon the selected preference. `meta description`, `meta og:description` or `meta twitter:description`")]
        public string Description { get; set; }

        [Description("The author of the given endpoint based on the 'meta author' attribute")]
        public string Author { get; set; }

        [Description("The theme color of the given endpoint based on the 'meta theme-color' attribute")]
        public string ThemeColor { get; set; }

        [Description("The type of the endpoint based on the 'meta og:type' attribute")]
        public string Type { get; set; }

        [Description("The twitter card type of the endpoint based on the 'meta twitter:card' attribute")]
        public string Card { get; set; }

        [Description("The endpoint image based upon the selected preference. `meta og:image` or `meta twitter:image`")]
        public string Image { get; set; }

        [Description("the (potentially relative) url for the endpoints favicon based on the 'link icon' attribute")]
        public string Favicon { get; set; }

        [Description("Entries for twitter:label{x}/twitter:data{x} meta combinations")]
        public IEnumerable<AdditionalInformationDto> AdditionalInformation  { get; set; }
    }
}
