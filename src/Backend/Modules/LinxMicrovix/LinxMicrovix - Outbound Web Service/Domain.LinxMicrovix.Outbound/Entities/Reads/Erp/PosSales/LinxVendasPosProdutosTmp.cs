using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.PosSales
{
    /// <summary>
    /// LinxVendasPosProdutosTmp, tabela linxmicrovix.LinxVendasPosProdutosTmp. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_vendas_pos_produtos
    /// </summary>
    public class LinxVendasPosProdutosTmp
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public long id_vendas_pos_produtos { get; private set; }
        public long? numero_ficha { get; private set; }
        public int? empresa { get; private set; }
        public long? codigoproduto { get; private set; }
        public decimal? preco_unitario { get; private set; }
        public decimal? preco_tabela { get; private set; }
        public decimal? quantidade { get; private set; }
        public int? vendedor { get; private set; }
        public decimal? desconto_item { get; private set; }
        public int? id_tabela_preco { get; private set; }
        public decimal? altura { get; private set; }
        public decimal? comprimento { get; private set; }
        public decimal? largura { get; private set; }
        public string? cfop_item { get; private set; }
        public bool? excluido { get; private set; }
        public decimal? aliq_icms { get; private set; }
        public string? cst_fiscal { get; private set; }
        public int? indice_ordenacao_espelho { get; private set; }
        public int? id_vendas_pos { get; private set; }
        public int? id_cst_icms_fiscal { get; private set; }
        public int? id_cst_ipi_fiscal { get; private set; }
        public int? id_cst_pis_fiscal { get; private set; }
        public int? id_cst_cofins_fiscal { get; private set; }
        public decimal? aliq_iss { get; private set; }
        public int? id_cfop_fiscal { get; private set; }
        public decimal? aliq_pis { get; private set; }
        public decimal? aliq_cofins { get; private set; }
        public decimal? desconto_item_valor { get; private set; }
        public string? lote { get; private set; }
        public int? deposito { get; private set; }
        public DateTime? previsao_entrega { get; private set; }
        public int? transacao_movimento_reserva { get; private set; }
        public DateTime? data_exclusao { get; private set; }
        public decimal? desconto_total_item { get; private set; }
        public int? id_motivo_desconto { get; private set; }
        public string? motivo_desconto_observacao { get; private set; }
        public string? crm_cliente_indicador { get; private set; }
        public int? cod_cliente_indicador { get; private set; }
        public long timestamp { get; private set; }

        public LinxVendasPosProdutosTmp() { }

        public LinxVendasPosProdutosTmp(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.PosSales.LinxVendasPosProdutosTmp dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_vendas_pos_produtos = MicrovixConverters.ToInt64(dto.id_vendas_pos_produtos);
            numero_ficha = MicrovixConverters.ToNullableInt64(dto.numero_ficha);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            preco_unitario = MicrovixConverters.ToNullableDecimal(dto.preco_unitario);
            preco_tabela = MicrovixConverters.ToNullableDecimal(dto.preco_tabela);
            quantidade = MicrovixConverters.ToNullableDecimal(dto.quantidade);
            vendedor = MicrovixConverters.ToNullableInt32(dto.vendedor);
            desconto_item = MicrovixConverters.ToNullableDecimal(dto.desconto_item);
            id_tabela_preco = MicrovixConverters.ToNullableInt32(dto.id_tabela_preco);
            altura = MicrovixConverters.ToNullableDecimal(dto.altura);
            comprimento = MicrovixConverters.ToNullableDecimal(dto.comprimento);
            largura = MicrovixConverters.ToNullableDecimal(dto.largura);
            cfop_item = MicrovixConverters.ToNullableString(dto.cfop_item, 10);
            excluido = MicrovixConverters.ToNullableBoolean(dto.excluido);
            aliq_icms = MicrovixConverters.ToNullableDecimal(dto.aliq_icms);
            cst_fiscal = MicrovixConverters.ToNullableString(dto.cst_fiscal, 4);
            indice_ordenacao_espelho = MicrovixConverters.ToNullableInt32(dto.indice_ordenacao_espelho);
            id_vendas_pos = MicrovixConverters.ToNullableInt32(dto.id_vendas_pos);
            id_cst_icms_fiscal = MicrovixConverters.ToNullableInt32(dto.id_cst_icms_fiscal);
            id_cst_ipi_fiscal = MicrovixConverters.ToNullableInt32(dto.id_cst_ipi_fiscal);
            id_cst_pis_fiscal = MicrovixConverters.ToNullableInt32(dto.id_cst_pis_fiscal);
            id_cst_cofins_fiscal = MicrovixConverters.ToNullableInt32(dto.id_cst_cofins_fiscal);
            aliq_iss = MicrovixConverters.ToNullableDecimal(dto.aliq_iss);
            id_cfop_fiscal = MicrovixConverters.ToNullableInt32(dto.id_cfop_fiscal);
            aliq_pis = MicrovixConverters.ToNullableDecimal(dto.aliq_pis);
            aliq_cofins = MicrovixConverters.ToNullableDecimal(dto.aliq_cofins);
            desconto_item_valor = MicrovixConverters.ToNullableDecimal(dto.desconto_item_valor);
            lote = MicrovixConverters.ToNullableString(dto.lote, 60);
            deposito = MicrovixConverters.ToNullableInt32(dto.deposito);
            previsao_entrega = MicrovixConverters.ToNullableDateTime(dto.previsao_entrega);
            transacao_movimento_reserva = MicrovixConverters.ToNullableInt32(dto.transacao_movimento_reserva);
            data_exclusao = MicrovixConverters.ToNullableDateTime(dto.data_exclusao);
            desconto_total_item = MicrovixConverters.ToNullableDecimal(dto.desconto_total_item);
            id_motivo_desconto = MicrovixConverters.ToNullableInt32(dto.id_motivo_desconto);
            motivo_desconto_observacao = MicrovixConverters.ToNullableString(dto.motivo_desconto_observacao, 500);
            crm_cliente_indicador = MicrovixConverters.ToNullableString(dto.crm_cliente_indicador, 18);
            cod_cliente_indicador = MicrovixConverters.ToNullableInt32(dto.cod_cliente_indicador);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
