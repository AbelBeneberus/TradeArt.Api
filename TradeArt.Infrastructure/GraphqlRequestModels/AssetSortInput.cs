namespace TradeArt.Infrastructure.GraphqlRequestModels
{  	
	public class AssetSortInput
	{
		public string MarketCapRank { get; set; }
		public SortOrder Order { get; set; }
	}

	public enum SortOrder
	{
		ASC,
		DESC
	}
}
