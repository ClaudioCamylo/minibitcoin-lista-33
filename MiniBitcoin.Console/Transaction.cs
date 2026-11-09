namespace MiniBitcoin;

public enum TransactionType
{
    Buy,
    Sell
}

public sealed record Transaction(
    TransactionType Type,
    decimal BitcoinAmount,
    decimal Quote,
    decimal TotalBrl,
    DateTimeOffset Timestamp);
