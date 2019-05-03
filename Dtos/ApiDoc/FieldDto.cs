using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlMetadata.Dtos.ApiDoc
{
    public class FieldDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TypeDto Type { get; set; }
    }
}
