using Microsoft.AspNetCore.Mvc;
using Shortly.Client.Data.Models;

namespace Shortly.Client.Controllers
{
    public class UrlController : Controller
    {
        public IActionResult Index()
        {
            var tempData = TempData["SuccessMessage"];
            var viewBag = ViewBag.Test1;
            var viewData = ViewData["Test2"];

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            return View();
        }

        public IActionResult Create()
        {
            //Shorten URL
            var shortenedURL = "short";

            TempData["SuccessMessage"] = "Successful!";
            ViewBag.Test1 = "test1";
            ViewData["Test2"] = "test2";

            return RedirectToAction("Index");
        }
    }
}
