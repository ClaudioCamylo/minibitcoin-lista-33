namespace MiniBitcoin;

internal static class AmountRules
{
    internal const int BrlPlaces = 2;
    internal const int BitcoinPlaces = 8;

    // A cotação e o BTC podem produzir dez casas decimais no produto.
    private const decimal MaxBrlCalculation = 7_000_000_000_000_000_000m;

    internal static decimal RoundBrl(decimal amount, MidpointRounding rounding)
    {
        if (amount > MaxBrlCalculation)
        {
            throw new OverflowException("O valor em reais excede a faixa de cálculo suportada.");
        }

        return decimal.Round(amount, BrlPlaces, rounding);
    }

    internal static void Validate(decimal amount, int places, string parameter, bool allowZero = false)
    {
        if (amount < 0m || (!allowZero && amount == 0m) || decimal.Round(amount, places) != amount)
        {
            var sign = allowZero ? "não negativo" : "positivo";
            throw new ArgumentOutOfRangeException(parameter, amount,
                $"Informe um valor {sign} com até {places} casas decimais.");
        }
    }
}
