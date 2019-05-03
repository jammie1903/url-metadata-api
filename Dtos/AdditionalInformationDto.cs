using System.ComponentModel;

namespace UrlMetadata.Dtos
{
    public class AdditionalInformationDto
    {
        [Description("The label for the given label/data pair")]
        public string Label { get; set; }

        [Description("The data for the given label/data pair")]
        public string Data { get; set; }
    }
}
