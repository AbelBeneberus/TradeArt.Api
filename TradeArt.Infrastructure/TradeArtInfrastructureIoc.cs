using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.DependencyInjection;
using TradeArt.Domain.Interfaces;
using TradeArt.Infrastructure.CircuitBreaker;
using TradeArt.Infrastructure.Clients;
using TradeArt.Infrastructure.Clients.Abstractions;
using TradeArt.Infrastructure.Configuration;
using TradeArt.Infrastructure.Repositories;
using TradeArt.Infrastructure.Services;

namespace TradeArt.Infrastructure
{
	public static class TradeArtInfrastructureIoc
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, AppSetting setting)
		{
			services.AddScoped<IDataProcessorService, ProcessorService>();
			services.AddScoped<IDataEmitterService, EmitterService>();
			services.AddScoped<IPriceService, PriceService>();
			services.AddScoped<IPriceGraphQlClient, PriceGraphQlClient>();
			services.AddScoped<IAssetGraphQlClient, AssetGraphQlClient>();
			services.AddScoped<IFileHashService, FileHashService>();
			services.AddSingleton<ICircuitBreaker, CircuitBreaker.CircuitBreaker>();
			services.AddScoped<IAssetService, AssetService>();
			services.AddSingleton<IInvertTextRepository, InvertTextRepository>();
			services.AddSingleton<IInvertTextService, InvertTextService>();
		    
			services.AddSingleton<IGraphQLClient>(ctx => new GraphQLHttpClient(setting.GraphQlEndpoint,
				new NewtonsoftJsonSerializer()));

			return services;
		}
	}


}
