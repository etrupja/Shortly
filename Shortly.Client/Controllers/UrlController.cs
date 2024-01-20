using Microsoft.AspNetCore.Mvc;

namespace Shortly.Client.Controllers
{
    public class UrlController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
