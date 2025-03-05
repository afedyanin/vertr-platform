namespace Vertr.Adapters.Tinvest.Tests;

[TestFixture(Category = "integration", Explicit = true)]
public class TinvestSandboxRelatedTests : TinvestTestBase
{
    [Test]
    public async Task CreateSandboxAccount()
    {
        var accountId = await Gateway.CreateSandboxAccount("test account");

        Assert.That(accountId, Is.Not.Empty);
        Console.WriteLine($"Sandbox account created. AccountId={accountId}");
    }

    [TestCase("0fde9e6e-7bb6-4c73-b7ae-629791aa2cf6", 100000.00)]
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

    [TestCase("<account_id>")]
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
