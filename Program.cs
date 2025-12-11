using AgrculturePriceTracker.CLI;
using AgrculturePriceTracker.Core.Interfaces;
using AgrculturePriceTracker.Data.Parsers;
using AgrculturePriceTracker.Data.Repositories;

class Program
{
    static void Main()
    {
        IPriceRepository repo = new JsonFilePriceRepository("data/prices.json");
        
        var cli = new CommandProcessor(repo);
        cli.Start();

    }
}
