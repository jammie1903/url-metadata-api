using System;

using UrlMetadata.Attributes;

namespace UrlMetadata.Dtos
{
    public class PingResponseDto
    {
        [Description("The server time at the time of the request")]
        public DateTime DateTime { get; set; }
    }
}
