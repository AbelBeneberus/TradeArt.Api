using TradeArt.Domain.Models;

namespace TradeArt.Domain.Interfaces;

public interface IPriceService
{
	Task<List<Price>> GetPricesAsync(List<string> marketSymbols, string quote, string exchange, CancellationToken cancellationToken);
}