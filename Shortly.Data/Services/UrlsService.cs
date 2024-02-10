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

        public Url GetById(int id)
        {
            var url = _context.Urls.FirstOrDefault(n => n.Id == id);
            return url;
        }

        public List<Url> GetUrls()
        {
            var allUrls = _context.Urls.Include(n => n.User).ToList();
            return allUrls;
        }

        public Url Add(Url url)
        {
            _context.Urls.Add(url);
            _context.SaveChanges();

            return url;
        }

        public Url Update(int id, Url url)
        {
            var urlDb = _context.Urls.FirstOrDefault(n => n.Id == id);
            if(urlDb != null)
            {
                urlDb.OriginalLink = url.OriginalLink;
                urlDb.ShortLink = url.ShortLink;
                urlDb.DateUpdated = DateTime.UtcNow;

                _context.SaveChanges();
            }

            return urlDb;
        }

        public void Delete(int id)
        {
            var urlDb = _context.Urls.FirstOrDefault(n => n.Id == id);

            if( urlDb != null)
            {
                _context.Remove(urlDb);
                _context.SaveChanges();
            }
        }
    }
}
