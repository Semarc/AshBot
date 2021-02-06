using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

public class LoggingService
{
	public LoggingService(DiscordSocketClient client, CommandService command)
	{
		client.Log += LogAsync;
		command.Log += LogAsync;
		command.CommandExecuted += Command_CommandExecuted;

	}


	private Task Command_CommandExecuted(Optional<CommandInfo> commandInfosOpt, ICommandContext arg2, IResult arg3)
	{
		string Infos = $"Server: {arg2.Guild}, Channel: {arg2.Channel}, User: {arg2.User}, Actual Message {arg2.Message}";
		string Error = arg3.IsSuccess ? string.Empty : $"Error: {(arg3.Error.HasValue ? string.Empty : arg3.Error.Value.ToString())}: {arg3.ErrorReason}";
		if (commandInfosOpt.IsSpecified)
		{
			CommandInfo ComInfo = commandInfosOpt.Value;
			Console.WriteLine($"[Command/{ComInfo.Module.Name}] {DateTime.Now.ToLongTimeString()} {ComInfo.Name} executed: Info: {Infos} {Error}");
		}
		else
		{
			Console.WriteLine($"[Something] Info: {Infos} {Error}");
		}
		return Task.CompletedTask;
	}

	private Task LogAsync(LogMessage message)
	{ 
		if (message.Exception is CommandException cmdException)
		{
			Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
				+ $" failed to execute in {cmdException.Context.Channel}.");
			Console.WriteLine(cmdException);
		}
		else
			Console.WriteLine($"[General/{message.Severity}] {message}");

		return Task.CompletedTask;
	}
}