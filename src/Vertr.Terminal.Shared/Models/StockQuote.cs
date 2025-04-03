namespace Vertr.Terminal.Shared.Models;

public class StockQuote
{
    public event EventHandler QuoteUpdated;

    public string Symbol { get; set; }
    public decimal Price { get; set; }
    public decimal BasePrice { get; set; }
    public decimal RecentPrice { get; private set; }
    public decimal Percent
    {
        get
        {
            return BasePrice == 0 ? 0 : (Price / BasePrice * 100m) - 100m;
        }
    }

    public bool PriceRise { get; private set; }
    public int Volume { get; set; }
    public DateTime Time { get; set; }

    public void UpdateQuoteData(StockQuote newValues)
    {
        PriceRise = newValues.Price > RecentPrice;
        RecentPrice = Price;
        Price = newValues.Price;
        Volume = newValues.Volume;
        Time = newValues.Time;

        QuoteUpdated?.Invoke(this, new EventArgs());
    }
}
