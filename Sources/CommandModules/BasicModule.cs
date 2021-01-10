using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AshBot
{
	public class BasicModule : ModuleBase<SocketCommandContext>
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
		[Summary("Listet alle Befehle auf")]
		public Task HelpAsync()
		{
			MemberInfo[] Methodinfos = typeof(BasicModule).GetMethods();

			Methodinfos = Methodinfos.Concat(typeof(BotServerManagmentModule).GetMethods()).ToArray();

			List<CommandDescription> Commands = new List<CommandDescription>();

			foreach (MethodInfo info in Methodinfos)
			{
				GroupAttribute GroupName = (GroupAttribute)info.DeclaringType.GetCustomAttribute(typeof(GroupAttribute));

				CommandAttribute CommandName = (CommandAttribute)info.GetCustomAttribute(typeof(CommandAttribute));
				SummaryAttribute CommandSummary = (SummaryAttribute)info.GetCustomAttribute(typeof(SummaryAttribute));

				if (!(CommandName is null) && !(CommandSummary is null))
				{
					Commands.Add(new CommandDescription { Groupname = GroupName?.Prefix ?? string.Empty, CommandName = CommandName.Text, CommandSummary = CommandSummary.Text });
				}
			}

			int LongestCommand = Commands.Max(x => x.Groupname.Length + x.CommandName.Length) + 5;

			StringBuilder Result = new StringBuilder();

			Result.AppendLine("```");
			foreach (CommandDescription description in Commands)
			{
				Result.Append($"{description.Groupname} {description.CommandName}".PadRight(LongestCommand) + description.CommandSummary).AppendLine();
			}
			Result.AppendLine("```");

			return ReplyAsync(Result.ToString());
		}

		private class CommandDescription
		{
			public string Groupname { get; set; }
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

		[Command("yeet")]
		[Summary("Yeets")]
		public Task ThrowAsync([Remainder] string ToBeYeeted)
		{
			return ReplyAsync($"I yeeted {ToBeYeeted}");
		}

		[Command("SetPicture")]
		[Summary("Set the Profilepicture of the bot")]
		[RequireBotMod]
		public Task SetPrifilePictureAsync()
		{
			IReadOnlyCollection<Attachment> attachs = Context.Message.Attachments;

			if (attachs.Count >= 1)
			{
				HttpClient client = default;
				try
				{
					client = new HttpClient();
					Attachment attach = attachs.First();

					Stream stream = client.GetStreamAsync(attach.Url).GetAwaiter().GetResult();

					Image image = new Image(stream);

					Context.Client.CurrentUser.ModifyAsync(x => { x.Avatar = image; });
					return ReplyAsync("Avatar set");
				}
				finally
				{
					client.Dispose();
				}
			}
			else
			{
				return ReplyAsync("Kein gültiges Profilbild gefunden");
			}
		}

	}


	[Group("servers")]
	public class BotServerManagmentModule : ModuleBase<SocketCommandContext>
	{

		[Command("list")]
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
