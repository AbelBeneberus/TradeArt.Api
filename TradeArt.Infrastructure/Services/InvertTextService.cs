using System.Text;
using Microsoft.Extensions.Logging;
using TradeArt.Domain.Interfaces;
using TradeArt.Infrastructure.Repositories;

namespace TradeArt.Infrastructure.Services
{
    public class InvertTextService	 : IInvertTextService
	{
		private readonly IInvertTextRepository _repository;
		private readonly ILogger<InvertTextService> _logger;

		public InvertTextService(IInvertTextRepository repository, ILogger<InvertTextService> logger)
		{
			_repository = repository;
			_logger = logger;
		}

		public string InvertText(string text)
		{
			if (text == null)
			{
				_logger.LogError("Input text is null.");
				throw new ArgumentNullException(nameof(text));
			}

			var stringBuilder = new StringBuilder(text);

			try
			{
				var result = _repository.InvertText(stringBuilder);
				_logger.LogInformation("Inverted text: {0}", result);
				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while inverting text.");
				throw;
			}
		}
	}
}
