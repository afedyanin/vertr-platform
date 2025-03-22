using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertr.Terminal.Host.Models;

internal class Program
{
    static async Task Main(string[] args)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/stocks")
            .ConfigureLogging(logging =>
            {
                logging.AddConsole();
            })
            .AddMessagePackProtocol()
            .Build();

        await connection.StartAsync();

        Console.WriteLine("Starting connection. Press Ctrl-C to close.");
        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, a) =>
        {
            a.Cancel = true;
            cts.Cancel();
        };

        connection.Closed += e =>
        {
            Console.WriteLine("Connection closed with error: {0}", e);

            cts.Cancel();
            return Task.CompletedTask;
        };


        connection.On("marketOpened", () =>
        {
            Console.WriteLine("Market opened");
        });

        connection.On("marketClosed", () =>
        {
            Console.WriteLine("Market closed");
        });

        connection.On("marketReset", () =>
        {
            // We don't care if the market rest
        });

        var channel = await connection.StreamAsChannelAsync<Stock>("StreamStocks", CancellationToken.None);
        while (await channel.WaitToReadAsync() && !cts.IsCancellationRequested)
        {
            while (channel.TryRead(out var stock))
            {
                Console.WriteLine($"{stock.Symbol} {stock.Price}");
            }
        }
    }
}
