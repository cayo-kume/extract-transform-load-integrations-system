namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// centro de custo (GET fiscal/v2/cost-center, CostCenterModel; componente GECFM001). Os enums vêm pelo nome (Expense,
    /// Administrative...; testado) e a resposta não tem data de alteração
    /// </summary>
    public class CostCenter
    {
        public string? code { get; init; }
        public string? description { get; init; }
        public string? @class { get; init; }
        public string? classType { get; init; }
        public string? indirectType { get; init; }
        public string? isInactive { get; init; }
        public List<CostCenterClassification>? classifications { get; init; }
    }
}
