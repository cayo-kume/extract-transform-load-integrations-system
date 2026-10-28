namespace Application.Totvs.Abstractions.Services
{
    /// <summary>
    /// bônus da API Person (docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.3 e §3.4.5, PE9): a consulta de saldo; as escritas (vencimento e
    /// consumo) entram na fase PE-7
    /// </summary>
    public interface ITotvsBonusService
    {
        /// <summary>
        /// devolve o saldo de bônus de uma pessoa nas lojas (POST person/v2/list-balance-bonus), sem gravar
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostListBalanceBonus(string user_name, string doc_company, Domain.Totvs.Dtos.Reads.People.Bonus.ListBalanceBonus body);
    }
}
