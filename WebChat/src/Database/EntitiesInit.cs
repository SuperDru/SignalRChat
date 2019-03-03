using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebChat;
using WebChat.Database.Model;
using WebChat.DtoModels;

namespace WebChat.Database
{
public static class EntitiesInit
    {
        
        public static void Init(this EntityTypeBuilder<User> builder)
        {
            builder.HasData(
                new User
                {
                    Id = 1,
                    Nickname = "JFoster",
                    
                },
                new User
                {
                    Id = 2,
                    Nickname = "AShishkin"
                },
                new User
                {
                    Id = 3,
                    Nickname = "AShurikov"
                });
        }   
        
        public static void Init(this EntityTypeBuilder<Room> builder)
        {
            builder.HasData(
               new Room()
               {
                   Id = 1,
                   Name = "Developers",
                   CreatedAt = DateTime.Parse("2017-01-29"),
                   UserId = 1
               }, 
               new Room()
               {
                   Id = 2,
                   Name = "Managers",
                   CreatedAt = DateTime.Parse("2016-05-13"),
                   UserId = 3
               });
        }  
        
        public static void Init(this EntityTypeBuilder<UserCredential> builder)
        {
            var creds = new List<UserCredential>();

            var salt = PasswordGenerator.GenerateSalt();
            var password = PasswordGenerator.HashPassword("1", salt);
            creds.Add(new UserCredential()
            {
                UserId = 1,
                Salt = salt,
                HashedPassword = password
            });
            
            salt = PasswordGenerator.GenerateSalt();
            password = PasswordGenerator.HashPassword("12", salt);
            creds.Add(new UserCredential()
            {
                UserId = 2,
                Salt = salt,
                HashedPassword = password
            });
            
            salt = PasswordGenerator.GenerateSalt();
            password = PasswordGenerator.HashPassword("123", salt);
            creds.Add(new UserCredential()
            {
                UserId = 3,
                Salt = salt,
                HashedPassword = password
            });
            
            builder.HasData(creds);
        }      
        
        public static void Init(this EntityTypeBuilder<RoomCredential> builder)
        {
            var creds = new List<RoomCredential>();

            var salt = PasswordGenerator.GenerateSalt();
            var password = PasswordGenerator.HashPassword("one", salt);
            creds.Add(new RoomCredential()
            {
                RoomId = 1,
                Salt = salt,
                HashedPassword = password
            });
            
            salt = PasswordGenerator.GenerateSalt();
            password = PasswordGenerator.HashPassword("two", salt);
            creds.Add(new RoomCredential()
            {
                RoomId = 2,
                Salt = salt,
                HashedPassword = password
            });
            
            builder.HasData(creds);
        } 
    }
}