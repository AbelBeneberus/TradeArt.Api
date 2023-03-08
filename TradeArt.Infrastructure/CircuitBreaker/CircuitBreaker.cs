using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using TradeArt.Application.Exceptions;
using TradeArt.Infrastructure.Configuration;

namespace TradeArt.Infrastructure.CircuitBreaker
{
	public class CircuitBreaker : ICircuitBreaker
	{
		private readonly IOptions<AppSetting> _options;
		private readonly ILogger<CircuitBreaker> _logger;

		public CircuitBreaker(IOptions<AppSetting> options, ILogger<CircuitBreaker> logger)
		{
			_options = options;
			_logger = logger;
		}

		public AsyncCircuitBreakerPolicy GetPolicy()
		{
			return Policy.Handle<CircuitBreakerOpenException>()
				.CircuitBreakerAsync(_options.Value.CircuitBreakerHandledEventsAllowedBeforeBreaking,
					durationOfBreak: TimeSpan.FromSeconds(_options.Value.CircuitBreakerDurationOfBreak),
					onBreak: (ex, breakDelay) =>
					{
						_logger.LogWarning(ex,
							"The circuit breaker has tripped. Requests will be blocked for {BreakDelay} seconds.",
							breakDelay.TotalSeconds);
					},
					onReset: () =>
					{
						_logger.LogInformation("The circuit breaker has reset. Requests can now be made.");
					}
				);
		}
	}
}
