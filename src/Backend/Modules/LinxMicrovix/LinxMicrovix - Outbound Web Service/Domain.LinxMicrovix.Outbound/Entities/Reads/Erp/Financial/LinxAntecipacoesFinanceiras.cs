using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Financial
{
    /// <summary>
    /// LinxAntecipacoesFinanceiras, tabela linxmicrovix.LinxAntecipacoesFinanceiras. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + numero_antecipacao + codigoproduto (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxAntecipacoesFinanceiras
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? empresa { get; private set; }
        public int? cod_cliente { get; private set; }
        public int numero_antecipacao { get; private set; }
        public DateTime? data_antecipacao { get; private set; }
        public DateTime? previsao_entrega { get; private set; }
        public string? dav_pv { get; private set; }
        public int? numero_origem { get; private set; }
        public int? dav_remessa { get; private set; }
        public long codigoproduto { get; private set; }
        public decimal? quantidade { get; private set; }
        public decimal? preco_unitario_produto { get; private set; }
        public decimal? valor_pago_antecipacao { get; private set; }
        public string? entregue { get; private set; }
        public Guid? identificador { get; private set; }
        public long timestamp { get; private set; }
        public bool? cancelado { get; private set; }
        public int? id_antecipacoes_financeiras { get; private set; }
        public int? id_antecipacoes_financeiras_detalhes { get; private set; }
        public int? id_vendas_pos_produtos { get; private set; }
        public decimal? valor_pagar { get; private set; }
        public Guid? identificador_faturamento_antecipado { get; private set; }

        public LinxAntecipacoesFinanceiras() { }

        public LinxAntecipacoesFinanceiras(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Financial.LinxAntecipacoesFinanceiras dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            cod_cliente = MicrovixConverters.ToNullableInt32(dto.cod_cliente);
            numero_antecipacao = MicrovixConverters.ToInt32(dto.numero_antecipacao);
            data_antecipacao = MicrovixConverters.ToNullableDateTime(dto.data_antecipacao);
            previsao_entrega = MicrovixConverters.ToNullableDateTime(dto.previsao_entrega);
            dav_pv = MicrovixConverters.ToNullableString(dto.dav_pv, 3);
            numero_origem = MicrovixConverters.ToNullableInt32(dto.numero_origem);
            dav_remessa = MicrovixConverters.ToNullableInt32(dto.dav_remessa);
            codigoproduto = MicrovixConverters.ToInt64(dto.codigoproduto);
            quantidade = MicrovixConverters.ToNullableDecimal(dto.quantidade);
            preco_unitario_produto = MicrovixConverters.ToNullableDecimal(dto.preco_unitario_produto);
            valor_pago_antecipacao = MicrovixConverters.ToNullableDecimal(dto.valor_pago_antecipacao);
            entregue = MicrovixConverters.ToNullableString(dto.entregue, 1);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            cancelado = MicrovixConverters.ToNullableBoolean(dto.cancelado);
            id_antecipacoes_financeiras = MicrovixConverters.ToNullableInt32(dto.id_antecipacoes_financeiras);
            id_antecipacoes_financeiras_detalhes = MicrovixConverters.ToNullableInt32(dto.id_antecipacoes_financeiras_detalhes);
            id_vendas_pos_produtos = MicrovixConverters.ToNullableInt32(dto.id_vendas_pos_produtos);
            valor_pagar = MicrovixConverters.ToNullableDecimal(dto.valor_pagar);
            identificador_faturamento_antecipado = MicrovixConverters.ToNullableGuid(dto.identificador_faturamento_antecipado);
        }
    }
}
