using System;
using System.Threading;
using System.Threading.Tasks;

namespace OneBagJianghu.Net
{
    public interface IGameTransport : IDisposable
    {
        bool IsConnected { get; }
        Task ConnectAsync(Uri uri, CancellationToken cancellationToken);
        Task SendTextAsync(string text, CancellationToken cancellationToken);
        Task<string> ReceiveTextAsync(CancellationToken cancellationToken);
        Task CloseAsync(CancellationToken cancellationToken);
    }
}
