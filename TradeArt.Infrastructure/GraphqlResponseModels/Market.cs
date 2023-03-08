namespace TradeArt.Infrastructure.GraphqlResponseModels
{    
	public class Market
	{
		public string MarketSymbol { get; set; }
		public Ticker Ticker { get; set; }
	}


	public class Ticker
	{
		public string LastPrice { get; set; }
	}
}