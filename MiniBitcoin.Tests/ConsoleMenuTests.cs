namespace MiniBitcoin.Tests;

public sealed class ConsoleMenuTests
{
    [Fact]
    public void RunsTradingFlowWithBothDecimalSeparators()
    {
        var (exchange, output) = RunMenu(
            "1\n2\n3\n0,001\n6\n600000\n1\n4\n0.0005\n2\n5\n0\n");

        Assert.Equal(800m, exchange.Wallet.BrlBalance);
        Assert.Equal(0.0005m, exchange.Wallet.BitcoinBalance);
        Assert.Equal(1100m, exchange.PortfolioValue);
        Assert.Contains("Cotação atual:", output);
        Assert.Contains("Cotação atualizada.", output);
        Assert.Contains("Compra realizada.", output);
        Assert.Contains("Venda realizada.", output);
        Assert.Contains("Saldo em BTC: 0,0005 BTC", output);
        Assert.Contains("Patrimônio total:", output);
        Assert.Contains("| Compra | 0,001 BTC", output);
        Assert.Contains("| Venda | 0,0005 BTC", output);
        Assert.True(output.IndexOf("| Compra |", StringComparison.Ordinal)
            < output.IndexOf("| Venda |", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("1.000,00")]
    [InlineData("1,000.00")]
    public void RejectsInvalidAmountAndKeepsMenuAvailable(string amount)
    {
        var (exchange, output) = RunMenu($"3\n{amount}\n5\n0\n");

        Assert.Equal(1000m, exchange.Wallet.BrlBalance);
        Assert.Empty(exchange.History);
        Assert.Contains("Valor inválido.", output);
        Assert.Contains("Sem transações.", output);
        Assert.Contains("Patrimônio total:", output);
    }

    [Fact]
    public void ReportsInvalidOptionAndFailedOrderThenContinues()
    {
        var (exchange, output) = RunMenu("9\n3\n1\n2\n0\n");

        Assert.Equal(1000m, exchange.Wallet.BrlBalance);
        Assert.Empty(exchange.History);
        Assert.Contains("Opção inválida.", output);
        Assert.Contains("Operação recusada.", output);
        Assert.Contains("Saldo em BRL:", output);
    }

    [Theory]
    [InlineData("")]
    [InlineData("3\n")]
    public void ExitsAtEndOfInputWithoutChangingBalance(string input)
    {
        var (exchange, output) = RunMenu(input);

        Assert.Equal(1000m, exchange.Wallet.BrlBalance);
        Assert.Empty(exchange.History);
        Assert.Contains("MiniBitcoin", output);
    }

    [Fact]
    public void KeepsTheMenuAvailableIfThePortfolioValueIsTooLarge()
    {
        var (_, output) = RunMenu("6\n0,01\n3\n100000\n6\n100000000000000\n5\n1\n0\n");

        Assert.Contains("Patrimônio indisponível.", output);
        Assert.Contains("Cotação atual:", output);
    }

    private static (BitcoinExchange Exchange, string Output) RunMenu(string commands)
    {
        var exchange = new BitcoinExchange(new Wallet(1000m, 0m), 500_000m);
        using var input = new StringReader(commands);
        using var output = new StringWriter();

        new ConsoleMenu(exchange, input, output).Run();

        return (exchange, output.ToString());
    }
}
