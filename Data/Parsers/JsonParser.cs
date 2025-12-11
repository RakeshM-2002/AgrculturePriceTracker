using System.Text.Json;
using AgrculturePriceTracker.Core.Interfaces;
using AgrculturePriceTracker.Core.Models;

namespace AgrculturePriceTracker.Data.Parsers
{
    public class JsonParser : IDataParser
    {
        public IEnumerable<PriceRecord> Parse(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("JSON file not found", filePath);

            var fileContent = File.ReadAllText(filePath);

            using var document = JsonDocument.Parse(fileContent);

 
            if (!document.RootElement.TryGetProperty("market_data", out JsonElement items))
                throw new Exception("Invalid JSON format: 'market_data' node missing.");

            var results = new List<PriceRecord>();

            foreach (var element in items.EnumerateArray())
            {
                try
                {
                    string produceName = element.GetProperty("produce_name").GetString() ?? "";
                    string marketName = element.GetProperty("market_name").GetString() ?? "";
                    decimal price = element.GetProperty("price_per_kg").GetDecimal();
                    string dateString = element.GetProperty("date").GetString() ?? "";

                    // If any mandatory value missing skip record
                    if (string.IsNullOrWhiteSpace(produceName) ||
                        string.IsNullOrWhiteSpace(marketName) ||
                        string.IsNullOrWhiteSpace(dateString))
                        continue;

                    if (!DateTime.TryParse(dateString, out DateTime parsedDate))
                        continue;

                    var record = new PriceRecord
                    {
                        ProduceId = element.GetProperty("produce_id").GetString() ?? "",
                        ProduceName = produceName,
                        Category = element.GetProperty("category").GetString() ?? "",   
                        MarketId = element.GetProperty("market_id").GetString() ?? "",
                        MarketName = marketName,
                        Region = element.GetProperty("region").GetString() ?? "",
                        State = element.GetProperty("state").GetString() ?? "",

                        Date = parsedDate,
                        PricePerKg = price,
                        Unit = element.GetProperty("unit").GetString() ?? "kg",

                        QuantityAvailable = element.TryGetProperty("quantity_available", out var q)
                                ? q.GetDecimal()
                                : (decimal?)null,

                        QualityGrade = element.TryGetProperty("quality_grade", out var g)
                                ? g.GetString()
                                : null,

                        Source = element.TryGetProperty("source", out var s)
                                ? s.GetString()
                                : null
                    };

                    results.Add(record);
                }
                catch
                {
                    continue;
                }
            }

            return results;
        }
    }
}

