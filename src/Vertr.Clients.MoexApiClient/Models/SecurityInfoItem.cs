using CsvHelper.Configuration.Attributes;

namespace Vertr.Clients.MoexApiClient.Models;

internal record class SecurityInfoItem
{
    [Name("name")]
    public required string Name { get; set; }

    [Name("title")]
    public string? Title { get; set; }

    [Name("value")]
    public required string Value { get; set; }

    [Name("type")]
    public string? Type { get; set; }

    [Name("sort_order")]
    public long? SortOrder { get; set; }

    [Name("is_hidden")]
    public long? IsHidden { get; set; }

    [Name("precision")]
    public long? Precision { get; set; }
}
