using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebChat.DtoModels;

namespace WebChat.Controllers
{
    [Route("/")]
    public class LoginController: Controller
    {
        [HttpPost("login")]
        public async Task Login([FromBody] Credential credential)
        {
           
        }
    }
}