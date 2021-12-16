using System;

namespace Code.Models
{
    [Serializable]
    public class User
    {
        public User()
        {
        }

        public User(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public int id;
        public string username;
        public string password;
    }
}