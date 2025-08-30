using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Common.Mediator;
using Vertr.Platform.Common.Mediator;

namespace Vertr.Infrastructure.Common.Tests.Mediator;

[TestFixture(Category = "Unit")]

public class MediatrTests
{
    [Test]
    public async Task CanCallHandlerViaMediator()
    {
        var services = new ServiceCollection();
        services.AddTransient<IRequestHandler<TestRequest>, TestRequestHandler>();
        services.AddSingleton<ISomeService, SomeService>();
        services.AddSingleton<IMediator, Mediatr>();
        var serviceProvider = services.BuildServiceProvider();

        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var service = serviceProvider.GetRequiredService<ISomeService>();

        var request = new TestRequest();
        await mediator.Send(request);

        Assert.That(service.Message, Is.Not.Empty);
        Console.WriteLine(service.Message);
    }
}
