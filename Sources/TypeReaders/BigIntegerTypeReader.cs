using System;
using System.Numerics;
using System.Threading.Tasks;

using Discord.Commands;

namespace AshBot
{
	public class BigIntegerTypeReader : TypeReader
	{
		public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
		{
			BigInteger result;
			if (BigInteger.TryParse(input, out result))
			{
				return Task.FromResult(TypeReaderResult.FromSuccess(result));
			}
			return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Failed to Parse Input as Integer"));

		}
	}
}
