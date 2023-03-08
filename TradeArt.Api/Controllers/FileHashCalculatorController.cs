using Microsoft.AspNetCore.Mvc;
using TradeArt.Application.UseCases.CalculateFileHash;
using TradeArt.Domain.Interfaces;

namespace TradeArt.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FileHashCalculatorController : ControllerBase
	{
		private readonly ILogger<FileHashCalculatorController> _logger;
		private readonly CalculateFileHashUseCase _calculateFileHashUseCase;

		public FileHashCalculatorController(IFileHashService fileHashService, ILogger<FileHashCalculatorController> logger)
		{
			_logger = logger;
			_calculateFileHashUseCase = new CalculateFileHashUseCase(fileHashService);
		}


		[HttpGet("fromRemote")]
		public async Task<IActionResult> ComputeHash(CancellationToken cancellationToken, string fileName = "https://speed.hetzner.de/100MB.bin")
		{
			try
			{
				var result = await _calculateFileHashUseCase.ExecuteFromUrl(fileName, cancellationToken);
				return Ok(result);
			}
			catch (ArgumentNullException ex)
			{
				_logger.LogError(ex, "url is null.");
				return BadRequest(ex.Message);
			}
			catch (OperationCanceledException operationCanceledException)
			{
				_logger.LogError(operationCanceledException, "Operation canceled");
				return StatusCode(444, operationCanceledException.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while inverting text.");
				return StatusCode(500, ex.Message);
			}
		}

		[HttpGet("fromLocal")]
		public async Task<IActionResult> ComputeFromLocalDisk(CancellationToken cancellationToken, string filePath = "C:\\TradeArt\\100MB.bin")
		{
			try
			{
				var hash = await _calculateFileHashUseCase.ExecuteFromLocalPath(filePath, cancellationToken);
				return Ok(hash);
			}
			catch (ArgumentNullException ex)
			{
				_logger.LogError(ex, "file path is null.");
				return BadRequest(ex.Message);
			}
			catch (OperationCanceledException operationCanceledException)
			{
				_logger.LogError(operationCanceledException, "Operation canceled");
				return StatusCode(444, operationCanceledException.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while processing hash.");
				return StatusCode(500, ex.Message);
			}

		}
	}
}
