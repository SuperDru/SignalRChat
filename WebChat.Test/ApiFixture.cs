using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using WebChat.DtoModels;


namespace WebChat.Test
{
    public class ApiFixture: IDisposable
    {
        public TestServer Server { get; set; }

        public HttpClient Client { get; set; }

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

        public async Task AuthorizeAsJFoster()
        {
            var request = new Credential()
            {
                Name = "JFoster",
                Password = "1"
            };
            
            var clientLogin = await Client.PostAsJsonAsync("login", request);
            var cookie = clientLogin.Headers.FirstOrDefault(h => h.Key == "Set-Cookie").Value;
            Client.DefaultRequestHeaders.Add("Cookie", cookie);
        }
        
        public async Task AuthorizeAsAShishkin()
        {
            var request = new Credential()
            {
                Name = "AShishkin",
                Password = "12"
            };
            
            var clientLogin = await Client.PostAsJsonAsync("login", request);
            var cookie = clientLogin.Headers.FirstOrDefault(h => h.Key == "Set-Cookie").Value;
            Client.DefaultRequestHeaders.Add("Cookie", cookie);
        }
        
        public async Task JoinRoom()
        {
            var request = new Credential()
            {
                Name = "Developers",
                Password = "one"
            };
            
            
            await Client.PostAsJsonAsync("chat/join", request);
            
        }
        
        public async Task<HubConnection> StartConnectionAsync(string hubName)
        {
            var hubConnection = new HubConnectionBuilder()
                .WithUrl($"ws://localhost/{hubName}", o =>
                {
                    
                    o.HttpMessageHandlerFactory = _ => Server.CreateHandler();
                })
                .Build();
 
            await hubConnection.StartAsync();
 
            return hubConnection;
        }
        
    }
}