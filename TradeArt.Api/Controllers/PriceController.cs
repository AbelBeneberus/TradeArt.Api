using Microsoft.AspNetCore.Mvc;
using TradeArt.Application.Exceptions;
using TradeArt.Application.UseCases.FetchAssetsAndPricesUseCase;

namespace TradeArt.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PriceController : ControllerBase
	{
		private readonly ILogger<PriceController> _logger;
		private readonly IGetAssetPricesUseCase _getAssetPricesUseCase;
		 
		public PriceController(IGetAssetPricesUseCase getAssetPricesUseCase, ILogger<PriceController> logger)
		{
			_logger = logger;
			_getAssetPricesUseCase = getAssetPricesUseCase;
		}

		[HttpGet("{quoteSymbol}/{exchangeSymbol}")]
		public async Task<IActionResult> GetPriceResult(string quoteSymbol = "EUR", string exchangeSymbol = "BINANCE")
		{
			try
			{
				var result =
					await _getAssetPricesUseCase.ExecuteAsync(quoteSymbol, exchangeSymbol, CancellationToken.None);
				return Ok(result);
			}
			catch (CircuitBreakerOpenException exception)
			{
				_logger.LogError(exception, "Unable to connect to remote service.");
				return StatusCode(500, exception.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while processing price.");
				return StatusCode(500, ex.Message);
			}

		}
	}
}
