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

        public async Task<User> GetByIdAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(n => n.Id == id);
            return user;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var users = await _context.Users.Include(n => n.Urls).ToListAsync();
            return users;
        }

        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateAsync(int id, User user)
        {
            var userDb = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if(userDb != null)
            {
                userDb.Email = user.Email;
                userDb.FullName = user.FullName;

                await _context.SaveChangesAsync();
            }

            return userDb;
        }

        public async Task DeleteAsync(int id)
        {
            var userDb = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if(userDb != null)
            {
                _context.Users.Remove(userDb);
                await _context.SaveChangesAsync();
            }
        }
    }
}
