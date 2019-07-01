using Grpc.Core;
using GrpcChat.Extensions;
using GrpcChat.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcChat.ClientConsole.Clients
{
    internal class Room
    {
        private readonly Service.Room.RoomClient client;
        private readonly Action<Message> log;
        private readonly CancellationToken cancellationToken;
        private readonly Metadata headers;

        public Room(Channel channel, string authToken, Action<Message> log, CancellationToken cancellationToken)
        {
            this.client = new Service.Room.RoomClient(channel);
            this.log = log;
            this.cancellationToken = cancellationToken;

            this.headers = new Metadata
            {
                new Metadata.Entry("token", authToken)
            };
        }

        public async Task Listen()
        {

            using (var call = this.client.Listen(new Empty(), this.headers, cancellationToken: this.cancellationToken))
            {
                while (await call.ResponseStream.MoveNext() && !this.cancellationToken.IsCancellationRequested)
                {
                    var msg = call.ResponseStream.Current;
                    this.log(msg);
                }
            }
        }

        internal async Task GetHistory()
        {
            var history = await this.client.GetAllMessagesAsync(new Empty(), this.headers, cancellationToken: this.cancellationToken);
            foreach (var message in history.Messages)
            {
                this.log(message);
            }
        }

        public async Task Publish(string name, string text)
        {
            using (var call = this.client.Publish(this.headers, cancellationToken: this.cancellationToken))
            {
                var msg = new Message
                {
                    Timestamp = DateTime.Now.ToLongMilliSeconds(),
                    UserName = name,
                    Text = text
                };
                await call.RequestStream.WriteAsync(msg);
                await call.RequestStream.CompleteAsync();
            }
        }
    }
}
