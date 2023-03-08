namespace TradeArt.Domain.Models;

public class Price
{
	private Price(string marketSymbol, decimal lastPrice)
	{
		MarketSymbol = marketSymbol;
		LastPrice = lastPrice;
	}

	public string MarketSymbol { get; set; }
	public decimal LastPrice { get; set; }

	public static Price Create(string marketSymbol, decimal lastPrice)
	{
		return new Price(marketSymbol, lastPrice);
	}
}