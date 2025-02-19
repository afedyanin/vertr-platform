namespace Vertr.Domain;

public record class Account
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string AccessLevel { get; set; }

    public string Status { get; set; }

    public string AccountType { get; set; }

    public DateTime OpenedDate { get; set; }

    public DateTime? ClosedDate { get; set; }
}
