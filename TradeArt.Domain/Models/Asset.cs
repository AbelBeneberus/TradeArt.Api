namespace TradeArt.Domain.Models
{
	public class Asset
	{
		private Asset(string assetName, string assetSymbol, long? marketCap)
		{
			AssetName = assetName;
			AssetSymbol = assetSymbol;
			MarketCap = marketCap;
		}
		public string AssetName { get; }
		public string AssetSymbol { get; }
		public long? MarketCap { get; }

		public static Asset CreateAsset(string assetName, string assetSymbol, long? marketCap)
		{
			return new Asset(assetName, assetSymbol, marketCap);
		}
	}

}
