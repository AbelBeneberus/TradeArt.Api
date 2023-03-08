namespace TradeArt.Domain.Exceptions;

public class FileHashCalculationException : Exception
{
	public FileHashCalculationException(string message, Exception exception) : base(message, exception)
	{

	}
}