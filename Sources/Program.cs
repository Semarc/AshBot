using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace AshBot
{
	public static class Program
	{
		public static DiscordSocketClient client;
		public static CommandHandler commandHandler;

		public static LoggingService logger;


		public static void Main() =>
			MainAsync().GetAwaiter().GetResult();

		public static async Task MainAsync()
		{

			DiscordSocketConfig Clientconfig = new DiscordSocketConfig()
			{
				ExclusiveBulkDelete = false,
				LogLevel = LogSeverity.Verbose,
				MessageCacheSize = 100
			};

			CommandServiceConfig CommandConfig = new CommandServiceConfig()
			{
				CaseSensitiveCommands = false,
				LogLevel = LogSeverity.Verbose,
				ThrowOnError = false,
				DefaultRunMode = RunMode.Async,
				IgnoreExtraArgs = true
			};

			client = new DiscordSocketClient(Clientconfig);

			CommandService commandservicer = new CommandService(CommandConfig);

			commandHandler = new CommandHandler(client, commandservicer);

			logger = new LoggingService(client, commandservicer);

			await commandHandler.InstallCommandsAsync();

			// Remember to keep token private or to read it from an 
			// external source! In this case, we are reading the token 
			// from an environment variable. If you do not know how to set-up
			// environment variables, you may find more information on the 
			// Internet or by using other methods such as reading from 
			// a configuration.

			await client.LoginAsync(TokenType.Bot, GlobalConstants.BotToken);
			await client.StartAsync();


			// Block this task until the program is closed.
			await Task.Delay(-1);
		}


	}
}
