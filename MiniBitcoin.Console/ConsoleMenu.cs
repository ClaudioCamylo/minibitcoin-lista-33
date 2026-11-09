using System.Globalization;

namespace MiniBitcoin;

public sealed class ConsoleMenu(BitcoinExchange exchange, TextReader input, TextWriter output)
{
    private static readonly CultureInfo BrazilianCulture = CultureInfo.GetCultureInfo("pt-BR");
    private const NumberStyles DecimalInput = NumberStyles.AllowLeadingWhite
        | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;

    public void Run()
    {
        while (true)
        {
            output.WriteLine("""
                === MiniBitcoin ===
                1. Consultar cotação
                2. Ver saldos
                3. Comprar Bitcoin
                4. Vender Bitcoin
                5. Exibir extrato e patrimônio
                6. Atualizar cotação simulada
                0. Sair
                """);
            output.Write("Opção: ");

            var option = input.ReadLine()?.Trim();
            switch (option)
            {
                case null:
                case "0":
                    return;
                case "1":
                    output.WriteLine($"Cotação atual: {Brl(exchange.Quote)} por BTC.");
                    break;
                case "2":
                    output.WriteLine($"Saldo em BRL: {Brl(exchange.Wallet.BrlBalance)}");
                    output.WriteLine($"Saldo em BTC: {Btc(exchange.Wallet.BitcoinBalance)}");
                    break;
                case "3":
                case "4":
                case "6":
                    if (!ApplyAmount(option))
                    {
                        return;
                    }
                    break;
                case "5":
                    ShowStatement();
                    break;
                default:
                    output.WriteLine("Opção inválida.");
                    break;
            }

            output.WriteLine();
        }
    }

    private bool ApplyAmount(string option)
    {
        (string Prompt, Action<decimal> Apply, string Success) operation = option switch
        {
            "3" => ("Quantidade a comprar em BTC: ", exchange.Buy, "Compra realizada."),
            "4" => ("Quantidade a vender em BTC: ", exchange.Sell, "Venda realizada."),
            _ => ("Nova cotação em BRL por BTC: ", exchange.UpdateQuote, "Cotação atualizada.")
        };

        output.Write(operation.Prompt);
        var text = input.ReadLine();
        if (text is null)
        {
            return false;
        }

        if (!decimal.TryParse(text, DecimalInput, BrazilianCulture, out var amount)
            && !decimal.TryParse(text, DecimalInput, CultureInfo.InvariantCulture, out amount))
        {
            output.WriteLine("Valor inválido. Use vírgula ou ponto decimal, sem separador de milhar.");
            return true;
        }

        try
        {
            operation.Apply(amount);
            output.WriteLine(operation.Success);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or OverflowException)
        {
            output.WriteLine($"Operação recusada. {exception.Message}");
        }

        return true;
    }

    private void ShowStatement()
    {
        output.WriteLine("Extrato de transações");
        var history = exchange.History;
        if (history.Count == 0)
        {
            output.WriteLine("Sem transações.");
        }

        foreach (var transaction in history)
        {
            var operation = transaction.Type == TransactionType.Buy ? "Compra" : "Venda";
            var timestamp = transaction.Timestamp.ToString("dd/MM/yyyy HH:mm:ss zzz", BrazilianCulture);
            output.WriteLine($"{timestamp} | {operation} | {Btc(transaction.BitcoinAmount)}"
                + $" | Cotação: {Brl(transaction.Quote)}/BTC | Total: {Brl(transaction.TotalBrl)}");
        }

        try
        {
            output.WriteLine($"Patrimônio total: {Brl(exchange.PortfolioValue)}");
        }
        catch (OverflowException)
        {
            output.WriteLine("Patrimônio indisponível. O valor excede a faixa de cálculo suportada.");
        }
    }

    private static string Brl(decimal value) => value.ToString("C2", BrazilianCulture);

    private static string Btc(decimal value) => value.ToString("0.########", BrazilianCulture) + " BTC";
}
