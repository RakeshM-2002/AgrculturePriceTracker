using System.Text.Json;
using AgrculturePriceTracker.Core.Interfaces;
using AgrculturePriceTracker.Core.Models;

namespace AgrculturePriceTracker.Data.Repositories
{
    public class JsonFilePriceRepository : IPriceRepository
    {
        private readonly string _filePath;

        public JsonFilePriceRepository(string filePath)
        {
            _filePath = filePath;

            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
        }

        public void SaveMany(IEnumerable<PriceRecord> newRecords)
        {

            var existingRecords = GetAll().ToList();

            var combined = existingRecords.Concat(newRecords);

            var cleaned = combined
                .GroupBy(r =>
                    $"{r.ProduceName.ToLower()}|" +
                    $"{r.MarketName.ToLower()}|" +
                    $"{r.Date:yyyy-MM-dd}"
                )
                .Select(g => g.First())  // keep the first valid record
                .OrderBy(r => r.ProduceName)
                .ThenBy(r => r.MarketName)
                .ThenBy(r => r.Date)
                .ToList();

            var json = JsonSerializer.Serialize(cleaned, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public IEnumerable<PriceRecord> GetAll()
        {
            try
            {
                var json = File.ReadAllText(_filePath);
                var records = JsonSerializer.Deserialize<List<PriceRecord>>(json);
                return records ?? new List<PriceRecord>();
            }
            catch
            {

                return new List<PriceRecord>();
            }
        }

        public IEnumerable<PriceRecord> GetByProduceAndMarket(string produceName, string marketName, DateTime? since = null)
        {
            string normalizedProduce = produceName.Replace(" ", "").Trim().ToLower();
            string normalizedMarket = marketName.Replace(" ", "").Trim().ToLower();

            var records = GetAll()
                .Where(r =>
                    r.ProduceName.Replace(" ", "").Trim().ToLower() == normalizedProduce &&
                    r.MarketName.Replace(" ", "").Trim().ToLower() == normalizedMarket
                );

            if (since.HasValue)
                records = records.Where(r => r.Date >= since.Value);

            return records.OrderBy(r => r.Date);
        }

    }
}

