using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Qoden.Validation;
using WebChat;
using WebChat.Database;
using WebChat.Database.Model;
using WebChat.DtoModels;
using WebChat.Repositories;

namespace WebChat.Services
{
    public interface IAccountService
    {
        Task<ClaimsPrincipal> Login(Credential cred);
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
    
        private static bool CheckPassword(User user, string password) =>
            PasswordGenerator.HashPassword(password, user.Salt) == user.Password;

        private async Task<ClaimsPrincipal> GetClaimsPrincipal(User user)
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
        
        public async Task<ClaimsPrincipal> Login(Credential cred)
        {
            var user = await _rep.GetUser(cred.Name);
            Check.Value(user, "credentials").NotNull(ErrorMessages.CredentialsMsg);

            var correct = CheckPassword(user, cred.Password);
            Check.Value(correct, "credentials").IsTrue(ErrorMessages.CredentialsMsg);

            return await GetClaimsPrincipal(user);
        }
    }
}