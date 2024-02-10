using Microsoft.EntityFrameworkCore;
using Shortly.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shortly.Data.Services
{
    public class UsersService:IUsersService
    {
        private AppDbContext _context;
        public UsersService(AppDbContext context)
        {
            _context = context;
        }

        public User GetById(int id)
        {
            var user = _context.Users.FirstOrDefault(n => n.Id == id);
            return user;
        }

        public List<User> GetUsers()
        {
            var users = _context.Users.Include(n => n.Urls).ToList();
            return users;
        }

        public User Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public User Update(int id, User user)
        {
            var userDb = _context.Users.FirstOrDefault(u => u.Id == id);

            if(userDb != null)
            {
                userDb.Email = user.Email;
                userDb.FullName = user.FullName;

                _context.SaveChanges();
            }

            return userDb;
        }

        public void Delete(int id)
        {
            var userDb = _context.Users.FirstOrDefault(u => u.Id == id);
            if(userDb != null)
            {
                _context.Users.Remove(userDb);
                _context.SaveChanges();
            }
        }
    }
}
