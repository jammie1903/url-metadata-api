using System.Collections.Generic;

namespace UrlMetadata.Dtos.ApiDoc
{
    public class TypeDto
    {
        public string Name { get; set; }
        public FieldDto[] Fields { get; set; }
    }
}
