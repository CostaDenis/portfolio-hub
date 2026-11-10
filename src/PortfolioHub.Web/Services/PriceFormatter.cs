using System.Globalization;

namespace PortfolioHub.Web.Services;

public static class PriceFormatter
{
    // Preços unitários de ativos baratos (ex.: criptomoedas a R$ 0,003) perdem informação com 2 casas.
    // Abaixo de R$ 1 mostra até 8 casas, sem zeros sobrando: R$ 0,003 · R$ 0,07 · R$ 0,00001234.
    public static string Format(decimal price)
    {
        if (price == 0 || Math.Abs(price) >= 1)
            return price.ToString("C");

        var text = price.ToString("C8");
        var separator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
        var minLength = text.LastIndexOf(separator, StringComparison.Ordinal) + separator.Length + 2;

        var end = text.Length;
        while (end > minLength && text[end - 1] == '0')
            end--;

        return text[..end];
    }
}
