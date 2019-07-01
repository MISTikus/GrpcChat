using Grpc.Core;
using GrpcChat.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcChat.Console.Services
{
    internal class Room : Service.Room.RoomBase
    {
        private readonly ConcurrentQueue<Message> messages;
        private readonly Dictionary<string, (User user, DateTime tokenExpiration)> authCache;
        private readonly Action<Message> log;
        private readonly CancellationToken cancellationToken;

        private delegate void OnMessageReceived(Message message);
        private event OnMessageReceived MessageReceived;

        public Room(
            Dictionary<string, (User user, DateTime tokenExpiration)> authCache, 
            Action<Message> log, 
            CancellationToken cancellationToken)
        {
            this.messages = new ConcurrentQueue<Message>();
            this.authCache = authCache;
            this.log = log;
            this.cancellationToken = cancellationToken;
        }

        public override async Task<History> GetAllMessages(Empty request, ServerCallContext context)
        {
            CheckAuth(context);

            var result = new History();
            result.Messages.AddRange(
                new[] { new Message { UserName = "Server", Text = "Welcome to [RoomName]! (still working on it...)" } }
                .Union(this.messages)
            );

            return result;
        }

        public override async Task Listen(Empty request, IServerStreamWriter<Message> responseStream, ServerCallContext context)
        {
            CheckAuth(context);

            MessageReceived += async m => await responseStream.WriteAsync(m);
            while (!this.cancellationToken.IsCancellationRequested)
                Thread.Sleep(100);
        }

        public override async Task<Empty> Publish(IAsyncStreamReader<Message> requestStream, ServerCallContext context)
        {
            CheckAuth(context);

            while (await requestStream.MoveNext() && !this.cancellationToken.IsCancellationRequested)
            {
                try
                {
                    this.log(requestStream.Current);
                    this.messages.Enqueue(requestStream.Current);
                    MessageReceived?.Invoke(requestStream.Current);
                }
                catch (Exception e)
                {
                    var color = System.Console.ForegroundColor;
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("Error: " + e.Message);
                    System.Console.ForegroundColor = color;
                }
            }
            return new Empty();
        }

        private void CheckAuth(ServerCallContext context)
        {
            var token = context.RequestHeaders.FirstOrDefault(x => x.Key == "token")?.Value;
            if (token == null
                || !this.authCache.TryGetValue(token, out var user)
                || user.tokenExpiration <= DateTime.UtcNow)
                throw new ArgumentException("Failed to authenticate user by token.");
        }
    }
}
