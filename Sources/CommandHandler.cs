using Discord;
using Discord.Commands;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AshBot
{
	public class CommandHandler
	{
		public static DiscordSocketClient _client;
		public static CommandService _commands;

		public CommandHandler(DiscordSocketClient client, CommandService commands)
		{
			_commands = commands;
			_client = client;
		}

		public async Task InstallCommandsAsync()
		{


			// Hook the MessageReceived event into our command handler
			_client.MessageReceived += HandleCommandAsync;

			_client.MessageDeleted += _client_MessageDeleted;
			_client.MessageUpdated += _client_MessageUpdated;
			_client.ReactionAdded += _client_ReactionAdded;
			_client.ReactionRemoved += _client_ReactionRemoved;

			_commands.AddTypeReader(typeof(System.Numerics.BigInteger), new BigIntegerTypeReader());
			_commands.AddTypeReader(typeof(IEmote), new EmojiTypereader());

			// Here we discover all of the command modules in the entry 
			// assembly and load them. Starting from Discord.NET 2.0, a
			// service provider is required to be passed into the
			// module registration method to inject the 
			// required dependencies.
			//
			// If you do not use Dependency Injection, pass null.
			// See Dependency Injection guide for more information.

			await _commands.AddModuleAsync<BasicModule>(null);
			await _commands.AddModuleAsync<ListSortModule>(null);
			await _commands.AddModuleAsync<BotServerManagmentModule>(null);
			await _commands.AddModuleAsync<MiscCommandsModlue>(null);
			await _commands.AddModuleAsync<ReminderModule>(null);

			//await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
			//                                services: null);
		}

		private Task _client_ReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
		{
			if (GlobalConstants.ValidModIds.Contains(arg3.User.Value.Id))
			{
				return arg1.Value.RemoveReactionAsync(arg3.Emote, _client.CurrentUser);
			}
			return Task.CompletedTask;
		}

		private Task _client_ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
		{
			if (GlobalConstants.ValidModIds.Contains(arg3.User.Value.Id))
			{
				return arg1.Value.AddReactionAsync(arg3.Emote);
			}
			return Task.CompletedTask;
		}

		private async Task _client_MessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
		{
			IMessage message = await arg1.GetOrDownloadAsync();

			if (message is null == false)
			{
				Console.WriteLine($"{DateTime.Now.ToLongTimeString()} Message by {arg2.Author} Edditet in #{arg3.Name} (Id: {arg3.Id}) Old: {message.Content}, New: {arg2.Content}");
			}
		}

		private async Task _client_MessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
		{
			IMessage message = await arg1.GetOrDownloadAsync();

			if (message is null == false)
			{
				Console.WriteLine($"{DateTime.Now.ToLongTimeString()} Message by {message.Author} Deleted in #{message.Channel.Name} (Id: {message.Channel.Id}) Messagetext: {message.Content}");
			}
		}

		private async Task HandleCommandAsync(SocketMessage messageParam)
		{
			// Don't process the command if it was a system message
			if (!(messageParam is SocketUserMessage message))
				return;

			string lowermessagestring = message.Content.ToLower();

			string trimmed = lowermessagestring.Trim();

			List<IEmote> ReactionEmotes = new List<IEmote>();

			if (message.Author.IsBot == false)
			{

				foreach (KeyValuePair<string, string> valuepairs in GlobalConstants.Answers)
				{
					if (lowermessagestring.Contains(valuepairs.Key))
					{
						await messageParam.Channel.SendMessageAsync(valuepairs.Value);
					}
				}
				foreach (KeyValuePair<string, string> valuepairs in GlobalConstants.WholeThings)
				{
					if (trimmed == valuepairs.Key)
					{
						await messageParam.Channel.SendMessageAsync(valuepairs.Value);
					}
				}

				if (lowermessagestring.StartsWith("+++"))
				{
					await messageParam.Channel.SendMessageAsync("Ja Moin " + GlobalConstants.Glatt);
					return;
				}


				if (message.Author.Id == GlobalConstants.CactuzId && message.Content.StartsWith(@"https://tenor.com/view/") && !lowermessagestring.Contains("ahsoka"))
				{
					await messageParam.Channel.SendMessageAsync(message.Content);
				}
			}

			// Create a number to track where the prefix ends and the command begins
			int argPos = 0;

			// Determine if the message is a command based on the prefix and make sure no bots trigger commands
			if (!(message.HasCharPrefix(GlobalVariables.CommandPrefix, ref argPos) ||
				message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
				message.Author.IsBot)
				return;

			// Create a WebSocket-based command context based on the message
			var context = new SocketCommandContext(_client, message);

			// Execute the command with the command context we just
			// created, along with the service provider for precondition checks.

			// Keep in mind that result does not indicate a return value
			// rather an object stating if the command executed successfully.
			IResult result = await _commands.ExecuteAsync(
				context: context,
				argPos: argPos,
				services: null);

			// Optionally, we may inform the user if the command fails
			// to be executed; however, this may not always be desired,
			// as it may clog up the request queue should a user spam a
			// command.
			if (!result.IsSuccess)
				await context.Channel.SendMessageAsync(result.ErrorReason);
		}
	}
}
