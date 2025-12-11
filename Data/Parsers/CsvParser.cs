using AgrculturePriceTracker.Core.Interfaces;
using AgrculturePriceTracker.Core.Models;

namespace AgrculturePriceTracker.Data.Parsers
{
    public class CsvParser : IDataParser
    {
        public IEnumerable<PriceRecord> Parse(string filePath)
        {
            var results = new List<PriceRecord>();

            if (!File.Exists(filePath))
                throw new FileNotFoundException("CSV file not found", filePath);

            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(',');

                if (parts.Length < 8)
                    continue;

                if (!DateTime.TryParse(parts[6], out DateTime date))
                    continue;

                if (!decimal.TryParse(parts[7], out decimal price))
                    continue;

                results.Add(new PriceRecord
                {
                    ProduceId = parts[0],
                    ProduceName = parts[1],
                    Category = parts[2],
                    MarketId = parts[3],
                    MarketName = parts[4],
                    Region = parts[5],
                    State = "",

                    Date = date,
                    PricePerKg = price
                });
            }

            return results;
        }
    }
}
