using System.Text;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TradeArt.Application.Exceptions;
using TradeArt.Domain.Models;
using TradeArt.Infrastructure.CircuitBreaker;
using TradeArt.Infrastructure.Clients.Abstractions;
using TradeArt.Infrastructure.GraphqlResponseModels;

namespace TradeArt.Infrastructure.Clients;

public class PriceGraphQlClient : IPriceGraphQlClient
{
	private readonly IGraphQLClient _client;
	private readonly ILogger<PriceGraphQlClient> _logger;
	private readonly ICircuitBreaker _circuitBreaker;

	public PriceGraphQlClient(IGraphQLClient client, ILogger<PriceGraphQlClient> logger, ICircuitBreaker circuitBreaker)
	{
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_circuitBreaker = circuitBreaker ?? throw new ArgumentNullException(nameof(circuitBreaker));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}


	public async Task<List<Price>> GetPricesAsync(List<string> marketSymbols, string quote, string exchange, CancellationToken cancellationToken)
	{
		try
		{
			var prices = new List<Price>();

			var batchPrices = await
				GetBatchPricesAsync(marketSymbols, quote, exchange, cancellationToken);

			prices.AddRange(batchPrices);

			return prices;
		}
		catch (Exception e)
		{
			_logger.LogError(e, $"unable to fetch ");
			throw;
		}

	}

	private async Task<List<Price>> GetBatchPricesAsync(List<string> marketSymbols, string quote, string exchange, CancellationToken cancellationToken)
	{
		var parameterValues = new List<(string QueryName, string BaseSymbol, string QuoteSymbol, string ExchangeSymbol)>();
		var keys = new HashSet<string>();
		foreach (string marketSymbol in marketSymbols)
		{
			var key = $"Price_{exchange}_{marketSymbol}_{quote}";
			keys.Add(key);
			parameterValues.Add((key, marketSymbol, quote, exchange));

		}

		var queries = GenerateBatchQuery(parameterValues.ToArray());

		var request = new GraphQLRequest
		{
			Query = string.Join("\n", queries),
			OperationName = null,
			Variables = null
		};

		_logger.LogInformation($"Query Generated: {queries}");

		var response = await _circuitBreaker.GetPolicy()
			.ExecuteAsync(() => _client.SendQueryAsync<JObject>(request, cancellationToken));

		if (response.Errors != null && response.Errors.Length > 0)
		{
			_logger.LogError(message: $"Unable to retrieve data from remote server", response);

			throw new RemoteClientResponseException(response.Errors[0].Message);
		}

		var results = keys.Select(key =>
		{
			var marketPerKey = response.Data[key]?.ToObject<IEnumerable<Market>>();

			if (marketPerKey != null && marketPerKey.Count() == 0)
			{
				_logger.LogWarning($"Market information not found for the symbol {key}");
			}
			return marketPerKey;
		});

		var marketResult = results.SelectMany(market =>
			(market ?? throw new ArgumentNullException(nameof(market)))
			.Select(m => m.ToDomainPrice()))
			.ToList();

		return marketResult;

	}

	public string GenerateBatchQuery(params (string MarketSymbol, string, string, string)[] queries)
	{
		var queryBuilder = new StringBuilder().Append("query {");

		var lastMarketSymbol = queries.Last().MarketSymbol;
		foreach (var (queryName, baseSymbol, quoteSymbol, exchangeSymbol) in queries)
		{
			var queryBody = @$"markets(filter: {{baseSymbol: {{_eq: ""{baseSymbol}""}}, quoteSymbol: {{_eq: ""{quoteSymbol}""}}, exchangeSymbol: {{_eq: ""{exchangeSymbol}""}}}}) {{ marketSymbol ticker {{ lastPrice }} }}{GetFinalChar(lastMarketSymbol, queryName)}";
			queryBuilder.Append($" {queryName}: {queryBody},");
		}

		queryBuilder.Append(" }");

		return queryBuilder.ToString();
	}
	private string GetFinalChar(string currentMarketSymbol, string finalMarketSymbol)
	{
		return currentMarketSymbol.Equals(finalMarketSymbol) ? string.Empty : ",";
	}
}