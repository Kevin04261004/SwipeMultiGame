using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPGameServer
{
    public class SwipeGame_User
    {
        public static readonly int ID_SIZE = 16;
        public static readonly int PASSWORD_SIZE = 16;
        public static readonly int NICKNAME_SIZE = 16;

        public string Id { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }

        public SwipeGame_User(string id, string password, string nickName)
        {
            Id = id;
            Password = password;
            Nickname = nickName;
        }
    }
}
