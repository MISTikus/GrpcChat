using GrpcChat.Models;

namespace GrpcChat.Console.Extensions
{
    internal static class AuthenticationExtensions
    {
        public static Credentials ToCredentials(this User user, string token)
        {
            return new Credentials
            {
                UserName = user.Name,
                Token = token,
                Result = AuthResult.Success
            };
        }
    }
}
