using System.Reflection;
using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Vertr.Adapters.Tinvest.Converters;

namespace Vertr.Adapters.Tinvest.Tests;

[TestFixture(Category = "Unit")]
public class MappingTests : TinvestTestBase
{
    [Test]
    public void CanUseTinvestMappingProfile()
    {
        // Arrange
        var config = new MapperConfiguration(configuration =>
        {
            configuration.EnableEnumMappingValidation();
            configuration.AddMaps(typeof(TinvestMappingProfile).GetTypeInfo().Assembly);
        });

        // Assert
        config.AssertConfigurationIsValid();
    }

    [Test]
    public void CanMapPriceType()
    {
        var item = Tinkoff.InvestApi.V1.PriceType.Point;
        var mapped = TinvestMapper.Map<Domain.Enums.PriceType>(item);
        Assert.That(mapped, Is.EqualTo(Domain.Enums.PriceType.Point));

        var reversed = TinvestMapper.Map<Tinkoff.InvestApi.V1.PriceType>(mapped);
        Assert.That(reversed, Is.EqualTo(item));
    }

    [Test]
    public void CanMapTimeInForceType()
    {
        var item = Tinkoff.InvestApi.V1.TimeInForceType.TimeInForceFillAndKill;
        var mapped = TinvestMapper.Map<Domain.Enums.TimeInForceType>(item);
        Assert.That(mapped, Is.EqualTo(Domain.Enums.TimeInForceType.FillAndKill));

        var reversed = TinvestMapper.Map<Tinkoff.InvestApi.V1.TimeInForceType>(mapped);
        Assert.That(reversed, Is.EqualTo(item));
    }

    [Test]
    public void CanMapMoneyToMoneyValue()
    {
        var item = new Domain.Money
        {
            Currency = "RUB",
            Value = 453.456m,
        };

        var gMoney = MoneyConverter.ToGoogleType(item);

        var mapped = TinvestMapper.Map<Tinkoff.InvestApi.V1.MoneyValue>(item);

        Assert.Multiple(() =>
        {
            Assert.That(mapped.Currency, Is.EqualTo(item.Currency));
            Assert.That(mapped.Nano, Is.EqualTo(gMoney.Nanos));
            Assert.That(mapped.Units, Is.EqualTo(gMoney.Units));
        });
    }

    [Test]
    public void CanMapMoneyValueToMoney()
    {
        var item = new Domain.Money
        {
            Currency = "RUB",
            Value = 453.456m,
        };

        var gMoney = MoneyConverter.ToGoogleType(item);

        var tMoney = new Tinkoff.InvestApi.V1.MoneyValue
        {
            Currency = gMoney.CurrencyCode,
            Nano = gMoney.Nanos,
            Units = gMoney.Units
        };

        var mapped = TinvestMapper.Map<Domain.Money>(tMoney);

        Assert.Multiple(() =>
        {
            Assert.That(mapped.Currency, Is.EqualTo(item.Currency));
            Assert.That(mapped.Value, Is.EqualTo(item.Value));
        });
    }

    [Test]
    public void CanConvertHistoricCandle()
    {
        var date = DateTime.UtcNow;
        var candle = new Tinkoff.InvestApi.V1.HistoricCandle
        {
            Open = 123.45m,
            Close = 234.56m,
            High = 345.467m,
            Low = 789.01m,
            IsComplete = true,
            Volume = 12345,
            CandleSource = Tinkoff.InvestApi.V1.CandleSource.Exchange,
            Time = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(date),
        };

        var mapped = TinvestMapper.Map<Domain.HistoricCandle>(candle);

        Assert.Multiple(() =>
        {
            Assert.That(mapped.TimeUtc, Is.EqualTo(date));
            Assert.That(mapped.Open, Is.EqualTo((decimal)candle.Open));
            Assert.That(mapped.Close, Is.EqualTo((decimal)candle.Close));
            Assert.That(mapped.High, Is.EqualTo((decimal)candle.High));
            Assert.That(mapped.Low, Is.EqualTo((decimal)candle.Low));
            Assert.That(mapped.IsCompleted, Is.EqualTo(candle.IsComplete));
            Assert.That(mapped.Volume, Is.EqualTo(candle.Volume));
            Assert.That(mapped.CandleSource, Is.EqualTo((int)candle.CandleSource));
        });
    }

    [Test]
    public void CanConvertAccount()
    {
        var openedDate = DateTime.UtcNow.AddDays(-10);

        var acc = new Tinkoff.InvestApi.V1.Account
        {
            Id = Guid.NewGuid().ToString(),
            OpenedDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(openedDate),
            ClosedDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.MinValue.ToUniversalTime()),
            Name = "Test account",
            AccessLevel = Tinkoff.InvestApi.V1.AccessLevel.AccountAccessLevelFullAccess,
            Status = Tinkoff.InvestApi.V1.AccountStatus.Open,
            Type = Tinkoff.InvestApi.V1.AccountType.Tinkoff
        };

        var mapped = TinvestMapper.Map<Domain.Account>(acc);

        Assert.Multiple(() =>
        {
            Assert.That(mapped.Id, Is.EqualTo(acc.Id));
            Assert.That(mapped.AccessLevel, Is.EqualTo(acc.AccessLevel.ToString()));
            Assert.That(mapped.Name, Is.EqualTo(acc.Name));
            Assert.That(mapped.OpenedDate, Is.EqualTo(acc.OpenedDate.ToDateTime()));
            Assert.That(mapped.ClosedDate, Is.EqualTo(acc.ClosedDate.ToDateTime()));
        });
    }
}
