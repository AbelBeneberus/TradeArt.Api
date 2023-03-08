namespace TradeArt.Application.Exceptions
{
	public class CircuitBreakerOpenException : Exception
	{
		public CircuitBreakerOpenException() : base("Circuit breaker is open")
		{
		}
	}
}