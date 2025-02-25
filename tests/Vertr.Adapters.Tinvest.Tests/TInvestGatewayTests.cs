using Google.Type;
using Vertr.Domain.Enums;

namespace Vertr.Adapters.Tinvest.Tests;

[TestFixture(Category = "integration", Explicit = true)]
public class TinvestGatewayTests : TinvestTestBase
{
    [Test]
    public async Task CanFindInstrument()
    {
        var instruments = await Gateway.FindInstrument("SBER");

        Assert.That(instruments, Is.Not.Null);

        foreach (var instrument in instruments)
        {
            Console.WriteLine($"{instrument}");
        }
    }

    [TestCase("SBER", "TQBR")]
    [TestCase("SBERP", "TQBR")]
    [TestCase("T", "TQBR")]
    [TestCase("VTBR", "TQBR")]
    [TestCase("GAZP", "TQBR")]
    [TestCase("LKOH", "TQBR")]
    [TestCase("ROSN", "TQBR")]
    [TestCase("AFKS", "TQBR")]
    [TestCase("MOEX", "TQBR")]
    [TestCase("GMKN", "TQBR")]
    [TestCase("MGNT", "TQBR")]
    [TestCase("X5", "TQBR")]
    [TestCase("NLMK", "TQBR")]
    [TestCase("OZON", "TQBR")]
    public async Task CanGetInstrument(string ticker, string classCode)
    {
        var instrument = await Gateway.GetInstrument(ticker, classCode);

        Assert.That(instrument, Is.Not.Null);

        Console.WriteLine($"{instrument}");
    }

    [Test]
    public async Task CanGetCandles()
    {
        var to = System.DateTime.UtcNow;
        var from = to.AddDays(-1);

        var sberUid = Settings.GetSymbolId("SBER");

        var candles = await Gateway.GetCandles(
            sberUid!,
            CandleInterval._10Min,
            from,
            to);

        Assert.That(candles, Is.Not.Null);
        Assert.That(candles.Count, Is.GreaterThanOrEqualTo(1));

        foreach (var candle in candles)
        {
            Console.WriteLine($"{candle}");
        }
    }

    // accountId=d5ac27d9-1066-4933-832c-a480ebaa5cf1
    [Test]
    public async Task CanCreateSandboxAccount()
    {
        var accountId = await Gateway.CreateSandboxAccount("test account");

        Assert.That(accountId, Is.Not.Empty);
        Console.WriteLine($"accountId={accountId}");
    }

    [Test]
    public async Task CanGetAccounts()
    {
        var accounts = await Gateway.GetAccounts();

        Assert.That(accounts, Is.Not.Null);

        foreach (var account in accounts)
        {
            Console.WriteLine($"{account}");
        }
    }

    [Test]
    public async Task CanPayInSandboxAccount()
    {
        var amount = new Domain.Money
        {
            Currency = "rub",
            Value = 45.78m
        };

        var accountId = "d5ac27d9-1066-4933-832c-a480ebaa5cf1";

        var result = await Gateway.SandboxPayIn(accountId, amount);

        Assert.Multiple(() =>
        {
            Assert.That(result.Currency, Is.EqualTo(amount.Currency));
            Assert.That(result.Value, Is.GreaterThanOrEqualTo(amount.Value));
        });
    }

    [Test]
    public async Task CanCloseSandboxAccount()
    {
        var accountId = "d5ac27d9-1066-4933-832c-a480ebaa5cf1";
        await Gateway.CloseSandboxAccount(accountId);
    }
}
