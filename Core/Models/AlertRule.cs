namespace AgriculturalPriceTracker.Core.Models
{
    public class AlertRule
    {
        public string ProduceName { get; set; } = "";
        public string MarketName { get; set; } = "";
        public decimal ThresholdPrice { get; set; }
        public bool NotifyWhenAbove { get; set; } 
    }
}
