using System.Text;
using Microsoft.Data.Analysis;

namespace Vertr.Experimental.Tests.Backtests;

[TestFixture(Category = "System", Explicit = true)]
public class PredictorBacktests
{
    private const string _csvFilePath = "Data\\SBER_251101_251104.csv";

    [Test]
    public void CanReaadDataFromCsv()
    {
        var csv = File.ReadAllText(_csvFilePath);
        using var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var dataFrame = DataFrame.LoadCsv(csvStream, separator: ';');

        Assert.That(dataFrame, Is.Not.Null);
        Console.WriteLine(dataFrame);
    }

    [Test]
    public void CanIterateDataRows()
    {
        var csv = File.ReadAllText(_csvFilePath);
        using var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var dataFrame = DataFrame.LoadCsv(csvStream, separator: ';');

        foreach (var row in dataFrame.Rows)
        {
            Console.WriteLine($"{row["<TIME>"]}:{row["<CLOSE>"]}");
        }
    }
}
