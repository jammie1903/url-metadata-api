using Microsoft.AspNetCore.Mvc;

namespace UrlMetadata.Controllers
{
    [Route("/")]
    [Controller]
    public class MainController : Controller
    {
        public ActionResult Index()
        {
            return View(new {});
        }
    }
}
