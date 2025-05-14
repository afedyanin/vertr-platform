namespace Vertr.Adapters.Tinvest.Tests;

[TestFixture(Category = "integration", Explicit = true)]
public class TinvestSandboxRelatedTests : TinvestTestBase
{
    [TestCase("sb3")]
    [TestCase("random walk")]
    [TestCase("trend follow")]
    public async Task CreateSandboxAccount(string accountName)
    {
        var accountId = await Gateway.CreateSandboxAccount(accountName);

        Assert.That(accountId, Is.Not.Empty);
        Console.WriteLine($"Sandbox account created. AccountId={accountId}");
    }

    [TestCase("a48c2760-20ae-4e0a-8d4b-4005cdb10d70", 100000.00)]
    [TestCase("0e284896-ba30-440f-9626-18ab2e2cc2f0", 100000.00)]
    [TestCase("f7c33024-67bc-428e-a149-e916e87e79ad", 100000.00)]
    public async Task DepositSandboxAccount(string accountId, decimal deposit)
    {
        var amount = new Domain.Money
        {
            Currency = "rub",
            Value = deposit
        };

        var balance = await Gateway.SandboxPayIn(accountId, amount);
        Console.WriteLine($"Sandbox AccountId={accountId} Balance={balance}");
    }

    [TestCase("0ae71fa0-d8ef-4043-8c53-0a353072954b")]
    [TestCase("d43202f0-b9c5-4afb-a50d-22ae20351195")]
    [TestCase("6ce70491-fb13-491d-be64-71e667af4383")]
    public async Task CloseSandboxAccount(string accountId)
    {
        await Gateway.CloseSandboxAccount(accountId);
        Assert.Pass();
    }

    [Test]
    public async Task GetAllAccounts()
    {
        var accounts = await Gateway.GetAccounts();

        Assert.That(accounts, Is.Not.Null);

        foreach (var account in accounts)
        {
            Console.WriteLine($"{account}");
        }
    }
}
