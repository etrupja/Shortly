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
        List<User> GetUsers();
        User Add(User user);
        User GetById(int id);
        User Update(int id, User user);
        void Delete(int id);
    }
}
