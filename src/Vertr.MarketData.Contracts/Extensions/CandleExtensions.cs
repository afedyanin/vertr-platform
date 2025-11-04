using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace Vertr.MarketData.Contracts.Extensions;
public static class CandleExtensions
{
    private class CandleWriterMap : ClassMap<Candle>
    {
        // <DATE>,<TIME>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOL>
        public CandleWriterMap()
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
    private class CandleReaderMap : ClassMap<CandleDto>
    {
        // <DATE>,<TIME>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOL>
        public CandleReaderMap()
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

    private record class CandleDto
    {
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public long Volume { get; set; }

        public Candle ToCandle(Guid instrumentId)
            => new Candle(
                instrumentId,
                DateTime.ParseExact($"{Date}T{Time}", "yyMMddTHHmmss", CultureInfo.InvariantCulture),
                Open, Close, High, Low, Volume);
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
        csv.Context.RegisterClassMap<CandleWriterMap>();
        csv.WriteRecords(candles);
        csv.Flush();

        var csvString = Encoding.UTF8.GetString(csvStream.ToArray());

        return csvString;
    }

    public static IEnumerable<Candle> FromCsv(
        string pathToCsv,
        Guid instrumentId,
        string delimiter = ";")
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = delimiter,
        };

        using var reader = new StreamReader(pathToCsv);
        using var csv = new CsvReader(reader, config);

        csv.Context.RegisterClassMap<CandleReaderMap>();
        csv.Read();
        csv.ReadHeader();

        var res = new List<Candle>();
        while (csv.Read())
        {
            var dto = csv.GetRecord<CandleDto>();
            res.Add(dto.ToCandle(instrumentId));
        }

        return res;
    }
}

