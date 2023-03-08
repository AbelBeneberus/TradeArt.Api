
using Microsoft.Extensions.DependencyInjection;
using TradeArt.Application.UseCases.FetchAssetsAndPricesUseCase;
using TradeArt.Application.UseCases.NumberProcessor;

namespace TradeArt.Application
{
	public static class ApplicationIoc
	{
		public static IServiceCollection AddApplication(this IServiceCollection service)
		{
			service.AddScoped<IGetAssetPricesUseCase, GetAssetPricesUseCase>();
			service.AddScoped<IProcessNumberUseCase, ProcessNumberUseCase>();
			return service;
		}
	}
}