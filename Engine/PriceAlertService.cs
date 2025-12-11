using AgrculturePriceTracker.Core.Interfaces;
using AgrculturePriceTracker.Core.Models;
using AgriculturalPriceTracker.Core.Models;

namespace AgrculturePriceTracker.Engine
{
    public class PriceAlertService : IAlertService
    {
        private readonly List<AlertRule> _alerts = new();

        public void AddAlert(AlertRule rule)
        {
            _alerts.Add(rule);
            Console.WriteLine("Alert added successfully!");
        }

        public void CheckAlerts(IEnumerable<PriceRecord> latestPrices)
        {
            foreach (var alert in _alerts)
            {
                var match = latestPrices
                    .Where(p =>
                        p.ProduceName.Equals(alert.ProduceName, StringComparison.OrdinalIgnoreCase) &&
                        p.MarketName.Replace(" ", "").ToLower()
                        .Contains(alert.MarketName.Replace(" ", "").ToLower()))
                    .OrderByDescending(p => p.Date)
                    .FirstOrDefault();

                if (match == null)
                    continue;

                if (alert.NotifyWhenAbove && match.PricePerKg > alert.ThresholdPrice)
                {
                    Notify(alert, match);
                }
                else if (!alert.NotifyWhenAbove && match.PricePerKg < alert.ThresholdPrice)
                {
                    Notify(alert, match);
                }
            }
        }

        private void Notify(AlertRule alert, PriceRecord price)
        {
            Console.WriteLine($"\n !! PRICE ALERT !!");
            Console.WriteLine($"Produce: {alert.ProduceName}");
            Console.WriteLine($"Market: {alert.MarketName}");
            Console.WriteLine($"Current Price: ₹{price.PricePerKg}");
            Console.WriteLine($"Threshold: ₹{alert.ThresholdPrice}");
            Console.WriteLine($"Condition: {(alert.NotifyWhenAbove ? "Above" : "Below")}");
            Console.WriteLine("----------------------------------");
        }
    }
}
