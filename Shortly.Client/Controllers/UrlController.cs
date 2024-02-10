using Microsoft.AspNetCore.Mvc;
using Shortly.Client.Data.ViewModels;
using Shortly.Data;

namespace Shortly.Client.Controllers
{
    public class UrlController : Controller
    {
        private AppDbContext _context;
        public UrlController(AppDbContext context) 
        { 
            _context = context;
        }

        public IActionResult Index()
        {
            var allUrls = _context
                .Urls
                .Select(url => new GetUrlVM()
                {
                    Id = url.Id,
                    OriginalLink = url.OriginalLink,
                    ShortLink = url.ShortLink,
                    NrOfClicks = url.NrOfClicks,
                    UserId = url.UserId
                })
                .ToList();

            return View(allUrls);
        }

        public IActionResult Create()
        {
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            var url = _context.Urls.FirstOrDefault(n => n.Id == id);
            _context.Urls.Remove(url);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
