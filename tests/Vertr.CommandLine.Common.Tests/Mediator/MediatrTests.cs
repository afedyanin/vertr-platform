using Microsoft.Extensions.DependencyInjection;
using Vertr.CommandLine.Common.Mediator;

namespace Vertr.CommandLine.Common.Tests.Mediator;

[TestFixture(Category = "Unit")]

public class MediatrTests
{
    [Test]
    public async Task CanCallHandlerViaMediator()
    {
        var serviceProvider = BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var service = serviceProvider.GetRequiredService<ISomeService>();

        var request = new TestRequest();
        await mediator.Send(request);

        Assert.That(service.Message, Is.Not.Empty);
        Console.WriteLine(service.Message);
    }

    [Test]
    public async Task CanCallHandlerViaMediatorWithResponse()
    {
        var serviceProvider = BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        var request = new TestRequestWithResponse();
        var response = await mediator.Send(request);

        Assert.That(response.Message, Is.Not.Empty);
        Console.WriteLine(response.Message);
    }

    private IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddTransient<IRequestHandler<TestRequest>, TestRequestHandler>();
        services.AddTransient<IRequestHandler<TestRequestWithResponse, TestResponse>, TestRequestWithResponseHandler>();
        services.AddSingleton<ISomeService, SomeService>();
        services.AddSingleton<IMediator, Mediatr>();
        return services.BuildServiceProvider();
    }
}