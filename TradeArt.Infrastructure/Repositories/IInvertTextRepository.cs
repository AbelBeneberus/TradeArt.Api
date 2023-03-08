using System.Text;

namespace TradeArt.Infrastructure.Repositories
{
    public interface IInvertTextRepository
    {
        string InvertText(StringBuilder stringBuilder);
    }
}
