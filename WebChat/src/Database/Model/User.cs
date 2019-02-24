namespace WebChat.Database.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Nickname { get; set; }

        public Room CurrentRoom { get; set; }
    }
}