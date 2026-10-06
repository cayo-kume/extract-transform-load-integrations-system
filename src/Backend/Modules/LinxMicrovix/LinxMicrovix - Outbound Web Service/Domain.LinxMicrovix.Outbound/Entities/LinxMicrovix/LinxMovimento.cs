
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxMovimento
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? portal { get; private set; }
        public string? cnpj_emp { get; private set; }
        public Int32? transacao { get; private set; }
        public Int32? usuario { get; private set; }
        public Int32? documento { get; private set; }
        public string? chave_nf { get; private set; }
        public Int32? ecf { get; private set; }
        public string? numero_serie_ecf { get; private set; }
        public Int32? modelo_nf { get; private set; }
        public DateTime? data_documento { get; private set; }
        public DateTime? data_lancamento { get; private set; }
        public Int32? codigo_cliente { get; private set; }
        public string? serie { get; private set; }
        public string? desc_cfop { get; private set; }
        public string? id_cfop { get; private set; }
        public Int32? cod_vendedor { get; private set; }
        public decimal? quantidade { get; private set; }
        public decimal? preco_custo { get; private set; }
        public decimal? valor_liquido { get; private set; }
        public decimal? desconto { get; private set; }
        public string? cst_icms { get; private set; }
        public string? cst_pis { get; private set; }
        public string? cst_cofins { get; private set; }
        public string? cst_ipi { get; private set; }
        public decimal? valor_icms { get; private set; }
        public decimal? aliquota_icms { get; private set; }
        public decimal? base_icms { get; private set; }
        public decimal? valor_pis { get; private set; }
        public decimal? aliquota_pis { get; private set; }
        public decimal? base_pis { get; private set; }
        public decimal? valor_cofins { get; private set; }
        public decimal? aliquota_cofins { get; private set; }
        public decimal? base_cofins { get; private set; }
        public decimal? valor_icms_st { get; private set; }
        public decimal? aliquota_icms_st { get; private set; }
        public decimal? base_icms_st { get; private set; }
        public decimal? valor_ipi { get; private set; }
        public decimal? aliquota_ipi { get; private set; }
        public decimal? base_ipi { get; private set; }
        public decimal? valor_total { get; private set; }
        public Int32? forma_dinheiro { get; private set; }
        public decimal? total_dinheiro { get; private set; }
        public Int32? forma_cheque { get; private set; }
        public decimal? total_cheque { get; private set; }
        public Int32? forma_cartao { get; private set; }
        public decimal? total_cartao { get; private set; }
        public Int32? forma_crediario { get; private set; }
        public decimal? total_crediario { get; private set; }
        public Int32? forma_convenio { get; private set; }
        public decimal? total_convenio { get; private set; }
        public decimal? frete { get; private set; }
        public string? operacao { get; private set; }
        public string? tipo_transacao { get; private set; }
        public Int64? cod_produto { get; private set; }
        public string? cod_barra { get; private set; }
        public string? cancelado { get; private set; }
        public string? excluido { get; private set; }
        public string? soma_relatorio { get; private set; }
        public Guid? identificador { get; private set; }
        public string? deposito { get; private set; }
        public string? obs { get; private set; }
        public decimal? preco_unitario { get; private set; }
        public string? hora_lancamento { get; private set; }
        public string? natureza_operacao { get; private set; }
        public Int32? tabela_preco { get; private set; }
        public string? nome_tabela_preco { get; private set; }
        public Int32? cod_sefaz_situacao { get; private set; }
        public string? desc_sefaz_situacao { get; private set; }
        public string? protocolo_aut_nfe { get; private set; }
        public DateTime? dt_update { get; private set; }
        public Int32? forma_cheque_prazo { get; private set; }
        public decimal? total_cheque_prazo { get; private set; }
        public string? cod_natureza_operacao { get; private set; }
        public decimal? preco_tabela_epoca { get; private set; }
        public decimal? desconto_total_item { get; private set; }
        public string? conferido { get; private set; }
        public Int32? transacao_pedido_venda { get; private set; }
        public string? codigo_modelo_nf { get; private set; }
        public decimal? acrescimo { get; private set; }
        public bool? mob_checkout { get; private set; }
        public decimal? aliquota_iss { get; private set; }
        public decimal? base_iss { get; private set; }
        public Int32? ordem { get; private set; }
        public Int32? codigo_rotina_origem { get; private set; }
        public Int64? timestamp { get; private set; }
        public decimal? troco { get; private set; }
        public Int32? transportador { get; private set; }
        public decimal? icms_aliquota_desonerado { get; private set; }
        public decimal? icms_valor_desonerado_item { get; private set; }
        public Int32? empresa { get; private set; }
        public decimal? desconto_item { get; private set; }
        public decimal? aliq_iss { get; private set; }
        public decimal? iss_base_item { get; private set; }
        public decimal? despesas { get; private set; }
        public decimal? seguro_total_item { get; private set; }
        public decimal? acrescimo_total_item { get; private set; }
        public decimal? despesas_total_item { get; private set; }
        public Int32? forma_pix { get; private set; }
        public decimal? total_pix { get; private set; }
        public Int32? forma_deposito_bancario { get; private set; }
        public decimal? total_deposito_bancario { get; private set; }
        public Int32? id_venda_produto_b2c { get; private set; }
        public bool? item_promocional { get; private set; }
        public decimal? acrescimo_item { get; private set; }
        public decimal? icms_st_antecipado_aliquota { get; private set; }
        public decimal? icms_st_antecipado_margem { get; private set; }
        public decimal? icms_st_antecipado_percentual_reducao { get; private set; }
        public decimal? icms_st_antecipado_valor_item { get; private set; }
        public decimal? icms_base_desonerado_item { get; private set; }
        public string? codigo_status_nfe { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxMovimento() { }

        /// <param name="listValidations"></param>
        /// <param name="portal"></param>
        /// <param name="cnpj_emp"></param>
        /// <param name="transacao"></param>
        /// <param name="usuario"></param>
        /// <param name="documento"></param>
        /// <param name="chave_nf"></param>
        /// <param name="ecf"></param>
        /// <param name="numero_serie_ecf"></param>
        /// <param name="modelo_nf"></param>
        /// <param name="data_documento"></param>
        /// <param name="data_lancamento"></param>
        /// <param name="codigo_cliente"></param>
        /// <param name="serie"></param>
        /// <param name="desc_cfop"></param>
        /// <param name="id_cfop"></param>
        /// <param name="cod_vendedor"></param>
        /// <param name="quantidade"></param>
        /// <param name="preco_custo"></param>
        /// <param name="valor_liquido"></param>
        /// <param name="desconto"></param>
        /// <param name="cst_icms"></param>
        /// <param name="cst_pis"></param>
        /// <param name="cst_cofins"></param>
        /// <param name="cst_ipi"></param>
        /// <param name="valor_icms"></param>
        /// <param name="aliquota_icms"></param>
        /// <param name="base_icms"></param>
        /// <param name="valor_pis"></param>
        /// <param name="aliquota_pis"></param>
        /// <param name="base_pis"></param>
        /// <param name="valor_cofins"></param>
        /// <param name="aliquota_cofins"></param>
        /// <param name="base_cofins"></param>
        /// <param name="valor_icms_st"></param>
        /// <param name="aliquota_icms_st"></param>
        /// <param name="base_icms_st"></param>
        /// <param name="valor_ipi"></param>
        /// <param name="aliquota_ipi"></param>
        /// <param name="base_ipi"></param>
        /// <param name="valor_total"></param>
        /// <param name="forma_dinheiro"></param>
        /// <param name="total_dinheiro"></param>
        /// <param name="forma_cheque"></param>
        /// <param name="total_cheque"></param>
        /// <param name="forma_cartao"></param>
        /// <param name="total_cartao"></param>
        /// <param name="total_crediario"></param>
        /// <param name="forma_crediario"></param>
        /// <param name="forma_convenio"></param>
        /// <param name="total_convenio"></param>
        /// <param name="frete"></param>
        /// <param name="operacao"></param>
        /// <param name="tipo_transacao"></param>
        /// <param name="cod_produto"></param>
        /// <param name="cod_barra"></param>
        /// <param name="cancelado"></param>
        /// <param name="excluido"></param>
        /// <param name="soma_relatorio"></param>
        /// <param name="identificador"></param>
        /// <param name="deposito"></param>
        /// <param name="obs"></param>
        /// <param name="preco_unitario"></param>
        /// <param name="hora_lancamento"></param>
        /// <param name="natureza_operacao"></param>
        /// <param name="tabela_preco"></param>
        /// <param name="nome_tabela_preco"></param>
        /// <param name="cod_sefaz_situacao"></param>
        /// <param name="desc_sefaz_situacao"></param>
        /// <param name="protocolo_aut_nfe"></param>
        /// <param name="dt_update"></param>
        /// <param name="forma_cheque_prazo"></param>
        /// <param name="total_cheque_prazo"></param>
        /// <param name="cod_natureza_operacao"></param>
        /// <param name="preco_tabela_epoca"></param>
        /// <param name="desconto_total_item"></param>
        /// <param name="conferido"></param>
        /// <param name="transacao_pedido_venda"></param>
        /// <param name="codigo_modelo_nf"></param>
        /// <param name="acrescimo"></param>
        /// <param name="mob_checkout"></param>
        /// <param name="aliquota_iss"></param>
        /// <param name="base_iss"></param>
        /// <param name="ordem"></param>
        /// <param name="codigo_rotina_origem"></param>
        /// <param name="timestamp"></param>
        /// <param name="troco"></param>
        /// <param name="transportador"></param>
        /// <param name="icms_aliquota_desonerado"></param>
        /// <param name="icms_valor_desonerado_item"></param>
        /// <param name="empresa"></param>
        /// <param name="desconto_item"></param>
        /// <param name="aliq_iss"></param>
        /// <param name="iss_base_item"></param>
        /// <param name="despesas"></param>
        /// <param name="seguro_total_item"></param>
        /// <param name="acrescimo_total_item"></param>
        /// <param name="despesas_total_item"></param>
        /// <param name="forma_pix"></param>
        /// <param name="total_pix"></param>
        /// <param name="forma_deposito_bancario"></param>
        /// <param name="total_deposito_bancario"></param>
        /// <param name="id_venda_produto_b2c"></param>
        /// <param name="item_promocional"></param>
        /// <param name="acrescimo_item"></param>
        /// <param name="icms_st_antecipado_aliquota"></param>
        /// <param name="icms_st_antecipado_margem"></param>
        /// <param name="icms_st_antecipado_percentual_reducao"></param>
        /// <param name="icms_st_antecipado_valor_item"></param>
        /// <param name="icms_base_desonerado_item"></param>
        /// <param name="codigo_status_nfe"></param>
        public LinxMovimento(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimento record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.transacao = ConverterValidatorsExtensions.ConvertToInt32Validation(record.transacao);
            this.usuario = ConverterValidatorsExtensions.ConvertToInt32Validation(record.usuario);
            this.documento = ConverterValidatorsExtensions.ConvertToInt32Validation(record.documento);
            this.ecf = ConverterValidatorsExtensions.ConvertToInt32Validation(record.ecf);
            this.modelo_nf = ConverterValidatorsExtensions.ConvertToInt32Validation(record.modelo_nf);
            this.codigo_cliente = ConverterValidatorsExtensions.ConvertToInt32Validation(record.codigo_cliente);
            this.cod_vendedor = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_vendedor);
            this.tabela_preco = ConverterValidatorsExtensions.ConvertToInt32Validation(record.tabela_preco);
            this.cod_sefaz_situacao = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_sefaz_situacao);
            this.transacao_pedido_venda = ConverterValidatorsExtensions.ConvertToInt32Validation(record.transacao_pedido_venda);
            this.ordem = ConverterValidatorsExtensions.ConvertToInt32Validation(record.ordem);
            this.codigo_rotina_origem = ConverterValidatorsExtensions.ConvertToInt32Validation(record.codigo_rotina_origem);
            this.transportador = ConverterValidatorsExtensions.ConvertToInt32Validation(record.transportador);
            this.empresa = ConverterValidatorsExtensions.ConvertToInt32Validation(record.empresa);
            this.id_venda_produto_b2c = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_venda_produto_b2c);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.cod_produto = ConverterValidatorsExtensions.ConvertToInt64Validation(record.cod_produto);
            this.data_documento = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.data_documento);
            this.data_lancamento = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.data_lancamento);
            this.dt_update = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.dt_update);
            this.quantidade = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.quantidade);
            this.preco_custo = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.preco_custo);
            this.valor_liquido = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.valor_liquido);
            this.desconto = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.desconto);
            this.valor_icms = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.valor_icms);
            this.aliquota_icms = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.aliquota_icms);
            this.base_icms = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.base_icms);
            this.valor_pis = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.valor_pis);
            this.aliquota_pis = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.aliquota_pis);
            this.base_pis = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.base_pis);
            this.valor_cofins = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.valor_cofins);
            this.aliquota_cofins = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.aliquota_cofins);
            this.base_cofins = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.base_cofins);
            this.valor_icms_st = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.valor_icms_st);
            this.base_icms_st = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.base_icms_st);
            this.aliquota_icms_st = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.aliquota_icms_st);
            this.valor_ipi = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.valor_ipi);
            this.aliquota_ipi = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.aliquota_ipi);
            this.base_ipi = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.base_ipi);
            this.valor_total = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.valor_total);
            this.total_dinheiro = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.total_dinheiro);
            this.total_cheque = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.total_cheque);
            this.total_cartao = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.total_cartao);
            this.total_crediario = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.total_crediario);
            this.total_convenio = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.total_convenio);
            this.frete = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.frete);
            this.preco_unitario = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.preco_unitario);
            this.total_cheque_prazo = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.total_cheque_prazo);
            this.preco_tabela_epoca = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.preco_tabela_epoca);
            this.desconto_total_item = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.desconto_total_item);
            this.acrescimo = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.acrescimo);
            this.aliquota_iss = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.aliquota_iss);
            this.base_iss = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.base_iss);
            this.troco = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.troco);
            this.icms_aliquota_desonerado = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.icms_aliquota_desonerado);
            this.icms_valor_desonerado_item = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.icms_valor_desonerado_item);
            this.desconto_item = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.desconto_item);
            this.aliq_iss = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.aliq_iss);
            this.iss_base_item = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.iss_base_item);
            this.despesas = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.despesas);
            this.seguro_total_item = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.seguro_total_item);
            this.acrescimo_total_item = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.acrescimo_total_item);
            this.despesas_total_item = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.despesas_total_item);
            this.total_pix = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.total_pix);
            this.total_deposito_bancario = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.total_deposito_bancario);
            this.icms_st_antecipado_aliquota = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.icms_st_antecipado_aliquota);
            this.icms_st_antecipado_margem = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.icms_st_antecipado_margem);
            this.icms_st_antecipado_percentual_reducao = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.icms_st_antecipado_percentual_reducao);
            this.icms_st_antecipado_valor_item = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.icms_st_antecipado_valor_item);
            this.icms_base_desonerado_item = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.icms_base_desonerado_item);
            this.forma_dinheiro = ConverterValidatorsExtensions.ConvertToInt32Validation(record.forma_dinheiro);
            this.forma_cheque = ConverterValidatorsExtensions.ConvertToInt32Validation(record.forma_cheque);
            this.forma_cartao = ConverterValidatorsExtensions.ConvertToInt32Validation(record.forma_cartao);
            this.forma_crediario = ConverterValidatorsExtensions.ConvertToInt32Validation(record.forma_crediario);
            this.forma_convenio = ConverterValidatorsExtensions.ConvertToInt32Validation(record.forma_convenio);
            this.forma_cheque_prazo = ConverterValidatorsExtensions.ConvertToInt32Validation(record.forma_cheque_prazo);
            this.mob_checkout = ConverterValidatorsExtensions.ConvertToBooleanValidation(record.mob_checkout);
            this.forma_pix = ConverterValidatorsExtensions.ConvertToInt32Validation(record.forma_pix);
            this.forma_deposito_bancario = ConverterValidatorsExtensions.ConvertToInt32Validation(record.forma_deposito_bancario);
            this.item_promocional = ConverterValidatorsExtensions.ConvertToBooleanValidation(record.item_promocional);
            this.acrescimo_item = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.acrescimo_item);
            this.identificador = Guid.Parse(record.identificador);
            this.cnpj_emp = record.cnpj_emp;
            this.chave_nf = record.chave_nf;
            this.numero_serie_ecf = record.numero_serie_ecf;
            this.serie = record.serie;
            this.desc_cfop = record.desc_cfop;
            this.id_cfop = record.id_cfop;
            this.cst_icms = record.cst_icms;
            this.cst_pis = record.cst_pis;
            this.cst_cofins = record.cst_cofins;
            this.cst_ipi = record.cst_ipi;
            this.operacao = record.operacao;
            this.cod_barra = record.cod_barra;
            this.tipo_transacao = record.tipo_transacao;
            this.cancelado = record.cancelado;
            this.excluido = record.excluido;
            this.soma_relatorio = record.soma_relatorio;
            this.deposito = record.deposito;
            this.obs = record.obs;
            this.hora_lancamento = record.hora_lancamento;
            this.natureza_operacao = record.natureza_operacao;
            this.nome_tabela_preco = record.nome_tabela_preco;
            this.desc_sefaz_situacao = record.desc_sefaz_situacao;
            this.protocolo_aut_nfe = record.protocolo_aut_nfe;
            this.cod_natureza_operacao = record.cod_natureza_operacao;
            this.conferido = record.conferido;
            this.codigo_modelo_nf = record.codigo_modelo_nf;
            this.codigo_status_nfe = record.codigo_status_nfe;
            this.recordKey = $"[{record.cnpj_emp}]|[{record.documento}]|[{record.chave_nf}]|[{record.data_documento}]|[{record.codigo_cliente}]|[{record.cod_produto}]|[{record.cancelado}]|[{record.excluido}]|[{record.transacao_pedido_venda}]|[{record.ordem}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
