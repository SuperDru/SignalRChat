using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qoden.Validation;
using WebChat;
using WebChat.DtoModels;
using WebChat.Repositories;
using WebChat.Services;

namespace WebChat.Controllers
{
    [Route("/")]
    public class LoginController: Controller
    {
        private readonly IUserRepository _rep;
        private readonly IAccountService _accountService;
        
        public LoginController(IUserRepository rep, IAccountService accountService)
        {
            _rep = rep;
            _accountService = accountService;
        }
 
        
        [HttpPost("login")]
        public async Task Login([FromBody]Credential cred)
        {
            var user = await _rep.GetUser(cred.Name);
            Check.Value(user, "credentials").NotNull(ErrorMessages.CredentialsMsg);

            var correct = await _accountService.CheckPassword(user, cred.Password);
            Check.Value(correct, "credentials").IsTrue(ErrorMessages.CredentialsMsg);

            await HttpContext.SignInAsync(await _accountService.Login(user));
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            Response.Redirect("/");
        }
    }
}