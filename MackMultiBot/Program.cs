using Microsoft.EntityFrameworkCore;
using MackMultiBot;
using MackMultiBot.Bancho;
using MackMultiBot.Database;
using MackMultiBot.Logging;

Console.Title = "BotLogger";

Logger.Log(LogLevel.MackMulti, "--------------------------------------------------------------------------------", ConsoleColor.White);
Logger.Log(LogLevel.MackMulti, "    __  ___              __    __  ___        __ __   _ ", ConsoleColor.Green);
Logger.Log(LogLevel.MackMulti, "   /  |/  /____ _ _____ / /__ /  |/  /__  __ / // /_ (_)", ConsoleColor.Green);
Logger.Log(LogLevel.MackMulti, "  / /|_/ // __ `// ___// //_// /|_/ // / / // // __// / ", ConsoleColor.Green);
Logger.Log(LogLevel.MackMulti, " / /  / // /_/ // /__ / ,<  / /  / // /_/ // // /_ / /", ConsoleColor.Green);
Logger.Log(LogLevel.MackMulti, @"/_/  /_/ \__,_/ \___//_/|_|/_/  /_/ \__,_//_/ \__//_/", ConsoleColor.Green);
Logger.Log(LogLevel.MackMulti, "");
Logger.Log(LogLevel.MackMulti, "--------------------------------------------------------------------------------", ConsoleColor.White);
Logger.Log(LogLevel.MackMulti, "Bot Version: v1.0", ConsoleColor.DarkCyan);
Logger.Log(LogLevel.MackMulti, "Report any issues you encounter to me through discord @mackosu", ConsoleColor.DarkCyan);
Logger.Log(LogLevel.MackMulti, "--------------------------------------------------------------------------------", ConsoleColor.White);

string configPath = "config.txt";
if (!File.Exists(configPath))
    configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
if (!File.Exists(configPath))
    configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../config.txt");
if (!File.Exists(configPath))
    configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../config.txt");

Logger.Log(LogLevel.MackMulti, "Reading config file...", ConsoleColor.White);
var config = ConfigReader.ReadConfig(configPath);

Logger.Log(LogLevel.MackMulti, "Initializing database...", ConsoleColor.White);
BotDatabaseContext.ConnectionString = $"Data Source={config.DatabaseDirectory}/data.db";

if (!Directory.Exists(config.DatabaseDirectory))
    Directory.CreateDirectory(config.DatabaseDirectory);

using (var context = new BotDatabaseContext())
{
    context.Database.Migrate();
}

Logger.Log(LogLevel.MackMulti, "Initializing log file...", ConsoleColor.White);
Logger.LogFilePath = $"{config.LogDirectory}/Log.txt";

Logger.Log(LogLevel.MackMulti, "Starting Bot...", ConsoleColor.White);
Logger.Log(LogLevel.MackMulti, "--------------------------------------------------------------------------------", ConsoleColor.White);
Bot Bot = new(config);
await Bot.StartAsync();



Logger.Log(LogLevel.MackMulti, "--- Interactive CLI ---", ConsoleColor.Yellow);
Logger.Log(LogLevel.MackMulti, "Type messages to send to the lobby. Use /exit to quit.", ConsoleColor.Yellow);

while (true)
{
    string line = Console.ReadLine();
    if (line == null) break;
    if (line == "/exit") break;
    if (string.IsNullOrWhiteSpace(line)) continue;

    if (Bot.Lobby?.ChannelId != null && !string.IsNullOrEmpty(Bot.Lobby.ChannelId))
    {
        Bot.BanchoConnection.MessageHandler.SendMessage(Bot.Lobby.ChannelId, line);
    }
    else
    {
        Logger.Log(LogLevel.Warn, "Lobby not joined yet. Message not sent.");
    }
}

