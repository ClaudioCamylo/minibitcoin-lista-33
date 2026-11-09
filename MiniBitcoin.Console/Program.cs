using MiniBitcoin;

var wallet = new Wallet(1000m, 0m);
var exchange = new BitcoinExchange(wallet, 500_000m);

new ConsoleMenu(exchange, Console.In, Console.Out).Run();
