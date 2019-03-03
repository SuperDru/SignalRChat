using System.Net.NetworkInformation;
using System.Security.Policy;

namespace WebChat
{
    public static class ErrorMessages
    {
        public static string CredentialsMsg = "Invalid name or password";
        public static string IncorrectDateUpdateMsg = "Incorrect date update";
        
        public static string NoUserWithIdMsg(int id) => $"No user with id '{id}'";
        public static string NoUserWithNicknameMsg(string nickname) => $"No user with nickname '{nickname}'";
        public static string UserWithNicknameExistsMsg(string nickname) => $"User with nickname '{nickname}' already exists";
    }
}