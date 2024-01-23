using Microsoft.AspNetCore.Mvc;
using Shortly.Client.Data.Models;
using Shortly.Client.Data.ViewModels;

namespace Shortly.Client.Controllers
{
    public class UrlController : Controller
    {
        public IActionResult Index()
        {
            //Fake Db Data
            var allUrls = new List<GetUrlVM>()
            {
                new GetUrlVM()
                {
                    Id = 1,
                    OriginalLink = "https://link1.com",
                    ShortLink = "sh1",
                    NrOfClicks = 1,
                    UserId = 1,
                },
                new GetUrlVM()
                {
                    Id = 2,
                    OriginalLink = "https://link2.com",
                    ShortLink = "sh2",
                    NrOfClicks = 2,
                    UserId = 2,
                },
                new GetUrlVM()
                {
                    Id = 3,
                    OriginalLink = "https://link3.com",
                    ShortLink = "sh3",
                    NrOfClicks = 3,
                    UserId = 3,
                }
            };

            return View(allUrls);
        }

        public IActionResult Create()
        {
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            return View();
        }

        public IActionResult Remove(int userId, int linkId)
        {
            return View();
        }
    }
}
