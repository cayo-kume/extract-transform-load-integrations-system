using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Returns
{
    /// <summary>
    /// LinxTrocaUnificadaResumoVendasItens, tabela linxmicrovix.LinxTrocaUnificadaResumoVendasItens. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_troca_unificada_resumo_vendas_itens
    /// </summary>
    public class LinxTrocaUnificadaResumoVendasItens
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public long id_troca_unificada_resumo_vendas_itens { get; private set; }
        public long? id_troca_unificada_resumo_vendas { get; private set; }
        public long? codigoproduto { get; private set; }
        public int? transacao { get; private set; }
        public string? serial { get; private set; }
        public decimal? valor_liquido { get; private set; }
        public DateTime? data_validade { get; private set; }
        public bool? venda_referenciada { get; private set; }
        public bool? token_utilizado { get; private set; }
        public decimal? quantidade { get; private set; }
        public long timestamp { get; private set; }

        public LinxTrocaUnificadaResumoVendasItens() { }

        public LinxTrocaUnificadaResumoVendasItens(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Returns.LinxTrocaUnificadaResumoVendasItens dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_troca_unificada_resumo_vendas_itens = MicrovixConverters.ToInt64(dto.id_troca_unificada_resumo_vendas_itens);
            id_troca_unificada_resumo_vendas = MicrovixConverters.ToNullableInt64(dto.id_troca_unificada_resumo_vendas);
            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            transacao = MicrovixConverters.ToNullableInt32(dto.transacao);
            serial = MicrovixConverters.ToNullableString(dto.serial, 50);
            valor_liquido = MicrovixConverters.ToNullableDecimal(dto.valor_liquido);
            data_validade = MicrovixConverters.ToNullableDateTime(dto.data_validade);
            venda_referenciada = MicrovixConverters.ToNullableBoolean(dto.venda_referenciada);
            token_utilizado = MicrovixConverters.ToNullableBoolean(dto.token_utilizado);
            quantidade = MicrovixConverters.ToNullableDecimal(dto.quantidade);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
