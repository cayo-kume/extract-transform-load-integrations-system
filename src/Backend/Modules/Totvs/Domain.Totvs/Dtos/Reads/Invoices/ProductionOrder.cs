namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// objeto 1:1 (productionOrder); vira colunas da nota com o prefixo productionOrder
    /// </summary>
    public class ProductionOrder
    {
        public string? branchCode { get; init; }
        public string? cycleCode { get; init; }
        public string? orderCode { get; init; }
        public string? orderRelationshipType { get; init; }
    }
}
