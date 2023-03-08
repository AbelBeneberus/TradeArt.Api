using TradeArt.Domain.Interfaces;

namespace TradeArt.Application.UseCases.InvertText
{
	public class InvertTextUseCase
	{
		private readonly IInvertTextService _invertTextService;

		public InvertTextUseCase(IInvertTextService invertTextService)
		{
			_invertTextService = invertTextService;
		}

		public string Run(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException(nameof(text));
			}
			return _invertTextService.InvertText(text);
		}
	}
}
