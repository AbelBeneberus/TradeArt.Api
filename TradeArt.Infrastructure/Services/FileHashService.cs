using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TradeArt.Domain.Interfaces;
using TradeArt.Infrastructure.Configuration;


namespace TradeArt.Infrastructure.Services
{
	public class FileHashService : IFileHashService
	{
		private readonly ILogger<FileHashService> _logger;

		private readonly AppSetting _appSettings;
		private readonly IHttpClientFactory _httpClientFactory;

		public FileHashService(ILogger<FileHashService> logger, IOptions<AppSetting> appSettings, IHttpClientFactory httpClientFactory)
		{
			_logger = logger;
			_httpClientFactory = httpClientFactory;
			_appSettings = appSettings.Value;
		}

		/// <summary>
		/// Download file from remote server and compute the hash without saving it in the disk.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<string> CalculateFileHashFromRemoteServerAsync(string url,
			CancellationToken cancellationToken)
		{
			try
			{
				using var httpClient = _httpClientFactory.CreateClient();
				httpClient.Timeout = TimeSpan.FromHours(1);

				using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
				response.EnsureSuccessStatusCode();

				await using var httpStream = await response.Content.ReadAsStreamAsync(cancellationToken);
				using var sha256 = SHA256.Create();
				var buffer = new byte[_appSettings.ChunkSize];
				int bytesRead;
				do
				{
					bytesRead = await httpStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
					if (bytesRead > 0)
					{
						sha256.TransformBlock(buffer, 0, bytesRead, buffer, 0);
					}
				} while (bytesRead > 0);

				sha256.TransformFinalBlock(buffer, 0, 0);
				var hash = sha256.Hash;

				return BitConverter.ToString(hash).Replace("-", "").ToLower();
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, "Error downloading remote file {0}: {1}", url, ex.Message, url);
				throw;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error calculating SHA hash for remote file {0}: {1}", url, ex.Message);
				throw;
			}
		}

		public async Task<string> CalculateFileFromLocalPathAsync(string path, CancellationToken cancellationToken)
		{
			return await CalculateHash(path, cancellationToken);
		}

		public async Task<string> CalculateHash(string path, CancellationToken cancellationToken)
		{
			try
			{
				await using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read,
					FileShare.ReadWrite);
				using var sha256 = SHA256.Create();
				var buffer = new byte[_appSettings.ChunkSize];
				int bytesRead;
				do
				{
					bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
					if (bytesRead > 0)
					{
						sha256.TransformBlock(buffer, 0, bytesRead, buffer, 0);
					}
				} while (bytesRead > 0);

				sha256.TransformFinalBlock(buffer, 0, 0);
				var hash = sha256.Hash;

				return BitConverter.ToString(hash).Replace("-", "").ToLower();
			}

			catch (IOException ex)
			{
				_logger.LogError(ex, $"Error reading local file {path}: {ex.Message}");
				throw;
			}

			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error calculating SHA hash for local file {path}: {ex.Message}");
				throw;
			}
		}
	}
}
