using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TradeArt.Application.Exceptions;
using TradeArt.Infrastructure.CircuitBreaker;
using TradeArt.Infrastructure.Clients.Abstractions;
using TradeArt.Infrastructure.GraphqlRequestModels;
using TradeArt.Infrastructure.GraphqlResponseModels;

namespace TradeArt.Infrastructure.Clients;

public class AssetGraphQlClient : IAssetGraphQlClient
{
	private readonly IGraphQLClient _client;
	private readonly ICircuitBreaker _circuitBreaker;
	private readonly ILogger<AssetGraphQlClient> _logger;

	public AssetGraphQlClient(IGraphQLClient client, ICircuitBreaker circuitBreaker, ILogger<AssetGraphQlClient> logger)
	{
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_circuitBreaker = circuitBreaker ?? throw new ArgumentNullException(nameof(circuitBreaker));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<IEnumerable<Domain.Models.Asset>> GetAssetsAsync(CancellationToken cancellationToken)
	{
		var query = @"{assets(sort: [{marketCapRank: ASC}]){assetName assetSymbol marketCap}}";

		var variables = new
		{
			sort = new List<AssetSortInput>
			{
				new() { MarketCapRank = "ASC", Order = SortOrder.ASC }
			}
		};

		var requestObject = new GraphQLRequest
		{
			Query = query,
			Variables = variables
		};

		try
		{
			var result = await _circuitBreaker.GetPolicy()
				.ExecuteAsync(() => _client.SendQueryAsync<JObject>(requestObject, cancellationToken));

			if (result.Errors != null && result.Errors.Length > 0)
			{
				_logger.LogError("Unable to retrieve data from remote server: {ErrorMessage}", result.Errors[0].Message);
				throw new RemoteClientResponseException(result.Errors[0].Message);
			}

			var mappedResult = result.Data?["assets"]?.ToObject<IEnumerable<AssetResponse>>() ?? Enumerable.Empty<AssetResponse>();

			return mappedResult.Select(asset => asset.ToDomainAsset());
		}
		catch (OperationCanceledException operationCanceledException)
		{
			_logger.LogError(operationCanceledException, "Operation was cancelled");
			throw;
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "Unable to Fetch Assets");
			throw;
		}
	}
}