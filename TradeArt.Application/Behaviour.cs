using MediatR;
using Polly.CircuitBreaker;

namespace TradeArt.Application
{
	public class CircuitBreakerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	{
		private readonly AsyncCircuitBreakerPolicy<TResponse> _circuitBreakerPolicy;

		public CircuitBreakerBehavior(AsyncCircuitBreakerPolicy<TResponse> circuitBreakerPolicy)
		{
			_circuitBreakerPolicy = circuitBreakerPolicy;
		}

		public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
		{
			return await _circuitBreakerPolicy.ExecuteAsync(ct => next(), cancellationToken);

		}
	}
}