using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AshBot
{
	public class CommandHandler
	{
		public readonly DiscordSocketClient _client;
		public readonly CommandService _commands;

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

			//_client.MessageReceived += RumSpammer;




			// Here we discover all of the command modules in the entry 
			// assembly and load them. Starting from Discord.NET 2.0, a
			// service provider is required to be passed into the
			// module registration method to inject the 
			// required dependencies.
			//
			// If you do not use Dependency Injection, pass null.
			// See Dependency Injection guide for more information.

			await _commands.AddModuleAsync<BasicModule>(null);
			await _commands.AddModuleAsync<BotServerManagmentModule>(null);

			//await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
			//                                services: null);
		}

		private async Task _client_MessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
		{
			IMessage message = await arg1.GetOrDownloadAsync();

			if (message is null == false)
			{
				Console.WriteLine($"Message by {message.Author} Deleted in {message.Channel.Name} (Id: {message.Channel.Id}) Messagetext: {message.Content}");
			}
		}

		private async Task HandleCommandAsync(SocketMessage messageParam)
		{
			// Don't process the command if it was a system message
			if (!(messageParam is SocketUserMessage message))
				return;

			string messagestring = message.Content.ToLower();

			List<IEmote> ReactionEmotes = new List<IEmote>();

			if (message.Author.IsBot == false)
			{
				if (messagestring.Contains("senat"))
				{
					await messageParam.Channel.SendMessageAsync("https://tenor.com/view/senate-palpatine-star-wars-gif-5129779");
				}
				if (messagestring.Trim() == "hello there")
				{
					await messageParam.Channel.SendMessageAsync("https://tenor.com/view/hello-there-general-kenobi-star-wars-grevious-gif-17774326");
				}
				if (messagestring.Contains("democracy"))
				{
					await messageParam.Channel.SendMessageAsync("https://tenor.com/view/star-wars-democracy-i-love-democracy-gif-13935227");
				}
				if (messagestring.Trim() == "execute order 66")
				{
					await messageParam.Channel.SendMessageAsync("It will be done my Lord");
					//await messageParam.Channel.SendFileAsync(StreamFromBitmap(Properties.Resources.ExecuteOrder66), "ExecuteOrder66.gif");
				}
				if (messagestring.Trim() == "execute order 69")
				{
					await messageParam.Channel.SendMessageAsync("Luke, did I ever tell you about Ahsoka Tano? She was your father’s exotic teenage alien apprentice, a fine piece of jailbait from a more civilized age. She had the tightest body and the perkiest little breasts in the galaxy; barely legal in most systems. Anakin and I used to doubleteam her at the end of every successful campaign during the Clone Wars, and once in a while we’d even have the entire 501st run a train over her, part of official Jedi “training” of course. In time, she learned how to handle a meatsaber better than anyone in the Jedi Temple. She wore a miniskirt every day so we told her there were no panties in space, and since she was constantly doing acrobatics you’d get a glimpse of her orange pussy mid fight as she’d do a flip while slicing a B2 Super Battledroid in half. It was surreal. We taught her to grip her weapon backwards like a dildo and she constantly got captured by pirates and slavers almost every other day. It was ridiculous, like a constant porno Luke, you have no idea. And she was a good friend. And then there was that time somebody drilled a glory hole into the Temple's 3rd level bathrooms. We never did figure out who put it there but we sure made great use of it. Handjobs, blowjobs, anal, hell, I might have even fucked some orfices human's don't even have. The bathroom was unisex, so I'm fairly certain some dudes slipped it in, but I didn't really care. All I did was picture Satine and-- oh, I haven't told you about Satine have I, Luke? She was the duchess of Mandalore, though she was a more of a 'Mandawhore', if you get my drift...");
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
