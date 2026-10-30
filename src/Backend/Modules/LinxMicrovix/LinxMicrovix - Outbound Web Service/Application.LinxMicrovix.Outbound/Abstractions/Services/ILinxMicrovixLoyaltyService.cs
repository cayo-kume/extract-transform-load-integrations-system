namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Fidelidade. Os nomes dos métodos são os da API (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixLoyaltyService
    {
        /// <summary>
        /// os dados da tabela fidelidade_parceiro_log cadastrados no portal.
        /// </summary>
        public Task<bool?> LinxFidelidade();

        /// <summary>
        /// movimento_campanha_reshop.
        /// </summary>
        public Task<bool?> LinxMovimentoReshop();

        /// <summary>
        /// movimento_campanha_reshop_item.
        /// </summary>
        public Task<bool?> LinxMovimentoReshopItens();
    }
}
