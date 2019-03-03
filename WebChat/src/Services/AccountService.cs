using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebChat;
using WebChat.Database;
using WebChat.Database.Model;
using WebChat.Repositories;

namespace WebChat.Services
{
    public interface IAccountService
    {
        Task<bool> CheckPassword(User user, string password);
        Task<ClaimsPrincipal> Login(User user);
    }

    public class AccountService : IAccountService
    {
        private readonly ChatDbContext _dbContext;
        private readonly IUserRepository _rep;
    
        public AccountService(ChatDbContext dbContext, IUserRepository rep)
        {
            _dbContext = dbContext;
            _rep = rep;
        }
    
        public async Task<bool> CheckPassword(User user, string password)
        {
            var cred = await _dbContext.UserCredentials.FirstOrDefaultAsync(c => c.UserId == user.Id);

            var hashedPassword = PasswordGenerator.HashPassword(password, cred.Salt);

            return hashedPassword == cred.HashedPassword;
        }

        public async Task<ClaimsPrincipal> Login(User user)
        {
            var room = await _rep.GetUserRoom(user.Nickname);
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Nickname),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.UserData, room == null ? "" : room.Name)
            };
            
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}