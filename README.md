# AgriculturePriceTracker — Console App (Intern Assessment)

## Overview
This is a console-based Agricultural Produce Price Tracker & Alert System. 
It imports price data (JSON/CSV), stores historical records, runs analytics (7-day/30-day averages, volatility), evaluates alert rules, and prints notifications and reports.



## What is implemented
- JSON parsing with validation
- File-based JSON repository
- Analysis engine (current price, rolling averages, day-over-day percent, std dev)
- Simple rule evaluator (threshold & percent_change)
- Console notification and alert history file

## How to Run
1. Clone the project
2. Open in Visual Studio / VS Code
3. Run the project
4. Use commands:

- `import data/sample_price_data.json`
- `analyze Tomato Bangalore_APMC`
- `history Tomato Bangalore_APMC`
- `alert Tomato Bangalore_APMC 40 above`
-  CheckAlerts
- `list`
- `help`
- `exit`
