namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Vendas POS temporárias. Os nomes dos métodos são os da API (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixPosSalesService
    {
        /// <summary>
        /// os registros da tabela vendas_pos_tmp
        /// </summary>
        public Task<bool?> LinxVendasPosTmp();

        /// <summary>
        /// os registros da tabela vendas_pos_produtos_tmp.
        /// </summary>
        public Task<bool?> LinxVendasPosProdutosTmp();

        /// <summary>
        /// os registros da tabela vendas_pos_agrupamento_tmp
        /// </summary>
        public Task<bool?> LinxVendasPosAgrupamentoTmp();
    }
}
