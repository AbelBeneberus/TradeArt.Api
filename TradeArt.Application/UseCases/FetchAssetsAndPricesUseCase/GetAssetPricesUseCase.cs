using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TradeArt.Domain.Interfaces;
using TradeArt.Domain.Models;

namespace TradeArt.Application.UseCases.FetchAssetsAndPricesUseCase
{
	public interface IGetAssetPricesUseCase
	{
		Task<List<Price>> ExecuteAsync(string quoteSymbol, string exchangeSymbol, CancellationToken cancellationToken);
	}
	public class GetAssetPricesUseCase	:IGetAssetPricesUseCase
	{
		private readonly IAssetService _assetService;
		private readonly IPriceService _priceService;
		private readonly ILogger<GetAssetPricesUseCase> _logger;

		public GetAssetPricesUseCase(IAssetService assetService, IPriceService priceService, ILogger<GetAssetPricesUseCase> logger)
		{
			_assetService = assetService;
			_priceService = priceService;
			_logger = logger;
		}
		public async Task<List<Price>> ExecuteAsync(string quoteSymbol, string exchangeSymbol, CancellationToken cancellationToken)
		{
			try
			{
				var assets = await _assetService.GetAssetsAsync(cancellationToken);

				var batchedMarketSymbols = SplitIntoBatches(assets.Select(asset => asset.AssetSymbol), 20);

				var tasks = new List<Task<List<Price>>>();

				foreach (IEnumerable<string> batchedMarketSymbol in batchedMarketSymbols)
				{
					tasks.Add(_priceService.GetPricesAsync(marketSymbols: batchedMarketSymbol.ToList(),
						quote: quoteSymbol,
						exchange: exchangeSymbol,
						cancellationToken: cancellationToken));
					_logger.LogInformation($"Price Request Created: marketSymbols: {JsonConvert.SerializeObject(batchedMarketSymbol)}, quote : {quoteSymbol}, exchange: {exchangeSymbol}");
				}

				var batchResult = await Task.WhenAll(tasks);
				var pricesResult = batchResult.SelectMany(prices => prices);

				return pricesResult.ToList();
			}
			catch (Exception e)
			{
				_logger.LogError($"unexpected exception happens while executing {nameof(GetAssetPricesUseCase)}", e);
				throw;
			}

		}

		private static IEnumerable<IEnumerable<T>> SplitIntoBatches<T>(IEnumerable<T> items, int batchSize)
		{
			for (int i = 0; i < items.Count(); i += batchSize)
			{
				yield return items.Skip(i).Take(batchSize);
			}
		}

	}
}
