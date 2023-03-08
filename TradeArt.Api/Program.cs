using System.Text.Json;
using TradeArt.Application;
using TradeArt.Infrastructure;
using TradeArt.Infrastructure.Configuration;


var builder = WebApplication.CreateBuilder(args);
var loggerFactory = LoggerFactory.Create(loggingBuilder =>
{
	loggingBuilder.AddConsole();
});

ILogger logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("Starting Application");
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
	options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSetting"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
var appSetting = builder.Configuration.GetSection("AppSetting").Get<AppSetting>();
builder.Services.AddInfrastructure(appSetting);
builder.Services.AddApplication();
builder.Services.AddLogging((loggingBuilder) => loggingBuilder
	.SetMinimumLevel(LogLevel.Trace)
	.AddConsole()
);

try
{
	var app = builder.Build();


	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	app.UseHttpsRedirection();

	app.UseAuthorization();

	app.MapControllers();
	app.Run();
}
catch (Exception ex)
{
	logger.LogError($"Unable to start the app: {ex.Message}", ex);
	throw;
}


