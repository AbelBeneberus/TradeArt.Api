using System.Text;
using Microsoft.Extensions.Logging;

namespace TradeArt.Infrastructure.Repositories
{
	public class InvertTextRepository : IInvertTextRepository
	{
		private readonly ILogger<InvertTextRepository> _logger;
		public const int MaxInputLength = 1000000; // Maximum input length to prevent memory allocation errors

		public InvertTextRepository(ILogger<InvertTextRepository> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public string InvertText(StringBuilder input)
		{
			if (input == null)
			{
				_logger.LogError("Input is null.");
				throw new ArgumentNullException(nameof(input));
			}

			if (input.Length > MaxInputLength)
			{
				_logger.LogError($"Input exceeds maximum allowed length. [{input.Length}]");
				throw new ArgumentException(
					$"Input exceeds maximum allowed length. Maximum allowed length is {MaxInputLength}.", nameof(input));
			}

			var textLength = input.Length;

			if (textLength == 0)
			{
				_logger.LogError("Input is empty.");
				throw new ArgumentException("Input cannot be empty.", nameof(input));
			}

			var i = 0;
			var j = textLength - 1;

			while (i < j)
			{
				(input[i], input[j]) = (input[j], input[i]);
				i++;
				j--;
			}

			var result = input.ToString();

			_logger.LogInformation($"Input string [{input}] has been reversed to [{result}].");

			return result;
		}
	}
}
