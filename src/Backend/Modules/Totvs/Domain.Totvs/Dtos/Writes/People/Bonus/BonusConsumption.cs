namespace Domain.Totvs.Dtos.Writes.People.Bonus
{
    /// <summary>
    /// corpo do POST person/v2/bonus-consume (UpdateBonusConsumptionCommand): consome saldo de bônus do cliente numa loja. Efeito financeiro e
    /// sem estorno pela API. A pessoa vai pelo código ou pelo CPF/CNPJ (o schema marca o código, a descrição diz um ou outro)
    /// </summary>
    public class BonusConsumption
    {
        public int? personCode { get; init; }
        public string? personCpfCnpj { get; init; }
        public int? branchCode { get; init; }
        public decimal? usedValue { get; init; }
    }
}
