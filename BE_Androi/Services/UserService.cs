using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using Repositories.Models;

namespace Services
{


    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<User> GetUserByUsername(string username);
        Task Register(User user);
    }

    public class UserService : IUserService
    {
        private readonly UserRepository _repository;

        public UserService(UserRepository repository)
        {
            _repository = repository;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            var user = await _repository.GetUserAccount(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;
            return user;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await _repository.GetUserAccount(username);
        }

        public async Task Register(User user)
        {
            await _repository.CreateAsync(user);
        }
    }
}
