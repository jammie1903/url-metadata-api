using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using UrlMetadata.Attributes;
using UrlMetadata.Dtos.ApiDoc;
using UrlMetadata.ExtensionMethods;

namespace UrlMetadata.Services.Interfaces
{
    public interface IApiDocService
    {
        IEnumerable<EndpointDto> ReadController(Type controllerType);
        IEnumerable<EndpointDto> ReadControllers(params Type[] controllerTypes);
    }
}
