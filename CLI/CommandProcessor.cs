
using AgrculturePriceTracker.Core.Interfaces;
using AgrculturePriceTracker.Data.Parsers;
using AgrculturePriceTracker.Engine;
using AgriculturalPriceTracker.Core.Models;


namespace AgrculturePriceTracker.CLI
{
    public class CommandProcessor
    {
        private readonly IPriceRepository _repository;
        private readonly IAlertService _alertService;


        public CommandProcessor(IPriceRepository repository)
        {
            _repository = repository;
            _alertService = new PriceAlertService();

        }

        public void Start()
        {
            Console.WriteLine("### Agricultural Price Tracker  ###");
            Console.WriteLine("Type 'help' to see available commands.");
            Console.WriteLine("***--------------------------------***");

            while (true)
            {
                Console.Write("\n> ");
                string? input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                var parts = input.Split(' ');
                string command = parts[0].ToLower();


                switch (command)
                {
                    case "help":
                        ShowHelp();
                        break;

                    case "import":
                        if (parts.Length < 2)
                            Console.WriteLine("Usage: import <filepath>");
                        else
                            ImportData(parts[1]);
                        break;

                    case "analyze":
                        if (parts.Length < 3)
                            Console.WriteLine("Usage: analyze <produce> <market>");
                        else
                        {
                            var produce = parts[1];
                            var market = string.Join(" ", parts.Skip(2));
                            Analyze(produce, market);
                        }
                        break;

                    case "history":
                        if (parts.Length < 3)
                            Console.WriteLine("Usage: history <produce> <market>");
                        else
                        {
                            var produce = parts[1];
                            var market = string.Join(" ", parts.Skip(2));
                            ShowHistory(produce, market);
                        }

                            break;
                    case "list":
                        ListAll();
                        break;

                    case "alert":
                        if (parts.Length < 4)
                            Console.WriteLine("Usage: alert <produce> <market> <price> [above/below]");
                        else
                            AddAlert(parts);
                        break;

                    case "checkalerts":
                        CheckAlertsManually();
                        break;


                    case "exit":
                        Console.WriteLine("Exiting application...");
                        return;

                    default:
                        Console.WriteLine("Unknown command. Type 'help' for usage.");
                        break;
                }
            }
        }

        private void ShowHelp()
        {
            Console.WriteLine("\nAvailable Commands:");
            Console.WriteLine(" import <file>           - Import price data from JSON file");
            Console.WriteLine(" analyze <produce> <market> - Show price analysis");
            Console.WriteLine(" history <produce> <market> - Show full price history");
            Console.WriteLine(" exit                    - Exit the app");
        }

        private void ImportData(string file)
        {
            try
            {
                IDataParser parser = GetParserForFile(file);
                var records = parser.Parse(file);

                _repository.SaveMany(records);

                Console.WriteLine($"Imported {records.Count()} records successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Import failed: {ex.Message}");
            }
        }


        private void Analyze(string produce, string market)
        {

            var engine = new PriceAnalysisEngine(_repository);
            var result = engine.Analyze(produce, market);

            if (!result.HasSufficientData)
            {
                Console.WriteLine("Not enough data for analysis.");
                return;
            }

            Console.WriteLine("\n### Analysis Result ###");
            Console.WriteLine($"Produce       : {produce}");
            Console.WriteLine($"Market        : {market}");
            Console.WriteLine($"Latest Price  : ₹{result.LatestPrice}");
            Console.WriteLine($"Daily Change  : {result.DayChange} ({result.DayChangePercent}%)");
            Console.WriteLine($"7-day Avg     : {result.Average7}");
            Console.WriteLine($"30-day Avg    : {result.Average30}");
            Console.WriteLine($"Min/Max       : {result.Minimum} / {result.Maximum}");
            Console.WriteLine($"Std Dev       : {result.StandardDeviation}");
            Console.WriteLine($"Trend         : {result.Trend}");
            Console.WriteLine("\nSummary:");
            Console.WriteLine(result.Summary);
        }

        private void ShowHistory(string produce, string market)
        {
            var records = _repository.GetByProduceAndMarket(produce, market);

            Console.WriteLine($"\n### Price History for {produce} in {market} ###");

            foreach (var r in records)
            {
                Console.WriteLine($"{r.Date:yyyy-MM-dd}  ->  ₹{r.PricePerKg}  ->  {r.QualityGrade}");
            }
        }
        private void ListAll()
        {
            var all = _repository.GetAll();

            Console.WriteLine("\n### All Stored Records ###");

            foreach (var r in all)
            {
                Console.WriteLine($"{r.ProduceName} | {r.MarketName} | {r.Date:yyyy-MM-dd} | ₹{r.PricePerKg}");
            }
        }

        private IDataParser GetParserForFile(string file)
        {
            string ext = Path.GetExtension(file).ToLower();
            
            Console.WriteLine(ext);

            switch (ext)
            {
                case ".json":
                    return new JsonParser();
                case ".csv":
                    return new CsvParser();
                default:
                    throw new Exception("Unsupported file format");
            }

        }

        private void AddAlert(string[] parts)
        {
            string produce = parts[1];
            string market = parts[2];
            decimal price = decimal.Parse(parts[3]);

            bool above = true; 

            if (parts.Length == 5)
                above = parts[4].ToLower() == "above";

            var alert = new AlertRule
            {
                ProduceName = produce,
                MarketName = market,
                ThresholdPrice = price,
                NotifyWhenAbove = above
            };

            _alertService.AddAlert(alert);
        }

        private void CheckAlertsManually()
        {
            var allRecords = _repository.GetAll().ToList();

            if (!allRecords.Any())
            {
                Console.WriteLine("No data loaded. Please import data first.");
                return;
            }
            _alertService.CheckAlerts(allRecords);

            Console.WriteLine("Alert message is Done!!!");
        }



    }
}
