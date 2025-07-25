using System.Threading.Channels;

namespace Vertr.Experimental.Tests;
public class ChannelTests
{
    // https://deniskyashif.com/2019/12/08/csharp-channels-part-1/
    [Test]
    public async Task BasicUsingCahnnels()
    {
        var ch = Channel.CreateUnbounded<string>();

        var consumer = Task.Run(async () =>
        {
            while (await ch.Reader.WaitToReadAsync())
            {
                Console.WriteLine(await ch.Reader.ReadAsync());
            }
        });

        var producer = Task.Run(async () =>
        {
            var rnd = new Random();

            for (var i = 0; i < 5; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(rnd.Next(3)));
                await ch.Writer.WriteAsync($"Message {i}");
            }
            ch.Writer.Complete();
        });

        await Task.WhenAll(producer, consumer);
    }
}
