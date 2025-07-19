using Microsoft.Extensions.DependencyInjection;

namespace Vertr.Platform.Tests.Experimental;
public class MultipleInterfaces
{
    public interface IBar { }
    public interface IFoo { }
    public class Foo : IFoo, IBar { }

    // https://andrewlock.net/how-to-register-a-service-with-multiple-interfaces-for-in-asp-net-core-di/
    [Test]
    public void WhenRegisteredAsForwardedSingleton_InstancesAreTheSame()
    {
        var services = new ServiceCollection();

        services.AddSingleton<Foo>(); // We must explicitly register Foo
        services.AddSingleton<IFoo>(x => x.GetRequiredService<Foo>()); // Forward requests to Foo
        services.AddSingleton<IBar>(x => x.GetRequiredService<Foo>()); // Forward requests to Foo

        var provider = services.BuildServiceProvider();

        var foo1 = provider.GetService<Foo>(); // An instance of Foo
        var foo2 = provider.GetService<IFoo>(); // An instance of Foo
        var foo3 = provider.GetService<IBar>(); // An instance of Foo

        Assert.Multiple(() =>
        {
            Assert.That(foo2, Is.EqualTo(foo1)); // PASSES
            Assert.That(foo3, Is.EqualTo(foo1)); // PASSES
        });
    }
}
