using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AshBot
{
	public static class Utilities
	{
		public static string StringToRegionalIndicator(string Input)
		{
			StringBuilder builder = new StringBuilder(Input);

			foreach (KeyValuePair<string, string> valuePair in GlobalConstants.RegionalIndicatorStrings)
			{
				builder.Replace(valuePair.Key, $"{valuePair.Value} ");
			}
			return builder.ToString();

		}

		public static Image ImageFromUrl(string URL)
		{

			HttpClient client = default;
			try
			{
				client = new HttpClient();

				Stream stream = client.GetStreamAsync(URL).GetAwaiter().GetResult();

				Image image = new Image(stream);

				return image;
			}
			catch
			{
				return default;
			}
			finally
			{
				client.Dispose();
			}
		}

	}
}
