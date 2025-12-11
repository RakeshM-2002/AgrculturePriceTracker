
using AgrculturePriceTracker.Core.Models;

namespace AgrculturePriceTracker.Core.Interfaces
{
    public interface IPriceRepository
    {
        void SaveMany(IEnumerable<PriceRecord> records);
        IEnumerable<PriceRecord> GetAll();
        IEnumerable<PriceRecord> GetByProduceAndMarket(string produceName, string marketName, DateTime? since = null);
    }
}
