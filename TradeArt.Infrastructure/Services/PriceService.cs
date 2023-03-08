using TradeArt.Domain.Interfaces;
using TradeArt.Domain.Models;
using TradeArt.Infrastructure.Clients.Abstractions;

namespace TradeArt.Infrastructure.Services
{
    public class PriceService : IPriceService
	{
		private readonly IPriceGraphQlClient _priceGraphQlClient;
		public PriceService(IPriceGraphQlClient priceGraphQlClient)
		{
			_priceGraphQlClient = priceGraphQlClient;
		}
		public Task<List<Price>> GetPricesAsync(List<string> marketSymbols, string quote, string exchange, CancellationToken cancellationToken)
		{
			return _priceGraphQlClient.GetPricesAsync(marketSymbols, quote, exchange, cancellationToken);
		}
	}

}
