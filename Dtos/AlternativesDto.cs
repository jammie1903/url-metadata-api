using System.ComponentModel;
using Newtonsoft.Json;

namespace UrlMetadata.Dtos
{
    public class AlternativesDto { 

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Description("The generic version of the given metadata type")]
        public string Generic { get; set; }

        [JsonProperty(PropertyName = "og", NullValueHandling = NullValueHandling.Ignore)]
        [Description("The OpenGraph version of the given metadata type")]
        public string OpenGraph { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Description("The Twitter version of the given metadata type")]
        public string Twitter { get; set; }
    }
}
