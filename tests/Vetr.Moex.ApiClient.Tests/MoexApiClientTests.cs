using Refit;
using Vertr.Clients.MoexApiClient;

namespace Vetr.Moex.ApiClient.Tests;

public class MoexApiClientTests
{
    private const string BaseUrl = "https://iss.moex.com/iss/";

    private IMoexApiClient _api;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _api = RestService.For<IMoexApiClient>(BaseUrl);
    }

    [Test]
    public async Task CanGetSecurities()
    {
        var json = await _api.GetAllSecurities();

        Assert.That(json, Is.Not.Empty);

        Console.WriteLine(json);
    }

    [Test]
    public async Task CanGetMetadata()
    {
        var json = await _api.GetMetadata();

        Assert.That(json, Is.Not.Empty);

        Console.WriteLine(json);
    }

    [Test]
    public async Task CanGetRusfarCandles()
    {
        var json = await _api.GetRusfarCandles();

        Assert.That(json, Is.Not.Empty);

        Console.WriteLine(json);
    }

    [Test]
    public async Task CanGetRusfarDetails()
    {
        var json = await _api.GetRusfarDetails();

        Assert.That(json, Is.Not.Empty);

        Console.WriteLine(json);
    }
}
