using Microsoft.AspNetCore.Mvc;

namespace UrlMetadata.Controllers
{
    [Route("/demo")]
    [Controller]
    public class DemoController : Controller
    {
        public ActionResult Index()
        {
            return View(new {});
        }
    }
}
