AgriculturePriceTracker — Console App

Overview
This is a console-based Agricultural Produce Price Tracker & Alert System. It imports price data (JSON/CSV), stores historical records, runs analytics (7-day/30-day averages, volatility), evaluates alert rules, and prints notifications and reports.

What is implemented
JSON/CSV/Xml parsing with validation
File-based JSON repository
Analysis engine (current price, rolling averages, day-over-day percent, std dev, max, min, trend, day to day change)
Simple rule evaluator (threshold & percent_change)
Console notification and alert history file

How to Run
Clone the project
Open in Visual Studio / VS Code
Run the project

Use commands:
import data/sample_price_data.json
analyze Tomato Bangalore APMC
history Tomato Bangalore APMC
alert Tomato Bangalore_APMC 40 above
CheckAlerts
list
help
exit
