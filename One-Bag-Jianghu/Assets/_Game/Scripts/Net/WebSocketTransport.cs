using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OneBagJianghu.Net
{
    public sealed class WebSocketTransport : IGameTransport
    {
        private readonly ClientWebSocket socket = new ClientWebSocket();
        public bool IsConnected => socket.State == WebSocketState.Open;

        public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            return socket.ConnectAsync(uri, cancellationToken);
        }

        public Task SendTextAsync(string text, CancellationToken cancellationToken)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            var segment = new ArraySegment<byte>(bytes);
            return socket.SendAsync(segment, WebSocketMessageType.Text, true, cancellationToken);
        }

        public async Task<string> ReceiveTextAsync(CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[4096];
            using (var stream = new MemoryStream())
            {
                while (true)
                {
                    var segment = new ArraySegment<byte>(buffer);
                    WebSocketReceiveResult result = await socket.ReceiveAsync(segment,
                        cancellationToken).ConfigureAwait(false);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        return null;
                    }

                    if (result.MessageType != WebSocketMessageType.Text)
                    {
                        if (result.EndOfMessage)
                        {
                            continue;
                        }

                        continue;
                    }

                    stream.Write(buffer, 0, result.Count);
                    if (result.EndOfMessage)
                    {
                        return Encoding.UTF8.GetString(stream.ToArray());
                    }
                }
            }
        }

        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    "Client disconnect",
                    cancellationToken).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            socket.Dispose();
        }
    }
}