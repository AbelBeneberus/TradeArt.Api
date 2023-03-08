using TradeArt.Domain.Interfaces;
using TradeArt.Infrastructure.Clients.Abstractions;

namespace TradeArt.Infrastructure.Services
{
    public class AssetService : IAssetService
	{
		private readonly IAssetGraphQlClient _assetGraphQlClient;
		public AssetService(IAssetGraphQlClient assetGraphQlClient)
		{
			_assetGraphQlClient = assetGraphQlClient;
		}
		public Task<IEnumerable<Domain.Models.Asset>> GetAssetsAsync(CancellationToken cancellationToken)
		{
			return _assetGraphQlClient.GetAssetsAsync(cancellationToken);
		}
	}
}
