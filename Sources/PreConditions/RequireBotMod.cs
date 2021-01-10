using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AshBot
{
	class RequireBotMod : PreconditionAttribute
	{
		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			if (GlobalConstants.ValidModIds.Contains(context.User.Id))
			{
				return Task.FromResult(PreconditionResult.FromSuccess());
			}
			else
			{
				return Task.FromResult(PreconditionResult.FromError("Du hast nicht die Berechtigungen, um diesen Befehl auszuführen <:glatt:707922147334553620>"));
			}
		}
	}
}
