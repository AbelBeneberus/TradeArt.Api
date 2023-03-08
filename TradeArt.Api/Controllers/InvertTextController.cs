using Microsoft.AspNetCore.Mvc;
using TradeArt.Application.UseCases.InvertText;
using TradeArt.Domain.Interfaces;

namespace TradeArt.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class InvertTextController : ControllerBase
	{
		private readonly ILogger<InvertTextController> _logger;
		private readonly InvertTextUseCase _invertTextUseCase;

		private const string DefaultParam =
					@"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor 
					incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud
					exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure
					dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.
					Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit
					anim id est laborum.";

		public InvertTextController(IInvertTextService invertTextUseCase, ILogger<InvertTextController> logger)
		{
			_invertTextUseCase = new InvertTextUseCase(invertTextUseCase);
			_logger = logger;
		}

		[HttpPost]
		public IActionResult InvertText([FromBody] string text = DefaultParam)
		{
			try
			{
				var invertedText = _invertTextUseCase.Run(text);
				return Ok(invertedText);
			}
			catch (ArgumentNullException ex)
			{
				_logger.LogError(ex, "Input text is null.");
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while inverting text.");
				return StatusCode(500, ex.Message);
			}
		}



	}
}
