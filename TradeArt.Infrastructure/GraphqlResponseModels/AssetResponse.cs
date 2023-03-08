namespace TradeArt.Infrastructure.GraphqlResponseModels
{
	public class AssetResponse
	{
		public string AssetName { get; set; }	= string.Empty;
		public string AssetSymbol { get; set; } = string.Empty;
		public long? MarketCap { get; set; }   
	}

}
