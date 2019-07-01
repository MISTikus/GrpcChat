using System;

namespace GrpcChat.Console
{
    public class User
    {
        public string Name { get; set; }
        public int PasswordHash { get; set; }
    }
}
