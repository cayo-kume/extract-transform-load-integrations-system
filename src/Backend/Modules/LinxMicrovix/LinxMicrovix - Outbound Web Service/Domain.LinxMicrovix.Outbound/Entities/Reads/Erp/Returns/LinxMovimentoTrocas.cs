using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Returns
{
    /// <summary>
    /// LinxMovimentoTrocas, tabela linxmicrovix.LinxMovimentoTrocas. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + identificador + num_vale
    /// </summary>
    public class LinxMovimentoTrocas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public Guid identificador { get; private set; }
        public long num_vale { get; private set; }
        public decimal? valor_vale { get; private set; }
        public string? motivo { get; private set; }
        public int? doc_origem { get; private set; }
        public string? serie_origem { get; private set; }
        public int? doc_venda { get; private set; }
        public string? serie_venda { get; private set; }
        public bool? excluido { get; private set; }
        public long timestamp { get; private set; }
        public bool? desfazimento { get; private set; }
        public int? empresa { get; private set; }
        public int? vale_cod_cliente { get; private set; }
        public long? vale_codigoproduto { get; private set; }
        public int? id_vale_ordem_servico_externa { get; private set; }
        public int? doc_venda_origem { get; private set; }
        public string? serie_venda_origem { get; private set; }
        public int? cod_cliente { get; private set; }
        public DateTime? data_vencimento { get; private set; }
        public decimal? valor_original { get; private set; }
        public int? id_antecipacoes_financeiras { get; private set; }
        public string? obs_troca { get; private set; }

        public LinxMovimentoTrocas() { }

        public LinxMovimentoTrocas(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Returns.LinxMovimentoTrocas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador) ?? Guid.Empty;
            num_vale = MicrovixConverters.ToInt64(dto.num_vale);
            valor_vale = MicrovixConverters.ToNullableDecimal(dto.valor_vale);
            motivo = MicrovixConverters.ToNullableString(dto.motivo, 50);
            doc_origem = MicrovixConverters.ToNullableInt32(dto.doc_origem);
            serie_origem = MicrovixConverters.ToNullableString(dto.serie_origem, 10);
            doc_venda = MicrovixConverters.ToNullableInt32(dto.doc_venda);
            serie_venda = MicrovixConverters.ToNullableString(dto.serie_venda, 10);
            excluido = MicrovixConverters.ToNullableBoolean(dto.excluido);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            desfazimento = MicrovixConverters.ToNullableBoolean(dto.desfazimento);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            vale_cod_cliente = MicrovixConverters.ToNullableInt32(dto.vale_cod_cliente);
            vale_codigoproduto = MicrovixConverters.ToNullableInt64(dto.vale_codigoproduto);
            id_vale_ordem_servico_externa = MicrovixConverters.ToNullableInt32(dto.id_vale_ordem_servico_externa);
            doc_venda_origem = MicrovixConverters.ToNullableInt32(dto.doc_venda_origem);
            serie_venda_origem = MicrovixConverters.ToNullableString(dto.serie_venda_origem, 10);
            cod_cliente = MicrovixConverters.ToNullableInt32(dto.cod_cliente);
            data_vencimento = MicrovixConverters.ToNullableDateTime(dto.data_vencimento);
            valor_original = MicrovixConverters.ToNullableDecimal(dto.valor_original);
            id_antecipacoes_financeiras = MicrovixConverters.ToNullableInt32(dto.id_antecipacoes_financeiras);
            obs_troca = MicrovixConverters.ToNullableString(dto.obs_troca, 180);
        }
    }
}
