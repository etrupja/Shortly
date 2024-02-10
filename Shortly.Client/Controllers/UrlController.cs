using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                .Include(n => n.User)
                .Select(url => new GetUrlVM()
                {
                    Id = url.Id,
                    OriginalLink = url.OriginalLink,
                    ShortLink = url.ShortLink,
                    NrOfClicks = url.NrOfClicks,
                    UserId = url.UserId,

                    User = url.User != null ? new GetUserVM()
                    {
                        Id = url.User.Id,
                        FullName = url.User.FullName
                    } : null
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
