namespace Domain.Totvs.Dtos.Reads.People.Persons
{
    /// <summary>
    /// estatística por empresa (só vem com option.branchStaticDataList) da pessoa física ou jurídica (API Person)
    /// </summary>
    public class Statistic
    {
        public string? branchCode { get; init; }
        public string? customerCode { get; init; }
        public string? averageDelay { get; init; }
        public string? maximumDelay { get; init; }
        public string? returnedCheck { get; init; }
        public string? installmentPaid { get; init; }
        public string? purchaseQuantity { get; init; }
        public string? higherDebt { get; init; }
        public string? limitValue { get; init; }
        public string? biggestPurchase { get; init; }
        public string? firstPurchase { get; init; }
        public string? lastPurchase { get; init; }
        public string? largestAccumulationDate { get; init; }
        public string? biggestPurchaseDate { get; init; }
        public string? firstPurchaseDate { get; init; }
        public string? lastPurchaseDate { get; init; }
        public string? lastCardDate { get; init; }
        public string? totalPurchase { get; init; }
    }
}
