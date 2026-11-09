namespace MiniBitcoin;

public sealed class BitcoinExchange
{
    public BitcoinExchange(Wallet wallet, decimal quote)
    {
        Wallet = wallet;
        Quote = quote;
    }

    public Wallet Wallet { get; }
    public decimal Quote { get; }

    public void UpdateQuote(decimal quote) => throw new NotImplementedException();
    public void Buy(decimal bitcoinAmount) => throw new NotImplementedException();
    public void Sell(decimal bitcoinAmount) => throw new NotImplementedException();
}
