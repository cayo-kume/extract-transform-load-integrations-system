using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Returns
{
    /// <summary>
    /// LinxMovimentoDevolucoesItens, tabela linxmicrovix.LinxMovimentoDevolucoesItens. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_movimento_devolucoes_itens
    /// </summary>
    public class LinxMovimentoDevolucoesItens
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public Guid? identificador_venda { get; private set; }
        public Guid? identificador_devolucao { get; private set; }
        public long? codigoproduto { get; private set; }
        public int id_movimento_devolucoes_itens { get; private set; }
        public int? transacao_origem { get; private set; }
        public int? transacao_devolucao { get; private set; }
        public decimal? qtde_devolvida { get; private set; }
        public long timestamp { get; private set; }

        public LinxMovimentoDevolucoesItens() { }

        public LinxMovimentoDevolucoesItens(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Returns.LinxMovimentoDevolucoesItens dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            identificador_venda = MicrovixConverters.ToNullableGuid(dto.identificador_venda);
            identificador_devolucao = MicrovixConverters.ToNullableGuid(dto.identificador_devolucao);
            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            id_movimento_devolucoes_itens = MicrovixConverters.ToInt32(dto.id_movimento_devolucoes_itens);
            transacao_origem = MicrovixConverters.ToNullableInt32(dto.transacao_origem);
            transacao_devolucao = MicrovixConverters.ToNullableInt32(dto.transacao_devolucao);
            qtde_devolvida = MicrovixConverters.ToNullableDecimal(dto.qtde_devolvida);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
