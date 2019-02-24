using Microsoft.AspNetCore.SignalR;

namespace WebChat
{
    public class ChatHub: Hub
    {
        public async void SendMessage(string message)
        {
            await Clients.All.SendAsync("broadcast message", message);
        }
    }
}