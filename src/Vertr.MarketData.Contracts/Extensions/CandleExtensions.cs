using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace Vertr.MarketData.Contracts.Extensions;
public static class CandleExtensions
{
    private class CandleMap : ClassMap<Candle>
    {
        // <DATE>,<TIME>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOL>
        public CandleMap()
        {
            Map(m => m.Date).Index(0).Name("<DATE>");
            Map(m => m.Time).Index(1).Name("<TIME>");
            Map(m => m.Open).Index(2).Name("<OPEN>");
            Map(m => m.High).Index(3).Name("<HIGH>");
            Map(m => m.Low).Index(4).Name("<LOW>");
            Map(m => m.Close).Index(5).Name("<CLOSE>");
            Map(m => m.Volume).Index(6).Name("<VOL>");
        }
    }

    public static string? ToCsv(this IEnumerable<Candle> candles)
    {
        if (candles == null)
        {
            return null;
        }

        using var csvStream = new MemoryStream();
        using var writer = new StreamWriter(csvStream, Encoding.UTF8);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<CandleMap>();
        csv.WriteRecords(candles);
        csv.Flush();

        var csvString = Encoding.UTF8.GetString(csvStream.ToArray());

        return csvString;
    }
}

