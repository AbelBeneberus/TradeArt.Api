using TradeArt.Domain.Models;

namespace TradeArt.Domain.Interfaces;

public interface IAssetService
{
	Task<IEnumerable<Asset>> GetAssetsAsync(CancellationToken cancellationToken);
}