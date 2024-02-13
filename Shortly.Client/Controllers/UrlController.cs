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

        public async Task<IActionResult> Index()
        {
            var allUrls = await _urlsService.GetUrlsAsync();
            var mappedAllUrls = _mapper.Map<List<Url>, List<GetUrlVM>>(allUrls);

            return View(mappedAllUrls);
        }

        public async Task<IActionResult> Create()
        {
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int id)
        {
            await _urlsService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
