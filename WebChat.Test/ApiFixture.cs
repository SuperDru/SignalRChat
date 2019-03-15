using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebChat.Database;
using WebChat.DtoModels;


namespace WebChat.Test
{
    public class ApiFixture: IDisposable
    {
        public TestServer Server { get; set; }

        public HttpClient Client { get; set; }

        public ChatDbContext Db => Server.Host.Services.GetService<ChatDbContext>();
        
        public ApiFixture()
        {
            Server = new TestServer(SetupWebHost());
            
            Client = Server.CreateClient();
        }
        
        public void Dispose()
        {
            Server.Dispose();
            Client.Dispose();
        }
        
        private static IWebHostBuilder SetupWebHost()
        {
            return new WebHostBuilder()
                .ConfigureAppConfiguration((ctx, builder) =>
                {
                    builder.AddEnvironmentVariables();
                })
                .UseStartup<Startup>();
        }

        public async Task<string> AuthorizeAsJFoster()
        {
            var request = new Credential()
            {
                Name = "JFoster",
                Password = "1"
            };
            
            var clientLogin = await Client.PostAsJsonAsync("login", request);
            var cookie = clientLogin.Headers.FirstOrDefault(h => h.Key == "Set-Cookie").Value.FirstOrDefault();
            Client.DefaultRequestHeaders.Add("Cookie", cookie);
            
            return cookie;
        }
        
        public async Task<string> AuthorizeAsAShishkin()
        {
            var request = new Credential()
            {
                Name = "AShishkin",
                Password = "12"
            };
            
            var clientLogin = await Client.PostAsJsonAsync("login", request);
            var cookie = clientLogin.Headers.FirstOrDefault(h => h.Key == "Set-Cookie").Value.FirstOrDefault();
            Client.DefaultRequestHeaders.Add("Cookie", cookie);
            
            return cookie;
        }
        
        public async Task JoinDevelopersRoom()
        {
            var request = new Credential()
            {
                Name = "Developers",
                Password = "one"
            };
            
            await Client.PostAsJsonAsync("chat/join", request);  
        }
        
        public async Task JoinManagersRoom()
        {
            var request = new Credential()
            {
                Name = "Managers",
                Password = "two"
            };
            
            await Client.PostAsJsonAsync("chat/join", request);  
        }
        
        
        
        public async Task<HubConnection> CreateHubConnectionAsync(string hubName, string cookie)
        {
            var hubConnection = new HubConnectionBuilder()
                .WithUrl($"ws://localhost/{hubName}", o =>
                {
                    o.HttpMessageHandlerFactory = _ => Server.CreateHandler();
                    o.Headers.Add("Cookie", cookie);
                })
                .Build();
 
            return hubConnection;
        }
        
    }
}