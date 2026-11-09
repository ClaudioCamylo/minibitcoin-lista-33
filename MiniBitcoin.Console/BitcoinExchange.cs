namespace MiniBitcoin;

public sealed class BitcoinExchange
{
    private readonly List<Transaction> _history = [];

    public BitcoinExchange(Wallet wallet, decimal quote)
    {
        ArgumentNullException.ThrowIfNull(wallet);
        Wallet = wallet;
        UpdateQuote(quote);
        History = _history.AsReadOnly();
    }

    public Wallet Wallet { get; }
    public decimal Quote { get; private set; }
    public IReadOnlyList<Transaction> History { get; }
    public decimal PortfolioValue => AmountRules.RoundBrl(
        Wallet.BrlBalance + Wallet.BitcoinBalance * Quote, MidpointRounding.ToEven);

    public void UpdateQuote(decimal quote)
    {
        AmountRules.Validate(quote, AmountRules.BrlPlaces, nameof(quote));
        Quote = quote;
    }

    public void Buy(decimal bitcoinAmount)
    {
        AmountRules.Validate(bitcoinAmount, AmountRules.BitcoinPlaces, nameof(bitcoinAmount));
        var totalBrl = AmountRules.RoundBrl(bitcoinAmount * Quote, MidpointRounding.ToPositiveInfinity);
        Wallet.ApplyPurchase(totalBrl, bitcoinAmount);
        _history.Add(new Transaction(TransactionType.Buy, bitcoinAmount, Quote, totalBrl, DateTimeOffset.UtcNow));
    }

    public void Sell(decimal bitcoinAmount)
    {
        AmountRules.Validate(bitcoinAmount, AmountRules.BitcoinPlaces, nameof(bitcoinAmount));
        var totalBrl = AmountRules.RoundBrl(bitcoinAmount * Quote, MidpointRounding.ToZero);
        if (totalBrl == 0m)
        {
            throw new InvalidOperationException("A venda deve render pelo menos R$ 0,01.");
        }

        Wallet.ApplySale(totalBrl, bitcoinAmount);
        _history.Add(new Transaction(TransactionType.Sell, bitcoinAmount, Quote, totalBrl, DateTimeOffset.UtcNow));
    }
}
