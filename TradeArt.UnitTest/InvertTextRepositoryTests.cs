using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using TradeArt.Infrastructure.Repositories;
using Xunit;

namespace TradeArt.UnitTest
{
	public class InvertTextRepositoryTests
	{
		private readonly InvertTextRepository _invertTextRepository;

		public InvertTextRepositoryTests()
		{
			_invertTextRepository = new InvertTextRepository(Mock.Of<ILogger<InvertTextRepository>>());
		}

		[Theory]
		[InlineData("hello", "olleh")]
		[InlineData("12345", "54321")]
		[InlineData("racecar", "racecar")]
		public void InvertText_ValidInput_ReturnsReversedString(string input, string expectedOutput)
		{
			// Arrange
			var stringBuilder = new StringBuilder(input);

			// Act
			var result = _invertTextRepository.InvertText(stringBuilder);

			// Assert
			Assert.Equal(expectedOutput, result);
		}

		[Fact]
		public void InvertText_NullInput_ThrowsArgumentNullException()
		{
			// Arrange
			StringBuilder input = null;

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => _invertTextRepository.InvertText(input));
		}

		[Fact]
		public void InvertText_EmptyInput_ThrowsArgumentException()
		{
			// Arrange
			var input = new StringBuilder();

			// Act & Assert
			Assert.Throws<ArgumentException>(() => _invertTextRepository.InvertText(input));
		}

		[Fact]
		public void InvertText_InputExceedsMaxLength_ThrowsArgumentException()
		{
			// Arrange
			var input = new StringBuilder(new string('a', InvertTextRepository.MaxInputLength + 1));

			// Act & Assert
			Assert.Throws<ArgumentException>(() => _invertTextRepository.InvertText(input));
		}
	}
}
