namespace MiniBitcoin.Tests;

public class WalletTests
{
    [Fact]
    public void StartsWithTheProvidedBalances()
    {
        var wallet = new Wallet(12.34m, 0.00000001m);

        Assert.Equal(12.34m, wallet.BrlBalance);
        Assert.Equal(0.00000001m, wallet.BitcoinBalance);
    }

    [Theory]
    [InlineData("-1", "0")]
    [InlineData("0", "-0.00000001")]
    [InlineData("0.001", "0")]
    [InlineData("0", "0.000000001")]
    public void RejectsNegativeOrOverpreciseOpeningBalances(string brl, string bitcoin)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Wallet(Parse(brl), Parse(bitcoin)));
    }

    [Fact]
    public void AllowsDebitingExactlyTheAvailableBalanceInEachCurrency()
    {
        var wallet = new Wallet(50m, 0.01m);

        wallet.DebitBrl(50m);
        wallet.DebitBitcoin(0.01m);

        Assert.Equal(0m, wallet.BrlBalance);
        Assert.Equal(0m, wallet.BitcoinBalance);
    }

    [Theory]
    [InlineData("0", "0")]
    [InlineData("-1", "-0.01")]
    [InlineData("0.001", "0.000000001")]
    public void RejectsInvalidDebitAmountsWithoutChangingBalances(string brl, string bitcoin)
    {
        var wallet = new Wallet(50m, 0.01m);

        Assert.Throws<ArgumentOutOfRangeException>(() => wallet.DebitBrl(Parse(brl)));
        Assert.Throws<ArgumentOutOfRangeException>(() => wallet.DebitBitcoin(Parse(bitcoin)));
        Assert.Equal(50m, wallet.BrlBalance);
        Assert.Equal(0.01m, wallet.BitcoinBalance);
    }

    [Fact]
    public void RejectsDebitsLargerThanTheAvailableBalances()
    {
        var wallet = new Wallet(50m, 0.01m);

        Assert.Throws<InvalidOperationException>(() => wallet.DebitBrl(50.01m));
        Assert.Throws<InvalidOperationException>(() => wallet.DebitBitcoin(0.01000001m));
        Assert.Equal(50m, wallet.BrlBalance);
        Assert.Equal(0.01m, wallet.BitcoinBalance);
    }

    private static decimal Parse(string amount) =>
        decimal.Parse(amount, System.Globalization.CultureInfo.InvariantCulture);
}
