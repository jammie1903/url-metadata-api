using System.Collections.Generic;

namespace UrlMetadata.Dtos.ApiDoc
{
    public class EndpointDto
    {
        public string Name { get; set; }
        public string Route { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Methods { get; set; }
        public TypeDto ReturnType { get; set; }
        public IEnumerable<QueryParameterDto> QueryParams { get; set; }
    }
}
