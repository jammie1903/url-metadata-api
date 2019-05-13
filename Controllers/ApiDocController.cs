using Microsoft.AspNetCore.Mvc;
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