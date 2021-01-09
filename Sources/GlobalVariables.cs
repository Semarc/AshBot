using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.API;

namespace AshBot
{
	public static class GlobalVariables
	{
		public static char CommandPrefix { get; set; } = '+';

		public static string CreatePing(ulong UserId)
		{
			return $"<@!{UserId}>";
		}

	}
}
