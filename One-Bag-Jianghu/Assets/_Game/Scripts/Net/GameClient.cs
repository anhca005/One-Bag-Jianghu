using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace OneBagJianghu.Net
{
    public sealed class GameClient : IDisposable
    {
        private readonly object gate = new object();
        private readonly IGameTransport transport;
        private readonly Dictionary<long, TaskCompletionSource<Envelope>> pending = new Dictionary<long, TaskCompletionSource<Envelope>>();
        private readonly SemaphoreSlim sendGate = new SemaphoreSlim(1, 1);
        private CancellationTokenSource lifetime;
        private long nextId;
        private bool disconnectedRaised;

        public GameClient(IGameTransport transport = null)
        {
            this.transport = transport ?? new WebSocketTransport();
        }

        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(15);
        public bool IsConnected => transport.IsConnected;
        public event Action Connected;
        public event Action Disconnected;
        public event Action<Envelope> ServerMessage;

        public async Task ConnectAsync(string url, CancellationToken cancellationToken = default)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                throw new ArgumentException("Invalid WebSocket URL.", nameof(url));
            }

            lock (gate)
            {
                if (lifetime != null)
                {
                    throw new InvalidOperationException("The client is already connected or connecting.");
                }

                lifetime = new CancellationTokenSource();
                disconnectedRaised = false;
            }

            try
            {
                await transport.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);
                _ = ReceiveLoopAsync(lifetime.Token);
                Connected?.Invoke();
            }
            catch
            {
                Disconnect();
                throw;
            }
        }

        public async Task<Envelope> SendAsync(string type, object payload)
        {
            if (!transport.IsConnected)
            {
                throw new InvalidOperationException("The client is not connected.");
            }

            long id = Interlocked.Increment(ref nextId);
            Envelope request = ProtocolJson.Build(type, id, payload);
            var completion = new TaskCompletionSource<Envelope>(TaskCreationOptions.RunContinuationsAsynchronously);
            lock (gate)
            {
                pending.Add(id, completion);
            }

            try
            {
                await sendGate.WaitAsync().ConfigureAwait(false);
                try
                {
                    await transport.SendTextAsync(ProtocolJson.Serialize(request), CancellationToken.None).ConfigureAwait(false);
                }
                finally
                {
                    sendGate.Release();
                }

                Task delay = Task.Delay(RequestTimeout);
                Task finished = await Task.WhenAny(completion.Task, delay).ConfigureAwait(false);
                if (finished != completion.Task)
                {
                    throw new TimeoutException("The server did not reply within " + RequestTimeout.TotalSeconds + " seconds.");
                }

                return await completion.Task.ConfigureAwait(false);
            }
            finally
            {
                lock (gate)
                {
                    pending.Remove(id);
                }
            }
        }

        public void Disconnect()
        {
            CancellationTokenSource source;
            lock (gate)
            {
                source = lifetime;
                lifetime = null;
            }

            if (source != null)
            {
                source.Cancel();
                source.Dispose();
            }

            FailPending(new InvalidOperationException("The connection was closed."));
            _ = CloseTransportAsync();
            RaiseDisconnected();
        }

        public void Dispose()
        {
            Disconnect();
            sendGate.Dispose();
            transport.Dispose();
        }

        private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    string json = await transport.ReceiveTextAsync(cancellationToken).ConfigureAwait(false);
                    if (json == null)
                    {
                        break;
                    }

                    Envelope envelope = ProtocolJson.Parse(json);
                    TaskCompletionSource<Envelope> completion = null;
                    if (envelope.ReplyTo != 0)
                    {
                        lock (gate)
                        {
                            pending.TryGetValue(envelope.ReplyTo, out completion);
                        }
                    }

                    if (completion != null)
                    {
                        completion.TrySetResult(envelope);
                    }
                    else
                    {
                        ServerMessage?.Invoke(envelope);
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                FailPending(exception);
            }
            finally
            {
                RaiseDisconnected();
            }
        }

        private async Task CloseTransportAsync()
        {
            try
            {
                await transport.CloseAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch
            {
            }
        }

        private void FailPending(Exception exception)
        {
            TaskCompletionSource<Envelope>[] completions;
            lock (gate)
            {
                completions = new TaskCompletionSource<Envelope>[pending.Count];
                pending.Values.CopyTo(completions, 0);
                pending.Clear();
            }

            for (int i = 0; i < completions.Length; i++)
            {
                completions[i].TrySetException(exception);
            }
        }

        private void RaiseDisconnected()
        {
            lock (gate)
            {
                if (disconnectedRaised)
                {
                    return;
                }

                disconnectedRaised = true;
            }

            Disconnected?.Invoke();
        }
    }
}
