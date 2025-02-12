using Quartz;

namespace Vertr.Server.QuartzJobs;

public class HelloJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await Console.Out.WriteLineAsync("Greetings from HelloJob!");
    }
}
