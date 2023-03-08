using TradeArt.Domain.Interfaces;

namespace TradeArt.Application.UseCases.CalculateFileHash;

public class CalculateFileHashUseCase
{
	private readonly IFileHashService _fileHashService;

	public CalculateFileHashUseCase(IFileHashService fileHashService)
	{
		_fileHashService = fileHashService;
	}

	public async Task<string> ExecuteFromUrl(string input, CancellationToken cancellationToken)
	{
		 
		var hash = await _fileHashService.CalculateFileHashFromRemoteServerAsync(input, cancellationToken);

		return hash;
	}  
	public async Task<string> ExecuteFromLocalPath(string input, CancellationToken cancellationToken)
	{
		 
		var hash = await _fileHashService.CalculateFileFromLocalPathAsync(input, cancellationToken);

		return hash;
	}
}