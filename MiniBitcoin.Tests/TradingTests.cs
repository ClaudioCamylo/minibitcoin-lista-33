namespace MiniBitcoin.Tests;

public sealed class TradingTests
{
    [Fact]
    public void BuyUsesCurrentQuoteAndUpdatesBothBalances()
    {
        var wallet = new Wallet(1_000m, 0.001m);
        var exchange = new BitcoinExchange(wallet, 400_000m);
        exchange.UpdateQuote(500_000m);

        exchange.Buy(0.002m);

        Assert.Equal(500_000m, exchange.Quote);
        Assert.Equal(0m, wallet.BrlBalance);
        Assert.Equal(0.003m, wallet.BitcoinBalance);
    }

    [Fact]
    public void SellUsesCurrentQuoteAndUpdatesBothBalances()
    {
        var wallet = new Wallet(100m, 0.002m);
        var exchange = new BitcoinExchange(wallet, 400_000m);
        exchange.UpdateQuote(600_000m);

        exchange.Sell(0.002m);

        Assert.Equal(1_300m, wallet.BrlBalance);
        Assert.Equal(0m, wallet.BitcoinBalance);
    }

    [Fact]
    public void BuyRejectsInsufficientReaisWithoutChangingBalances()
    {
        var wallet = new Wallet(999.99m, 0.001m);
        var exchange = new BitcoinExchange(wallet, 500_000m);

        Assert.Throws<InvalidOperationException>(() => exchange.Buy(0.002m));

        Assert.Equal(999.99m, wallet.BrlBalance);
        Assert.Equal(0.001m, wallet.BitcoinBalance);
    }

    [Fact]
    public void SellRejectsInsufficientBitcoinWithoutChangingBalances()
    {
        var wallet = new Wallet(100m, 0.001m);
        var exchange = new BitcoinExchange(wallet, 500_000m);

        Assert.Throws<InvalidOperationException>(() => exchange.Sell(0.002m));

        Assert.Equal(100m, wallet.BrlBalance);
        Assert.Equal(0.001m, wallet.BitcoinBalance);
    }

    [Fact]
    public void OrdersRejectZeroNegativeAndSubSatoshiAmounts()
    {
        var wallet = new Wallet(1_000m, 0.002m);
        var exchange = new BitcoinExchange(wallet, 500_000m);

        foreach (var amount in new[] { 0m, -0.001m, 0.000000001m })
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => exchange.Buy(amount));
            Assert.Throws<ArgumentOutOfRangeException>(() => exchange.Sell(amount));
        }

        Assert.Equal(1_000m, wallet.BrlBalance);
        Assert.Equal(0.002m, wallet.BitcoinBalance);
    }

    [Fact]
    public void QuotesMustBePositiveAndHaveCentPrecision()
    {
        var wallet = new Wallet(1_000m, 0m);
        var exchange = new BitcoinExchange(wallet, 500_000m);

        foreach (var quote in new[] { 0m, -1m, 500_000.001m })
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new BitcoinExchange(wallet, quote));
            Assert.Throws<ArgumentOutOfRangeException>(() => exchange.UpdateQuote(quote));
            Assert.Equal(500_000m, exchange.Quote);
        }
    }

    [Fact]
    public void BuyingOneSatoshiRoundsCostUpToOneCent()
    {
        var wallet = new Wallet(0.01m, 0m);
        var exchange = new BitcoinExchange(wallet, 500_000.01m);

        exchange.Buy(0.00000001m);

        Assert.Equal(0m, wallet.BrlBalance);
        Assert.Equal(0.00000001m, wallet.BitcoinBalance);
    }

    [Fact]
    public void SellingOneSatoshiRoundsProceedsDownToOneCent()
    {
        var wallet = new Wallet(0m, 0.00000001m);
        var exchange = new BitcoinExchange(wallet, 1_999_999.99m);

        exchange.Sell(0.00000001m);

        Assert.Equal(0.01m, wallet.BrlBalance);
        Assert.Equal(0m, wallet.BitcoinBalance);
    }

    [Fact]
    public void SellRejectsAnOrderThatWouldCreditZeroCents()
    {
        var wallet = new Wallet(0m, 0.00000001m);
        var exchange = new BitcoinExchange(wallet, 999_999.99m);

        Assert.Throws<InvalidOperationException>(() => exchange.Sell(0.00000001m));

        Assert.Equal(0m, wallet.BrlBalance);
        Assert.Equal(0.00000001m, wallet.BitcoinBalance);
    }

    [Fact]
    public void BuyDoesNotDebitReaisIfBitcoinCreditOverflows()
    {
        var wallet = new Wallet(1m, decimal.MaxValue);
        var exchange = new BitcoinExchange(wallet, 1m);

        Assert.Throws<OverflowException>(() => exchange.Buy(1m));

        Assert.Equal(1m, wallet.BrlBalance);
        Assert.Equal(decimal.MaxValue, wallet.BitcoinBalance);
    }

    [Fact]
    public void SellDoesNotDebitBitcoinIfReaisCreditOverflows()
    {
        var wallet = new Wallet(decimal.MaxValue, 1m);
        var exchange = new BitcoinExchange(wallet, 1m);

        Assert.Throws<OverflowException>(() => exchange.Sell(1m));

        Assert.Equal(decimal.MaxValue, wallet.BrlBalance);
        Assert.Equal(1m, wallet.BitcoinBalance);
    }
}
