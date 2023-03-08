 
namespace TradeArt.Domain.Interfaces
{
	public interface IFileHashService
	{
		Task<string> CalculateFileHashFromRemoteServerAsync(string url, CancellationToken cancellationToken);
		Task<string> CalculateFileFromLocalPathAsync(string path, CancellationToken cancellationToken);

	}
}
