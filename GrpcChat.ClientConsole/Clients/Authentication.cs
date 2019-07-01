using Grpc.Core;
using GrpcChat.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcChat.ClientConsole.Clients
{
    internal class Authentication
    {
        private readonly Service.Authentication.AuthenticationClient client;
        private readonly Action<Message> log;
        private readonly CancellationToken cancellationToken;

        public Authentication(Channel channel, Action<Message> log, CancellationToken cancellationToken)
        {
            this.client = new Service.Authentication.AuthenticationClient(channel);
            this.log = log;
            this.cancellationToken = cancellationToken;
        }

        public async Task<(string registeredUserName, string token)> GetAuthnticationToken(string userName, string password)
        {
            var creds = new Credentials
            {
                UserName = userName,
                Password = password
            };

            var result = await this.client.AuthenticateAsync(creds, cancellationToken: this.cancellationToken);
            if (result.Result > 0)
            {
                this.log(new Message { UserName = "Server", Text = ParseResult(result.Result) });
                return (null, null);
            }
            return (result.UserName, result.Token);
        }

        private string ParseResult(AuthResult result)
        {
            switch (result)
            {
                case AuthResult.FailToken: return "Not valid token";
                case AuthResult.FailLogin: return "User not found by Login";
                case AuthResult.FailPassword: return "Password incorrect";
                default: return string.Empty;
            }
        }
    }
}
