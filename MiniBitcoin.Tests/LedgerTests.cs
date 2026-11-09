namespace MiniBitcoin.Tests;

public sealed class LedgerTests
{
    [Fact]
    public void RecordsSuccessfulOrdersInOrderUsingTheExecutedQuoteAndTotal()
    {
        var exchange = new BitcoinExchange(new Wallet(1_000m, 0m), 500_000m);
        var start = DateTimeOffset.UtcNow;

        exchange.Buy(0.001m);
        exchange.UpdateQuote(600_000m);
        exchange.Sell(0.0005m);

        var end = DateTimeOffset.UtcNow;
        Assert.Collection(
            exchange.History,
            buy =>
            {
                Assert.Equal(TransactionType.Buy, buy.Type);
                Assert.Equal(0.001m, buy.BitcoinAmount);
                Assert.Equal(500_000m, buy.Quote);
                Assert.Equal(500m, buy.TotalBrl);
                Assert.InRange(buy.Timestamp, start, end);
            },
            sell =>
            {
                Assert.Equal(TransactionType.Sell, sell.Type);
                Assert.Equal(0.0005m, sell.BitcoinAmount);
                Assert.Equal(600_000m, sell.Quote);
                Assert.Equal(300m, sell.TotalBrl);
                Assert.InRange(sell.Timestamp, start, end);
            });
    }

    [Fact]
    public void RecordsActualSettledAmountsAfterRounding()
    {
        var exchange = new BitcoinExchange(new Wallet(0.01m, 0m), 500_000.01m);
        exchange.Buy(0.00000001m);
        exchange.UpdateQuote(1_999_999.99m);
        exchange.Sell(0.00000001m);

        Assert.Equal(new[] { 0.01m, 0.01m }, exchange.History.Select(entry => entry.TotalBrl));
    }

    [Fact]
    public void DoesNotRecordRejectedOrdersOrQuoteUpdates()
    {
        var exchange = new BitcoinExchange(new Wallet(100m, 0m), 500_000m);
        Assert.Empty(exchange.History);
        exchange.Buy(0.0001m);
        var original = Assert.Single(exchange.History);

        Assert.Throws<InvalidOperationException>(() => exchange.Buy(0.001m));
        Assert.Throws<InvalidOperationException>(() => exchange.Sell(0.001m));
        exchange.UpdateQuote(600_000m);

        Assert.Equal(original, Assert.Single(exchange.History));
    }

    [Fact]
    public void HistoryCannotBeChangedThroughTheReturnedCollection()
    {
        var exchange = new BitcoinExchange(new Wallet(100m, 0m), 500_000m);
        exchange.Buy(0.0001m);

        if (exchange.History is ICollection<Transaction> collection)
        {
            Assert.Throws<NotSupportedException>(() => collection.Clear());
        }

        Assert.Single(exchange.History);
    }

    [Fact]
    public void PortfolioUsesCurrentQuoteAndIncludesAvailableReais()
    {
        var exchange = new BitcoinExchange(new Wallet(200m, 0.001m), 500_000m);
        Assert.Equal(700m, exchange.PortfolioValue);

        exchange.UpdateQuote(600_000m);
        Assert.Equal(800m, exchange.PortfolioValue);

        exchange.Sell(0.001m);
        Assert.Equal(800m, exchange.PortfolioValue);
    }

    [Fact]
    public void PortfolioRoundsHalfCentsToTheNearestEvenCent()
    {
        var exchange = new BitcoinExchange(new Wallet(100m, 0.01m), 1_000.50m);
        Assert.Equal(110m, exchange.PortfolioValue);

        exchange.UpdateQuote(1_001.50m);
        Assert.Equal(110.02m, exchange.PortfolioValue);

        var oddCent = new BitcoinExchange(new Wallet(100.01m, 0.01m), 1_000.50m);
        Assert.Equal(110.02m, oddCent.PortfolioValue);
    }
}
