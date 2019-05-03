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
using UrlMetadata.Services.Interfaces;

namespace UrlMetadata.Controllers
{
    [Route("/apidoc")]
    [Controller]
    public class ApiDocController : Controller
    {
        private readonly IApiDocService _service;

        public ApiDocController(IApiDocService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View(_service.ReadController(typeof(MetadataController)));
        }

    }
}