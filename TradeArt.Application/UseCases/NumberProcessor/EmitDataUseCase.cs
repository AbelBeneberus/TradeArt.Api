using Microsoft.Extensions.Logging;
using TradeArt.Domain.Interfaces;

namespace TradeArt.Application.UseCases.NumberProcessor
{
	public interface IProcessNumberUseCase
	{
		Task<bool> Run(CancellationToken cancellationToken, int count);
	}
	public class ProcessNumberUseCase : IProcessNumberUseCase
	{
		private readonly IDataEmitterService _dataEmitterService;
		private readonly ILogger<ProcessNumberUseCase> _logger;
		public ProcessNumberUseCase(IDataEmitterService dataEmitterService, ILogger<ProcessNumberUseCase> logger)
		{
			_dataEmitterService = dataEmitterService;
			_logger = logger;
		}

		public async Task<bool> Run(CancellationToken cancellationToken, int count)
		{
			try
			{
				await _dataEmitterService.EmitDataAsync(GenerateList(count), cancellationToken);
				return true;
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, $"Unable to processes data: {exception.Message}");
				throw;
			}

		}

		private List<int> GenerateList(int count)
		{
			var data = new List<int>();

			for (int i = 0; i < count; i++)
			{
				data.Add(i);
			}
			return data;
		}
	}
}
