namespace TradeArt.Domain.Interfaces
{
	public interface IDataProcessorService
	{
		Task<bool> ProcessDataAsync(int data, CancellationToken cancellationToken);
	}
}
