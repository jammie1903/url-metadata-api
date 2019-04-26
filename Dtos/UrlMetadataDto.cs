using System.Collections.Generic;
using Newtonsoft.Json;

namespace UrlMetadata.Dtos
{
    public class UrlMetadataDto
    {
        public string Title { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AlternativesDto Titles { get; set; }
        public string Domain { get; set; }
        public string SiteName { get; set; }
        public string Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AlternativesDto Descriptions { get; set; }
        public string Author { get; set; }
        public string ThemeColor { get; set; }
        public string Type { get; set; }
        public string Card { get; set; }
        public string Image { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AlternativesDto Images { get; set; }
        public string Favicon { get; set; }
        public IEnumerable<AdditionalInformationDto> AdditionalInformation  { get; set; }
    }
}
