using Microsoft.EntityFrameworkCore;
using Shortly.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shortly.Data.Services
{
    public class UrlsService:IUrlsService
    {
        private AppDbContext _context;
        public UrlsService(AppDbContext context)
        {
            _context = context;
        }

        public List<Url> GetUrls()
        {
            var allUrls = _context.Urls.Include(n => n.User).ToList();
            return allUrls;
        }
    }
}
