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
            var user = await _userRep.GetUser(User.Identity.Name);
            var room = await _roomService.GetRoom(credentials.Name);

            Check.Value(room, "room credentials").NotNull(ErrorMessages.CredentialsMsg);

            var check = await _roomService.CheckPassword(room, credentials.Password);
            
            Check.Value(check, "room credentials").IsTrue(ErrorMessages.CredentialsMsg);

            await _roomService.JoinRoom(user, room);
        }
        
        [HttpPost("create")]
        public async Task Create([FromBody] Credential credentials)
        {
            
        }
        
        [HttpPost("remove")]
        public async Task Remove([FromBody] Credential credentials)
        {
            
        }
    }
}