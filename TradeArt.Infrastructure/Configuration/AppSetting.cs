namespace TradeArt.Infrastructure.Configuration
{
	public class AppSetting
	{
		public string GraphQlEndpoint { get; set; } = string.Empty;
		public int CircuitBreakerHandledEventsAllowedBeforeBreaking { get; set; }
		public int CircuitBreakerDurationOfBreak { get; set; }
		public long ChunkSize { get; set; } 

	}
}
