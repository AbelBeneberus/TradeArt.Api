using Polly.CircuitBreaker;

namespace TradeArt.Infrastructure.CircuitBreaker;

public interface ICircuitBreaker
{
	AsyncCircuitBreakerPolicy GetPolicy();
}