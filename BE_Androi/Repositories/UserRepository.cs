using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;

namespace Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository() { }

        public async Task<User> GetUserAccount(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        // Giữ nguyên phương thức cũ nhưng không dùng trong Authenticate nữa
        public async Task<User> GetUserAccount(string username, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password);
        }
    }
}
