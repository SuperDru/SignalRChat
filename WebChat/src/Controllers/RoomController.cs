using System.Collections.Generic;
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

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }
        
        [HttpPost("join")]
        [Authorize]
        public async Task Join([FromBody] Credential credentials) =>
            await _roomService.JoinRoom(User.Identity.Name, credentials);
        
        [HttpPost("leave")]
        [Authorize]
        public async Task Leave() =>
            await _roomService.LeaveRoom(User.Identity.Name);
        
        [HttpGet("rooms")]
        [Authorize]
        public async Task<List<RoomInfoResponse>> GetPublicRooms() =>
            await _roomService.GetPublicRooms();
        
        [HttpGet("{roomName}/users")]
        [Authorize]
        public async Task<List<string>> GetUsersOfRoom([FromRoute] string roomName) =>
            await _roomService.GetUsersOfRoom(roomName);
        
        [HttpPost("create")]
        [Authorize]
        public async Task CreateRoom([FromBody] Credential credentials) =>
            await _roomService.CreateRoom(User.Identity.Name, credentials);
    }
}