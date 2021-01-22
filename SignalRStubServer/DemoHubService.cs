using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalRStubServer.Hubs;

namespace SignalRStubServer
{
    public class DemoHubService : IDisposable
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly IHubContext<DemoHub> hubContext;
        private Task counterTask;
        private int counter;
        private int clientPings;
        private static readonly object _syncRoot = new object();

        public DemoHubService(IHubContext<DemoHub> hubContext)
        {
            this.hubContext = hubContext;
            Console.WriteLine("Starting task.");
            counterTask = Task.Factory.StartNew(() => CounterTask(cancellationTokenSource.Token), cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void AddClientPing()
        {
            clientPings++;
        }

        private async void CounterTask(CancellationToken token)
        {
            while (true)
            {
                counter++;

                try
                {
                    if (counter % 5 == 0)
                    {
                        await hubContext.Clients.All.SendAsync("ServerCounterReport", $"ServerReport: counter={counter}, clientPings={clientPings}", token);
                    }

                    Task.Delay(1000, token).Wait(token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                if (token.IsCancellationRequested)
                    return;
            }
        }

        private bool disposed = false;
        public void Dispose()
        {
            if (disposed) return;

            cancellationTokenSource.Cancel();
            counterTask?.ContinueWith(_ => { });
            cancellationTokenSource.Dispose();
            Console.WriteLine($"Dispose completed!");
            disposed = true;
        }

    }
}
