namespace TradeArt.Infrastructure.Clients.Abstractions;

public interface IAssetGraphQlClient
{
    Task<IEnumerable<Domain.Models.Asset>> GetAssetsAsync(CancellationToken cancellationToken);
}