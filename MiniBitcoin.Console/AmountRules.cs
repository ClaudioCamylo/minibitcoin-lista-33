namespace MiniBitcoin;

internal static class AmountRules
{
    internal const int BrlPlaces = 2;
    internal const int BitcoinPlaces = 8;

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
