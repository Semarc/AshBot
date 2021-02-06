using Discord;
using Discord.Commands;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Timers;

namespace AshBot
{
	public class BasicModule : ModuleBase<SocketCommandContext>
	{
		[RequireBotMod]
		[Command("ChangeCommandPrefix")]
		[Summary("Ändert den Präfix, den der Bot für Commands benutzt")]
		public Task ChangeCommandPrefixAsync([Summary("Das neue Präfix für die Commands")] char CommandChar)
		{
			GlobalVariables.CommandPrefix = CommandChar;
			return ReplyAsync($"Command Präfix zu {GlobalVariables.CommandPrefix}");
		}

		[Command("help")]
		[Summary("Listet alle Befehle auf")]
		public Task HelpAsync()
		{

			//List<CommandInfo> commandInfos = new List<CommandInfo>();

			//foreach (ModuleInfo item in CommandHandler._commands.Modules)
			//{
			//	commandInfos.AddRange(item.GetExecutableCommandsAsync(Context, null).GetAwaiter().GetResult());
			//}


			//List<CommandDescription> Commands = new List<CommandDescription>();

			//foreach (CommandInfo info in commandInfos)
			//{
			//	info.Attributes
			//}


			MemberInfo[] Methodinfos = typeof(BasicModule).GetMethods();



			Methodinfos = Methodinfos.Concat(typeof(BotServerManagmentModule).GetMethods()).ToArray();
			Methodinfos = Methodinfos.Concat(typeof(ListSortModule).GetMethods()).ToArray();
			Methodinfos = Methodinfos.Concat(typeof(MiscCommandsModlue).GetMethods()).ToArray();
 

			Methodinfos = Methodinfos.Where(x => ((MethodBase)x).IsPublic).ToArray();

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



			EmbedBuilder builder = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder()
				{
					Name = "AutorName",
					Url = "https://www.emoji.co.uk/files/twitter-emojis/symbols-twitter/11121-negative-squared-latin-capital-letter-a.png"
				},
				Color = Color.Purple,
				ImageUrl = Properties.AshSettings.Default.ProfilePicture,
				Title = "Hilfe",
				Fields = new List<EmbedFieldBuilder>()
			};


			foreach (CommandDescription description in Commands)
			{
				builder.AddField($"{description.Groupname} {description.CommandName}", description.CommandSummary, false);
			}

			return ReplyAsync(embed: builder.Build());
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
			return ReplyAsync($"Ponggers {GlobalConstants.Glatt}");
		}

		[Command("echo")]
		[Alias("say")]
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

		[Command("Big")]
		[Summary("Makes the letters BIG. Only letters as Input though")]
		[RequireBotMod]
		public Task MakeBigAsync([Remainder] string toBeBigged)
		{
			if (Regex.IsMatch(toBeBigged, @"^[a-zA-Z ]+$"))
			{
				Context.Message.DeleteAsync();
				StringBuilder builder = new StringBuilder(toBeBigged.ToLowerInvariant());
				builder.Replace(" ", "    ");
				foreach (KeyValuePair<string, string> valuePair in GlobalConstants.RegionalIndicatorStrings)
				{
					builder.Replace(valuePair.Key, valuePair.Value + " ");
				}
				return ReplyAsync(builder.ToString());
			}
			else
			{
				return ReplyAsync("Nur Buchstaben du Affe (Keine Umlaute du Klugscheißer)");
			}

		}


		[Command("warn")]
		[Summary("Warns a user")]
		[RequireBotMod]
		public Task WarnAsync(IUser ToBeWarned, [Remainder] string Reason)
		{
			Console.WriteLine($"Warned {ToBeWarned.Username} for {Reason}");

			ToBeWarned.SendMessageAsync($"You have been warned by {Context.Message.Author.Username} for {Reason}");

			return ReplyAsync($"I warned <@{ToBeWarned.Id}> for {Reason}");
		}

		[Command("pyramid")]
		[Summary("Creates an Emoji Pyramid")]
		public Task CreatePyramidAsync(IEmote emote, int size = 3)
		{
			if (GlobalConstants.AllowedSpamChannels.Contains(Context.Channel.Id) == false && size > 3)
			{
				return ReplyAsync("Außerhalb von Spamkanälen ist eine Größe von Maximal 3 erlaubt");
			}
			if (size > 100)
			{
				return ReplyAsync("Nicht so groß meine Fresse");
			}
			string Returner = buildPyramid(emote.ToString(), size);
			if (Returner.Length > 2000)
			{
				ReplyAsync($"Die Pyramide ist {Returner.Length} Zeichen lang, sie darf aber maximal 2000 Zeichen lang sein, bitte reduziere die Größe");
			}
			return ReplyAsync(Returner);
		}

		private string buildPyramid(string input, int size)
		{
			input = input + " ";
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i <= size; i++)
			{
				for (int j = 1; i >= j; j++)
				{
					builder.Append(input);
				}
				builder.AppendLine();
			}
			for (int i = size - 1; i > 0; i--)
			{
				for (int j = i; 1 <= j; j--)
				{
					builder.Append(input);
				}
				builder.AppendLine();
			}
			return builder.ToString();
		}

		[Command("SendMessage")]
		[Summary("Sends a User a message")]
		[RequireBotMod]
		public Task SendMessageAsync(IUser ToBeSent, [Remainder] string Message)
		{
			ToBeSent.SendMessageAsync(Message);

			return Task.CompletedTask;
		}

		[Command("SetPicture")]
		[Summary("Set the Profilepicture of the bot")]
		[RequireBotMod]
		public Task SetProfilePictureAsync()
		{
			IReadOnlyCollection<Attachment> attachs = Context.Message.Attachments;

			if (attachs.Count >= 1)
			{
				Attachment attach = attachs.First();
				SetProfilePicture(Utilities.ImageFromUrl(attach.Url));
				return ReplyAsync("Avatar set");
			}
			else
			{
				return ReplyAsync("Kein gültiges Profilbild gefunden");
			}
		}

		private void SetProfilePicture(Image image)
		{
			Context.Client.CurrentUser.ModifyAsync(x => { x.Avatar = image; });
		}

		[Command("SetNickname")]
		[Summary("Set the Profilepicture of the bot")]
		[RequireBotMod]
		public Task SetServerNicknameAsync([Remainder] string NewNickname)
		{
			if (NewNickname.Length > 30)
			{
				return ReplyAsync("Der Nickname darf maximal 30 Zeichen lang sein");
			}
			SetNickname(NewNickname);
			return ReplyAsync($"Nickname Set to {NewNickname}");
		}

		private void SetNickname(string NewNickname)
		{
			Context.Client.GetGuild(Context.Guild.Id).GetUser(Context.Client.CurrentUser.Id).ModifyAsync(x => { x.Nickname = NewNickname; });
		}

		[Command("Clone")]
		[Summary("Copies a Users Nickname and Profile Picture")]
		[RequireBotMod]
		public Task CloneAsync(IUser ToBeCopied)
		{
			SocketGuildUser ToBeCopiedUser = Context.Client.GetGuild(Context.Guild.Id).GetUser(ToBeCopied.Id);

			SetNickname(ToBeCopiedUser.Nickname);
			SetProfilePicture(Utilities.ImageFromUrl(ToBeCopiedUser.GetAvatarUrl()));

			return ReplyAsync($"Cloned {ToBeCopiedUser.Nickname}");
		}

		[Command("Reset")]
		[Summary("Resets the Profilepicture and Nickname of the Bot")]
		[RequireBotMod]
		public Task ResetAsync()
		{
			SetProfilePicture(Utilities.ImageFromUrl(Properties.AshSettings.Default.DefaultImage));
			SetNickname(Context.Client.CurrentUser.Username);
			return ReplyAsync("Avatar and Nickname Reset");
		}


		[Command("Shutdown")]
		[Summary("Shuts the Bot down")]
		[RequireBotMod]
		public Task ShutdownAsync()
		{
			ReplyAsync("Shutting Down...");
			Environment.Exit(0);
			return ReplyAsync("How the Fuck did this Code get executed");
			;
		}

		[Command("delete", RunMode = RunMode.Async)]
		[Summary("Deletes the Last message")]
		[RequireBotMod]
		public Task DeleteAsync()
		{
			IAsyncEnumerable<IReadOnlyCollection<IMessage>> messages = Context.Channel.GetMessagesAsync(10);

			ValueTask<List<IReadOnlyCollection<IMessage>>> messageListListvalueTask = messages.ToListAsync();

			while (messageListListvalueTask.IsCompleted != true)
			{ }

			List<IReadOnlyCollection<IMessage>> messageListList = messageListListvalueTask.GetAwaiter().GetResult();



			IMessage temp = default;

			foreach (IReadOnlyCollection<IMessage> messageList in messageListList)
			{
				foreach (IMessage item in messageList)
				{
					if (item.Author.Id == Context.Client.CurrentUser.Id)
					{
						temp = item;
					}
					ReplyAsync(item.Content);
				}
				ReplyAsync(messageList.Count.ToString());
			}
			if (temp is not null)
			{
				temp.DeleteAsync();
			}
			else
			{
				return ReplyAsync("No Message to Delete found");
			}

			temp = ReplyAsync("Deleted the Last message").GetAwaiter().GetResult();
			;
			System.Threading.Thread.Sleep(5000);
			return temp.DeleteAsync();
		}

	}


	[Group("sort")]
	public class ListSortModule : ModuleBase<SocketCommandContext>
	{

		[Priority(0)]
		[Command("bogo")]
		[Summary("BogoSorts the Array of Strings")]
		public Task BogoSortAsync(params string[] input)
		{
			return BogoSort(input);
		}

		[Priority(2)]
		[Command("bogo")]
		[Summary("BogoSorts the Array of Integers")]
		public Task BogoSortAsync(params System.Numerics.BigInteger[] input)
		{
			return BogoSort(input);
		}
		[Priority(1)]
		[Command("bogo")]
		[Summary("BogoSorts the Array of Floats between -79,228,162,514,264,337,593,543,950,335 and 79,228,162,514,264,337,593,543,950,335")]
		public Task BogoSortAsync(params decimal[] input)
		{
			return BogoSort(input);
		}

		private Task BogoSort<T>(T[] Input) where T : IComparable
		{
			if (Input.Length > 6)
			{
				return ReplyAsync("Nicht so viele Elemente du Affe");
			}

			ReplyAsync("Sorting...");
			var tobedisposed = Context.Channel.EnterTypingState();
			ulong Randomizingns = 0;
			if (Input.Length == 0)
			{
				return ReplyAsync("Es muss mindestens ein Parameter angegeben sein");
			}
			while (true)
			{
				Input = ShuffleArray(Input);
				if (CheckIfSorted(Input))
				{
					break;
				}
				else
				{
					Input = ShuffleArray(Input);
					Randomizingns++;
				}
			}
			tobedisposed.Dispose();
			return ReplyAsync(string.Join(" ", Input) + $"{Environment.NewLine}The List was Bogosorted, is was shuffeled {Randomizingns} times");
		}

		[Command("quantumBogo")]
		[Summary("QuantumBogoSorts the Array of Strings")]
		public Task QuantumBogoSortAsync(params string[] Input)
		{
			return QuantumbogoSort(Input);
		}
		[Command("quantumBogo")]
		[Summary("QuantumBogoSorts the Array of Integers")]
		public Task QuantumBogoSortAsync(params System.Numerics.BigInteger[] Input)
		{
			return QuantumbogoSort(Input);
		}
		[Command("quantumBogo")]
		[Summary("QuantumBogoSorts the Array of Floats between -79,228,162,514,264,337,593,543,950,335 and 79,228,162,514,264,337,593,543,950,335")]
		public Task QuantumBogoSortAsync(params decimal[] Input)
		{
			return QuantumbogoSort(Input);
		}

		private Task QuantumbogoSort<T>(T[] Input) where T : IComparable
		{
			if (Input.Length == 0)
			{
				return ReplyAsync("Es muss mindestens ein Parameter angegeben sein");
			}

			Input = ShuffleArray(Input);
			if (CheckIfSorted(Input))
			{
				return ReplyAsync(string.Join(" ", Input) + $"{Environment.NewLine}The List was Quantum-Bogo-Sorted");
			}
			else
			{
				return ReplyAsync("The List was not sorted, please delete the universe");
			}
		}

		[Command("stalin")]
		[Summary("StalinSorts the Array of Strings")]
		public Task StalinortAsync(params string[] Input)
		{
			return Stalinsort(Input);
		}
		[Command("stalin")]
		[Summary("StalinSorts the Array of Integers")]
		public Task StalinortAsync(params System.Numerics.BigInteger[] Input)
		{
			return Stalinsort(Input);
		}
		[Command("stalin")]
		[Summary("StalinSorts the Array of Floats between -79,228,162,514,264,337,593,543,950,335 and 79,228,162,514,264,337,593,543,950,335")]
		public Task StalinortAsync(params decimal[] Input)
		{
			return Stalinsort(Input);
		}

		private Task Stalinsort<T>(T[] Input) where T : IComparable
		{
			List<T> Output = new List<T>(Input);
			if (Output.Count == 0)
			{
				return ReplyAsync("Es muss mindestens ein Parameter angegeben sein");
			}

			for (int i = 0; i < Output.Count - 1; i++)
			{
				if (Output[i].CompareTo(Output[i + 1]) > 0)
				{
					Output.RemoveAt(i + 1);
					i--;
				}
			}
			return ReplyAsync(string.Join(" ", Output) + $"{Environment.NewLine}The List was Stalin-Sorted");
		}

		private static bool CheckIfSorted<T>(T[] ToBeChecked) where T : IComparable
		{
			bool sorted = true;
			ToBeChecked = ShuffleArray(ToBeChecked);
			for (int i = 0; i < ToBeChecked.Length - 1; i++)
			{
				if (ToBeChecked[i].CompareTo(ToBeChecked[i + 1]) > 0)
				{
					sorted = false;
					break;
				}
			}
			return sorted;
		}

		private static T[] ShuffleArray<T>(T[] Input)
		{
			Random rand = new Random();
			return Input.OrderBy(x => rand.Next()).ToArray();
		}

	}


	[Group("reminder")]
	public class ReminderModule : ModuleBase<SocketCommandContext>, IDisposable
	{
		private Timer ReminderCheckTimer;
		private bool disposedValue;

		private List<Reminder> reminderList
		{
			get
			{
				return Properties.AshSettings.Default.Reminders;
			}
		}
		private void SaveReminders()
		{
			Properties.AshSettings.Default.Save();
		}

		public ReminderModule()
		{
			foreach (Reminder reminder in Properties.AshSettings.Default.Reminders)
			{
				ReminderCheckTimer = new Timer(6000)
				{
					AutoReset = true,
					Enabled = true
				};

				ReminderCheckTimer.Elapsed += ReminderCheckTimerElapsed;
			}
		}



		private void ReminderCheckTimerElapsed(object sender, ElapsedEventArgs e)
		{
			for (int i = reminderList.Count - 1; i >= 0; i--)
			{
				if (reminderList[i].EndTime < DateTime.Now)
				{
					Task<IUserMessage> SentMessage = CommandHandler._client.GetUser(reminderList[i].UserId).SendMessageAsync($"Deine Erinnerung: {reminderList[i].Note}");
					if (SentMessage is not null)
					{
						reminderList.RemoveAt(i);
					}
				}
			}
			SaveReminders();
		}


		[Command("add")]
		[Summary("Adds a new Reminder")]
		public Task timerStart(DateTime EndTime, [Remainder] string Note)
		{
			reminderList.Add(new Reminder { EndTime = EndTime, UserId = Context.User.Id, Note = Note });
			SaveReminders();

			return ReplyAsync("Reminder Set");
		}

		public record Reminder
		{
			public DateTime EndTime { get; init; }
			public ulong UserId { get; init; }
			public string Note { get; init; }
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
				}
				SaveReminders();

				ReminderCheckTimer.Dispose();


				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				disposedValue = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~ReminderModule()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}


	[Group("servers")]
	public class BotServerManagmentModule : ModuleBase<SocketCommandContext>
	{

		[Command("list")]
		[Summary("Lists the Servers of the bot")]
		[RequireBotMod]
		public Task ListServersAsync()
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


	public class MiscCommandsModlue : ModuleBase<SocketCommandContext>
	{
		[Command("trump")]
		[Summary("Gets the latest Trump Tweet")]
		public Task GetTrumpTweetAsync()
		{
			return ReplyAsync($"Donald Trump wurde von Twitter gebannt, TJA");
		}
	}
}
