using GraphQL.Client.Abstractions;
using GraphQL;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using Polly;
using TradeArt.Infrastructure.CircuitBreaker;
using TradeArt.Infrastructure.Clients;
using Xunit;
using TradeArt.Application.Exceptions;
using TradeArt.Infrastructure.GraphqlResponseModels;

namespace TradeArt.UnitTest.Clients
{
	public class AssetGraphQlClientTests
	{
		private readonly Mock<IGraphQLClient> _mockClient;
		private readonly Mock<ICircuitBreaker> _mockCircuitBreaker;
		private readonly Mock<ILogger<AssetGraphQlClient>> _mockLogger;



		public AssetGraphQlClientTests()
		{
			_mockClient = new Mock<IGraphQLClient>();
			_mockCircuitBreaker = new Mock<ICircuitBreaker>();
			_mockLogger = new Mock<ILogger<AssetGraphQlClient>>();
			_mockCircuitBreaker.Setup(cb => cb.GetPolicy()).Returns(() =>
				Policy.Handle<CircuitBreakerOpenException>()
					.CircuitBreakerAsync(1,
						durationOfBreak: TimeSpan.FromSeconds(1)));
		}

		[Fact]
		public async Task GetAssetsAsync_WithValidQuery_ReturnsMappedDomainAssets()
		{
			// Arrange
			 
			var expectedAssets = new List<AssetResponse>
			{
				new AssetResponse { AssetName = "Asset 1", AssetSymbol = "A1", MarketCap = 1000 },
				new AssetResponse { AssetName = "Asset 2", AssetSymbol = "A2", MarketCap = 2000 },
				new AssetResponse { AssetName = "Asset 3", AssetSymbol = "A3", MarketCap = 3000 }
			};

			var expectedData = new JObject(new JProperty("assets", JToken.FromObject(expectedAssets)));

			var expectedResult = new GraphQLResponse<JObject>
			{
				Data = expectedData
			};
				   
			_mockClient.Setup(x => x.SendQueryAsync<JObject>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(expectedResult);

			var client = new AssetGraphQlClient(_mockClient.Object, _mockCircuitBreaker.Object, _mockLogger.Object);


			// Act
			var result = await client.GetAssetsAsync(CancellationToken.None);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(expectedAssets.Count, result.Count());

			for (var i = 0; i < expectedAssets.Count; i++)
			{
				var expectedAsset = expectedAssets[i];
				var actualAsset = result.ElementAt(i);

				Assert.Equal(expectedAsset.AssetName, actualAsset.AssetName);
				Assert.Equal(expectedAsset.AssetSymbol, actualAsset.AssetSymbol);
				Assert.Equal(expectedAsset.MarketCap, actualAsset.MarketCap);
			}

			_mockClient.Verify(x => x.SendQueryAsync<JObject>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>()), Times.Once);
			_mockCircuitBreaker.Verify(x => x.GetPolicy(), Times.Once);
			_mockLogger.Verify(x => x.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.IsAny<It.IsAnyType>(),
				It.IsAny<Exception>(),
				((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
				Times.Never);
		}

		[Fact]
		public async Task GetAssetsAsync_WithErrorResponse_ThrowsRemoteClientResponseException()
		{
			// Arrange
			var expectedErrorMessage = "Test error message";
			var expectedErrors = new[] { new GraphQLError { Message = expectedErrorMessage } };
			var response = new GraphQLResponse<JObject>
			{
				Data = null,
				Errors = expectedErrors
			}; 

			_mockClient.Setup(x => x.SendQueryAsync<JObject>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(response);
			 
			 
			var mockLogger = new Mock<ILogger<AssetGraphQlClient>>();

			var client = new AssetGraphQlClient(_mockClient.Object, _mockCircuitBreaker.Object, mockLogger.Object);

			// Act & Assert
			var exception = await Assert.ThrowsAsync<RemoteClientResponseException>(async () => await client.GetAssetsAsync(CancellationToken.None));

			Assert.Equal(expectedErrorMessage, exception.Message);
		}

	}
}
