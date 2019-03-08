using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qoden.Validation;
using WebChat.DtoModels;
using WebChat.Repositories;
using WebChat.Services;

namespace WebChat.Controllers
{
    [Route("chat")]
    public class RoomController: Controller
    {
        private readonly IRoomService _roomService;
        private IUserRepository _userRep;

        public RoomController(IRoomService roomService, IUserRepository userRep)
        {
            _roomService = roomService;
            _userRep = userRep;
        }
        
        [HttpPost("join")]
        [Authorize]
        public async Task Join([FromBody] Credential credentials)
        {
            await _roomService.JoinRoom(User.Identity.Name, credentials);
        }
        
        [HttpPost("create")]
        public async Task Create([FromBody] Credential credentials)
        {
            await _roomService.CreateRoom(User.Identity.Name, credentials);
        }
    }
}