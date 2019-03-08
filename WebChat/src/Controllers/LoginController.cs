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
        private readonly IAccountService _accountService;
        
        public LoginController(IAccountService accountService)
        {
            _accountService = accountService;
        }
 
        
        [HttpPost("login")]
        public async Task Login([FromBody]Credential cred)
        {
            await HttpContext.SignInAsync(await _accountService.Login(cred));
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