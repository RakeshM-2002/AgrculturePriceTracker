
using AgrculturePriceTracker.Core.Models;

namespace AgrculturePriceTracker.Core.Interfaces
{
    public interface IDataParser
    {
        IEnumerable<PriceRecord> Parse(string filePath);
    }
}

