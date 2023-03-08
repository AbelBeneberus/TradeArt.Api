using Microsoft.AspNetCore.Mvc;
using TradeArt.Application.UseCases.NumberProcessor;

namespace TradeArt.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class NumberProcessor : ControllerBase
	{
		private readonly ILogger<NumberProcessor> _logger;
		private readonly IProcessNumberUseCase _processNumberUseCase;
		public NumberProcessor(IProcessNumberUseCase processNumberUseCase, ILogger<NumberProcessor> logger)
		{
			_processNumberUseCase = processNumberUseCase;
			_logger = logger;
		}
		[HttpGet]
		public async Task<IActionResult> Get(CancellationToken cancellationToken, int count = 1000)
		{
			try
			{
				var result = await _processNumberUseCase.Run(cancellationToken, count);
				if (result)
				{
					return Ok();
				}

				return Problem();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while processing numbers.");
				return StatusCode(500, ex.Message);
			}

		}
	}
}
