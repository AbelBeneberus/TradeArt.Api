using TradeArt.Domain.Models;
using TradeArt.Infrastructure.GraphqlResponseModels;

namespace TradeArt.Infrastructure
{
	public static class Extensions
	{
		public static Price ToDomainPrice(this Market market)
		{
			decimal price = 0;
			if (decimal.TryParse(market.Ticker.LastPrice, out decimal value))
			{
				price = value;
			}

			return Price.Create(market.MarketSymbol, price);
		}

		public static Asset ToDomainAsset(this AssetResponse assetResponse)
		{
			return Asset.CreateAsset(assetResponse.AssetName, assetResponse.AssetSymbol, assetResponse.MarketCap);
		}
	}
}
