using Shortly.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shortly.Data.Services
{
    public interface IUsersService
    {
        Task<List<AppUser>> GetUsersAsync();
        Task<AppUser> AddAsync(AppUser user);
        Task<AppUser> GetByIdAsync(int id);
        Task<AppUser> UpdateAsync(int id, AppUser user);
        Task DeleteAsync(int id);
    }
}
