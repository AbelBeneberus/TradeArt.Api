namespace TradeArt.Domain.Interfaces
{
	public interface IDataEmitterService
	{
		Task EmitDataAsync(List<int> data, CancellationToken cancellationToken);
	}
}
