using Microsoft.AspNetCore.Mvc;
using Shortly.Client.Data.Models;

namespace Shortly.Client.Controllers
{
    public class UrlController : Controller
    {
        public IActionResult Index()
        {
            ViewData["ShortenedUrl"] = "This is just a short url";
            ViewData["AllUrls"] = new List<string>() { "Url 1", "Url 2", "Url 3"};

            return View();
        }
    }
}
