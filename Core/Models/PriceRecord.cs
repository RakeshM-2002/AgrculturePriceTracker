
namespace AgrculturePriceTracker.Core.Models
{
    public class PriceRecord
    {
        public string ProduceId { get; set; } = "";
        public string ProduceName { get; set; } = "";
        public string Category { get; set; } = "";

        public string MarketId { get; set; } = "";
        public string MarketName { get; set; } = "";
        public string Region { get; set; } = "";
        public string State { get; set; } = "";


        public DateTime Date { get; set; }    
        public decimal PricePerKg { get; set; }  
        public string Unit { get; set; } = "kg";    


        public decimal? QuantityAvailable { get; set; }
        public string? QualityGrade { get; set; }
        public string? Source { get; set; }
    }
}

