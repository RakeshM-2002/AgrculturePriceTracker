using AgrculturePriceTracker.Core.Models;
using AgriculturalPriceTracker.Core.Models;

namespace AgrculturePriceTracker.Core.Interfaces
{
    public interface IAlertService
    {
        void AddAlert(AlertRule rule);
        void CheckAlerts(IEnumerable<PriceRecord> latestPrices);
    }
}
