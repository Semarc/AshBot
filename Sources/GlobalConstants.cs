using System;
using System.Collections.Generic;
using System.Configuration;

namespace AshBot
{
	public static class GlobalConstants
	{
		public static readonly Dictionary<string, string> RegionalIndicatorStrings = new Dictionary<string, string>
		{
			["a"] = "🇦",
			["b"] = "🇧",
			["c"] = "🇨",
			["d"] = "🇩",
			["e"] = "🇪",
			["f"] = "🇫",
			["g"] = "🇬",
			["h"] = "🇭",
			["i"] = "🇮",
			["j"] = "🇯",
			["k"] = "🇰",
			["l"] = "🇱",
			["m"] = "🇲",
			["n"] = "🇳",
			["o"] = "🇴",
			["p"] = "🇵",
			["q"] = "🇶",
			["r"] = "🇷",
			["s"] = "🇸",
			["t"] = "🇹",
			["u"] = "🇺",
			["v"] = "🇻",
			["w"] = "🇼",
			["x"] = "🇽",
			["y"] = "🇾",
			["z"] = "🇿"
		};

		public static readonly List<ulong> AllowedSpamChannels = new List<ulong>
		{
			756175335887995013UL
		};

		public static readonly string Glatt = "<:glatt:735557905742561340>";
		public static readonly string SecondRegionalIndicatorC = "<:GrossC:796770644254785597>";
		public static readonly string SecondRegionalIndicatorH = "<:GrossH:796770848165462057>";

		public static readonly ulong OliverId = 368487455730565131UL;
		public static readonly ulong ExegonneId = 261593494261465098UL;
		public static readonly ulong BellugahId = 377154197805596674UL;
		public static readonly ulong JulianReicheltBotId = 569097256217870336UL;

		public static string BotToken { get { return Properties.AshSettings.Default.Token; } }

		public static string ProfilePictureURL
		{
			get { return Properties.AshSettings.Default.ProfilePicture; }
			set
			{
				Properties.AshSettings.Default.ProfilePicture = value;
				Properties.AshSettings.Default.Save();
			}
		}

		public static readonly List<ulong> ValidModIds = new List<ulong> {/*Semarc*/ 285840083037454336UL, /*Darrin*/ 633289137658527756UL, /*Cactuz 229550714588889088UL, *//*Tim 509047639938039808UL */};

		public static readonly ulong CactuzId = 229550714588889088UL;

		public static readonly Dictionary<string, string> Answers = new Dictionary<string, string>
		{
			["rechtsbruch"] = Utilities.StringToRegionalIndicator("rechtsbruch"),
			["linksbruch"] = Utilities.StringToRegionalIndicator("linksbruch"),
			["obenbruch"] = Utilities.StringToRegionalIndicator("obenbruch"),
			["untenbruch"] = Utilities.StringToRegionalIndicator("untenbruch"),
			["vornebruch"] = Utilities.StringToRegionalIndicator("vornebruch"),
			["hintenbruch"] = Utilities.StringToRegionalIndicator("hintenbruch"),
			["verfassungsbruch"] = Utilities.StringToRegionalIndicator("verfassungsbruch"),
			["senat"] = "https://media1.tenor.com/images/3114605b7e08f02cd37a82fb4fd15b0c/tenor.gif",
			["democracy"] = "https://tenor.com/view/star-wars-democracy-i-love-democracy-gif-13935227",
			["treason"] = "https://media.tenor.com/images/57e5b1257bbfe74f1c6c7c49a067eb51/tenor.gif",
			//["mist"] = "https://media1.tenor.com/images/90d31f66e4e668a4e84926402cbcd660/tenor.gif",
		};
		public static readonly Dictionary<string, string> WholeThings = new Dictionary<string, string>
		{
			["hallo dort"] = "https://tenor.com/view/hello-there-general-kenobi-star-wars-grevious-gif-17774326",
			["hello there"] = "https://tenor.com/view/hello-there-general-kenobi-star-wars-grevious-gif-17774326",
			["execute order 66"] = "It will be done, my lord."
		};

	}
}
