using System.Globalization;
using CsvHelper;

namespace Vertr.Common.Aplication.Tests;

public class CsvExporterTests
{
    [Test]
    public void CanExportItemsToCsv()
    {
        var records = new List<Person>
        {
            new Person { Id = 1, Name = "John Doe", Email = "john@example.com" },
            new Person { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
        };

        using (var writer = new StreamWriter("test_file.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(records);
        }
    }
}

public class Person
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
}

