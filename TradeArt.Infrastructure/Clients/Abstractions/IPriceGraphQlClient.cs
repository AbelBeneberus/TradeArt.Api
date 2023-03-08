using TradeArt.Domain.Models;

namespace TradeArt.Infrastructure.Clients.Abstractions;

public interface IPriceGraphQlClient
{
    Task<List<Price>> GetPricesAsync(List<string> marketSymbols, string quote, string exchange, CancellationToken cancellationToken);

}