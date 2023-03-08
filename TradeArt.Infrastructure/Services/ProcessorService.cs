using Microsoft.Extensions.Logging;
using TradeArt.Domain.Interfaces;

namespace TradeArt.Infrastructure.Services;

public class ProcessorService : IDataProcessorService
{
	private readonly ILogger<ProcessorService> _logger;

	public ProcessorService(ILogger<ProcessorService> logger)
	{
		_logger = logger;
	}

	public async Task<bool> ProcessDataAsync(int data, CancellationToken cancellationToken)
	{
		_logger.LogInformation($"Processing: {data}");

		await Task.Delay(100, cancellationToken);
		 
		return true;

	}

}