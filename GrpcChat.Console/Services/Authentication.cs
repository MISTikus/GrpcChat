using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcChat.Console.Extensions;
using GrpcChat.Models;

namespace GrpcChat.Console.Services
{
    public class Authentication : Service.Authentication.AuthenticationBase
    {
        private readonly Dictionary<string, (User user, DateTime tokenExpiration)> authCache;
        private readonly List<User> users;

        public Authentication(Dictionary<string, (User user, DateTime tokenExpiration)> authCache, List<User> users)
        {
            this.authCache = authCache;
            this.users = users;
        }

        public override async Task<Credentials> Authenticate(Credentials request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Token) && string.IsNullOrWhiteSpace(request.UserName))
                return new Credentials { Result = AuthResult.FailLogin };

            if (!string.IsNullOrWhiteSpace(request.Token)
                && this.authCache.TryGetValue(request.Token, out var value)
                && value.tokenExpiration > DateTime.UtcNow)
                return value.user.ToCredentials(request.Token);

            var user = this.users.FirstOrDefault(x => x.Name.ToLower() == request.UserName.ToLower());
            if (user == null)
                return new Credentials { Result = AuthResult.FailLogin };
            if (user.PasswordHash != request.Password.GetHashCode())
                return new Credentials { Result = AuthResult.FailPassword };

            var token = Guid.NewGuid().ToString("N");
            this.authCache.Add(token, (user, DateTime.UtcNow.AddHours(3)));
            return user.ToCredentials(token);
        }
    }
}
