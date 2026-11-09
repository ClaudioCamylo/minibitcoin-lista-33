namespace MiniBitcoin;

public sealed class Wallet
{
    public Wallet(decimal brlBalance, decimal bitcoinBalance)
    {
        AmountRules.Validate(brlBalance, AmountRules.BrlPlaces, nameof(brlBalance), allowZero: true);
        AmountRules.Validate(bitcoinBalance, AmountRules.BitcoinPlaces, nameof(bitcoinBalance), allowZero: true);
        BrlBalance = brlBalance;
        BitcoinBalance = bitcoinBalance;
    }

    public decimal BrlBalance { get; private set; }
    public decimal BitcoinBalance { get; private set; }

    public void DebitBrl(decimal amount)
    {
        AmountRules.Validate(amount, AmountRules.BrlPlaces, nameof(amount));
        BrlBalance = BalanceAfterDebit(BrlBalance, amount);
    }

    public void DebitBitcoin(decimal amount)
    {
        AmountRules.Validate(amount, AmountRules.BitcoinPlaces, nameof(amount));
        BitcoinBalance = BalanceAfterDebit(BitcoinBalance, amount);
    }

    private static decimal BalanceAfterDebit(decimal balance, decimal amount)
    {
        if (amount > balance)
        {
            throw new InvalidOperationException("Saldo insuficiente.");
        }

        var remaining = balance - amount;
        if (balance - remaining != amount)
        {
            throw new OverflowException("O saldo resultante excede a precisão suportada.");
        }

        return remaining;
    }
}
