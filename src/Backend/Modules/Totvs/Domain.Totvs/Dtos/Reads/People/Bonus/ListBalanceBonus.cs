namespace Domain.Totvs.Dtos.Reads.People.Bonus
{
    /// <summary>
    /// consulta do saldo de bônus (POST person/v2/list-balance-bonus, BalanceBonusQuery), recebida pela rota e enviada à Totvs como veio
    /// </summary>
    public class ListBalanceBonus
    {
        // CPF (11) ou CNPJ (14) da pessoa
        public string? personCpf { get; init; }

        public List<ListBalanceBonusBranch>? branchList { get; init; }
    }

    /// <summary>
    /// loja da consulta: branchCode ou branchCnpj, um dos dois
    /// </summary>
    public class ListBalanceBonusBranch
    {
        public int? branchCode { get; init; }
        public string? branchCnpj { get; init; }
    }
}
