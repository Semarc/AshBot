using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AshBot
{
	class BasicModule : ModuleBase<SocketCommandContext>
	{
		[RequireBotMod]
		[Command("ChangeCommandPrefix")]
		[Summary("Ändert den Präfix, den der Bot für Commands benutzt")]
		public Task ChangeCommandPrefixAsync([Summary("Das neue Präfix für die Commands")]char CommandChar)
		{
			GlobalVariables.CommandPrefix = CommandChar;
			return ReplyAsync($"Command Präfix zu {GlobalVariables.CommandPrefix}");
		}

		[Command("help")]
		[Summary("Listet alle Befehler auf")]
		public Task HelpAsync()
		{
			MemberInfo[] Methodinfos = typeof(BasicModule).GetMethods();

			Methodinfos = Methodinfos.Concat(typeof(BotServerManagmentModule).GetMethods()).ToArray();

			List<CommandDescription> Commands = new List<CommandDescription>();

			foreach (MethodInfo info in Methodinfos)
			{

				CommandAttribute CommandName = (CommandAttribute)info.GetCustomAttribute(typeof(CommandAttribute));
				SummaryAttribute CommandSummary = (SummaryAttribute)info.GetCustomAttribute(typeof(SummaryAttribute));

				if (!(CommandName is null) && !(CommandSummary is null))
				{
					Commands.Add(new CommandDescription { CommandName = CommandName.Text, CommandSummary = CommandSummary.Text });
				}
			}

			int LongestCommand = Commands.Max(x => x.CommandName.Length) + 5;

			StringBuilder Result = new StringBuilder();

			Result.AppendLine("```");
			foreach (CommandDescription description in Commands)
			{
				Result.Append(description.CommandName.PadRight(LongestCommand) + description.CommandSummary).AppendLine();
			}
			Result.AppendLine("```");

			return ReplyAsync(Result.ToString());
		}

		private class CommandDescription
		{
			public string CommandName { get; set; }
			public string CommandSummary { get; set; }

		}

		[Command("ping")]
		[Summary("Pongs")]
		public Task PingAsync()
		{
			return ReplyAsync("Ponggers <:glatt:707922147334553620>");
		}

		[Command("echo")]
		[Summary("Echos")]
		[RequireBotMod]
		public Task EchoAsync([Remainder] string Input)
		{
			Context.Message.DeleteAsync();

			return ReplyAsync(Input);
		}

		[Command("SetNickname")]
		[Summary("Set the Nickname of the bot")]
		[RequireBotMod]
		public Task SetNicknameAsync([Remainder] string NewNickname)
		{
			Context.Guild.CurrentUser.ModifyAsync(x => { x.Nickname = NewNickname; });


			return ReplyAsync($"Nickname Set to: {NewNickname}");

		}



	}


	[Group("Servers")]
	public class BotServerManagmentModule : ModuleBase<SocketCommandContext>
	{

		[Command("List")]
		[Summary("Lists the Servers of the bot")]
		[RequireBotMod]
		public Task SetNicknameAsync()
		{
			List<string> ServerNames = new List<string>();

			foreach (SocketGuild item in Program.client.Guilds)
			{
				ServerNames.Add(item.Name);
			}

			StringBuilder Result = new StringBuilder();

			Result.AppendLine("```");
			foreach (string Servername in ServerNames)
			{
				Result.AppendLine(Servername);
			}
			Result.AppendLine("```");

			return ReplyAsync(Result.ToString());


		}
	}


}
