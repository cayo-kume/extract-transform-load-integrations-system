namespace Application.Totvs.Abstractions.Services
{
    /// <summary>
    /// bônus da API Person (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.3 e §3.4.5, PE9): a consulta de saldo; as escritas (vencimento e
    /// consumo) entram na fase PE-7
    /// </summary>
    public interface ITotvsBonusService
    {
        /// <summary>
        /// devolve o saldo de bônus de uma pessoa nas lojas (POST person/v2/list-balance-bonus), sem gravar
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostListBalanceBonus(string user_name, string doc_company, Domain.Totvs.Dtos.Reads.People.Bonus.ListBalanceBonus body);

        /// <summary>
        /// gera crédito de bônus para o cliente (POST person/v2/bonus-due-date). Efeito financeiro, sem estorno; só com Totvs:Writes:Bonus:Enabled
        /// e o mesmo corpo em 10 minutos volta 409 (T5)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostBonusDueDate(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.People.Bonus.BonusDueDate body);

        /// <summary>
        /// consome saldo de bônus do cliente numa loja (POST person/v2/bonus-consume). Com o documento da pessoa, o saldo é conferido antes (PE4);
        /// efeito financeiro, sem estorno; só com Totvs:Writes:Bonus:Enabled e o mesmo corpo em 10 minutos volta 409 (T5)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostBonusConsume(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.People.Bonus.BonusConsumption body);
    }
}
