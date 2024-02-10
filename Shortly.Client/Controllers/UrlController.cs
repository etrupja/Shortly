using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shortly.Client.Data.ViewModels;
using Shortly.Data;
using Shortly.Data.Models;
using Shortly.Data.Services;

namespace Shortly.Client.Controllers
{
    public class UrlController : Controller
    {
        private IUrlsService _urlsService;
        private readonly IMapper _mapper;
        public UrlController(IUrlsService urlsService, IMapper mapper) 
        { 
            _urlsService = urlsService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var allUrls = _urlsService.GetUrls();
            var mappedAllUrls = _mapper.Map<List<Url>, List<GetUrlVM>>(allUrls);

            return View(mappedAllUrls);
        }

        public IActionResult Create()
        {
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            _urlsService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
