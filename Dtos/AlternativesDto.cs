using Newtonsoft.Json;

namespace UrlMetadata.Dtos
{
    public class AlternativesDto { 

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Generic { get; set; }

        [JsonProperty(PropertyName = "og", NullValueHandling = NullValueHandling.Ignore)]
        public string OpenGraph { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Twitter { get; set; }
    }
}
