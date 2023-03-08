using Microsoft.Extensions.Logging;
using TradeArt.Domain.Interfaces;

namespace TradeArt.Infrastructure.Services;

public class EmitterService : IDataEmitterService
{
	private readonly IDataProcessorService _processor;
	private readonly ILogger<EmitterService> _logger;

	public EmitterService(IDataProcessorService processor, ILogger<EmitterService> logger)
	{
		_processor = processor;
		_logger = logger;
	}

	public async Task EmitDataAsync(List<int> data, CancellationToken cancellationToken)
	{
		//ConcurrentDictionary<int, bool> dataBag = data.ToDictionary();

		var tasks = data.Select(dataFromBag =>
				Task.Run(
					() =>
					{
						_processor.ProcessDataAsync(dataFromBag, cancellationToken)
							.ContinueWith(
								task =>
								{
									_logger.LogInformation($"Processed: {dataFromBag} with result of {task.Result}");
								},
								cancellationToken);
					}, cancellationToken))
			.ToList();

		await Task.WhenAll(tasks).ConfigureAwait(false);
	}

}

