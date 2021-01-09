using System;
using System.Collections.Generic;
using System.Configuration;

namespace AshBot
{
	public static class GlobalConstants
	{
		public static readonly string Glatt = "<:glatt:707922147334553620>";
		public static readonly string RegionalIndicatorC = "<:GrossC:796770644254785597>";
		public static readonly string RegionalIndicatorH = "<:GrossH:796770848165462057>";

		public static readonly ulong OliverId = 368487455730565131UL;
		public static readonly ulong ExegonneId = 261593494261465098UL;
		public static readonly ulong BellugahId = 377154197805596674UL;

		public static string BotToken { get { return ConfigurationManager.AppSettings.Get("DiscordToken"); } }

		public static List<ulong> ValidModIds { get; set; } = new List<ulong> { 285840083037454336UL, 633289137658527756UL, 229550714588889088UL, 509047639938039808UL };

	}
}
