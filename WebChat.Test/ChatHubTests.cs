using System;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;
using Xunit.Abstractions;

namespace WebChat.Test
{
    
    [Collection(ApiCollectionFixture.Name)]
    public class ChatHubTests: IClassFixture<ApiFixture>
    {
        private readonly ApiFixture _context;
        private ITestOutputHelper _output;
        
        public ChatHubTests(ApiFixture fixture, ITestOutputHelper output)
        {
            _context = fixture;
            _output = output;
        }

        [Fact]
        public async void UserCanJoinChatAngGetHistoryOfMessages()
        {
            var cookieFoster = await _context.AuthorizeAsJFoster();
            await _context.JoinDevelopersRoom();

            var conFoster = await _context.CreateHubConnectionAsync("chatHub", cookieFoster);

            var startTime = DateTime.Now;
            var historyCount = await _context.Db.Messages.Where(m => m.RoomId == 1).CountAsync();

            var messagesCount = 0;
            conFoster.On<string, string, string>("info", (u, m, t) => messagesCount++);
            conFoster.On<string, string, string>("broadcast", (u, m, t) => messagesCount++);
            
            await conFoster.StartAsync();
            Thread.Sleep(300);
            messagesCount.Should().Be(historyCount + 1);

            await conFoster.StopAsync();
            
            Thread.Sleep(300);
            var messages = _context.Db.Messages.Where(m => m.SendingTime > startTime);
            _context.Db.Messages.RemoveRange(messages);
            await _context.Db.SaveChangesAsync();
        }
        
        [Fact]
        public async void UsersCanSendAndGetMessage()
        {
            var cookieFoster = await _context.AuthorizeAsJFoster();
            await _context.JoinDevelopersRoom();        
            var cookieShishkin = await _context.AuthorizeAsAShishkin();
            await _context.JoinDevelopersRoom();

            var conShishkin = await _context.CreateHubConnectionAsync("chatHub", cookieShishkin);
            var conFoster = await _context.CreateHubConnectionAsync("chatHub", cookieFoster);

            var startTime = DateTime.Now;

            string message1 = "", message2 = "";
            conFoster.On<string, string, string>("broadcast", (u, m, t) => message1 = m );
            conShishkin.On<string, string, string>("broadcast", (u, m, t) => message2 = m );
            
            await conFoster.StartAsync();
            await conShishkin.StartAsync();
            
            await conFoster.InvokeAsync("SendMessage", "Hello");
            
            Thread.Sleep(300);
            message1.Should().Be("Hello");
            message2.Should().Be("Hello");
            
            await conFoster.StopAsync();
            await conShishkin.StopAsync();
            
            Thread.Sleep(300);
            var messages = _context.Db.Messages.Where(m => m.SendingTime > startTime);
            _context.Db.Messages.RemoveRange(messages);
            await _context.Db.SaveChangesAsync();
        }
        
        [Fact]
        public async void UsersFromDifferentRoomsCantGetMessagesFromEachOther()
        {
            var cookieFoster = await _context.AuthorizeAsJFoster();
            await _context.JoinDevelopersRoom();        
            var cookieShishkin = await _context.AuthorizeAsAShishkin();
            await _context.JoinManagersRoom();

            var conShishkin = await _context.CreateHubConnectionAsync("chatHub", cookieShishkin);
            var conFoster = await _context.CreateHubConnectionAsync("chatHub", cookieFoster);

            var startTime = DateTime.Now;

            string message1 = "", message2 = "";
            conFoster.On<string, string, string>("broadcast", (u, m, t) => message1 = m );
            conShishkin.On<string, string, string>("broadcast", (u, m, t) => message2 = m );
            
            await conFoster.StartAsync();
            await conShishkin.StartAsync();
            
            await conFoster.InvokeAsync("SendMessage", "Hello");
            
            Thread.Sleep(300);
            message1.Should().Be("Hello");
            message2.Should().BeEmpty();
            
            await conFoster.StopAsync();
            await conShishkin.StopAsync();
            
            Thread.Sleep(300);
            var messages = _context.Db.Messages.Where(m => m.SendingTime > startTime);
            _context.Db.Messages.RemoveRange(messages);
            await _context.Db.SaveChangesAsync();
        }
    }
}