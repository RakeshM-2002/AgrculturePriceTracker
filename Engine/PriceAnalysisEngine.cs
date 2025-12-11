
using AgrculturePriceTracker.Core.Models;
using AgrculturePriceTracker.Core.Interfaces;

namespace AgrculturePriceTracker.Engine
{
    public class PriceAnalysisEngine
    {
        private readonly IPriceRepository _repository;

        public PriceAnalysisEngine(IPriceRepository repository)
        {
            _repository = repository;
        }
        public PriceAnalysisResult Analyze(string produceName, string marketName)
        {
            var data = _repository
                .GetByProduceAndMarket(produceName, marketName)
                .OrderBy(r => r.Date)
                .ToList();

            if (data.Count < 2)
                return new PriceAnalysisResult { HasSufficientData = false };

            var prices = data.Select(r => r.PricePerKg).ToList();

            decimal latest = prices.Last();
            decimal previous = prices[prices.Count - 2];

            decimal dayChange = latest - previous;
            decimal dayChangePercent = (previous == 0) ? 0 : (dayChange / previous) * 100;

            decimal avg7 = CalculateMovingAverage(prices, 7);

            decimal avg30 = CalculateMovingAverage(prices, 30);

            decimal min = prices.Min();
            decimal max = prices.Max();
            decimal sd = CalculateStandardDeviation(prices);

            string trend = GetTrendDirection(dayChange, avg7, latest);

            string summary = GenerateSummaryText(produceName, marketName, latest, dayChange, dayChangePercent, trend);

            return new PriceAnalysisResult
            {
                HasSufficientData = true,
                LatestPrice = latest,
                PreviousPrice = previous,
                DayChange = dayChange,
                DayChangePercent = Math.Round(dayChangePercent, 2),
                Average7 = Math.Round(avg7, 2),
                Average30 = Math.Round(avg30, 2),
                Minimum = min,
                Maximum = max,
                StandardDeviation = Math.Round(sd, 2),
                Trend = trend,
                Summary = summary,
                FullHistory = data
            };
        }

        private decimal CalculateMovingAverage(List<decimal> prices, int days)
        {
            if (prices.Count < days)
                return prices.Average();

            return prices.TakeLast(days).Average();
        }

        private decimal CalculateStandardDeviation(List<decimal> prices)
        {
            var avg = prices.Average();
            var variance = prices.Average(v => (v - avg) * (v - avg));
            return (decimal)Math.Sqrt((double)variance);
        }

        private string GetTrendDirection(decimal dayChange, decimal avg7, decimal latest)
        {
            if (Math.Abs(dayChange) < 0.1m)
                return "Stable";

            if (latest > avg7)
                return "Increasing";

            if (latest < avg7)
                return "Decreasing";

            return "Stable";
        }

        private string GenerateSummaryText(
            string produceName, string marketName,
            decimal latest, decimal dayChange, decimal changePercent,
            string trend)
        {
            string direction = trend == "Increasing" ? "up" :
                               trend == "Decreasing" ? "down" : "stable";

            return
                $"{produceName} price in {marketName} is currently ₹{latest}. " +
                $"It is {direction}, with a daily change of {dayChange} ({Math.Round(changePercent, 2)}%).";
        }
    }
    public class PriceAnalysisResult
    {
        public bool HasSufficientData { get; set; }

        public decimal LatestPrice { get; set; }
        public decimal PreviousPrice { get; set; }
        public decimal DayChange { get; set; }
        public decimal DayChangePercent { get; set; }
        public decimal Average7 { get; set; }
        public decimal Average30 { get; set; }
        public decimal Minimum { get; set; }
        public decimal Maximum { get; set; }
        public decimal StandardDeviation { get; set; }
        public string Trend { get; set; } = "";
        public string Summary { get; set; } = "";

        public List<PriceRecord> FullHistory { get; set; } = new();
    }
}
