namespace Vertr.Domain;
public record class Money
{
    public string Currency { get; set; }

    public decimal Value { get; set; }
}

