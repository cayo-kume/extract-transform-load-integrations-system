
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce
{
    public class B2CConsultaPedidos
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? id_pedido { get; private set; }
        public DateTime? dt_pedido { get; private set; }
        public Int32? cod_cliente_erp { get; private set; }
        public Int32? cod_cliente_b2c { get; private set; }
        public decimal? vl_frete { get; private set; }
        public Int32? forma_pgto { get; private set; }
        public Int32? plano_pagamento { get; private set; }
        public string? anotacao { get; private set; }
        public decimal? taxa_impressao { get; private set; }
        public Int32? finalizado { get; private set; }
        public decimal? valor_frete_gratis { get; private set; }
        public Int32? tipo_frete { get; private set; }
        public Int32? id_status { get; private set; }
        public Int32? cod_transportador { get; private set; }
        public Int32? tipo_cobranca_frete { get; private set; }
        public Int32? ativo { get; private set; }
        public Int32? empresa { get; private set; }
        public Int32? id_tabela_preco { get; private set; }
        public decimal? valor_credito { get; private set; }
        public Int32? cod_vendedor { get; private set; }
        public Int64? timestamp { get; private set; }
        public DateTime? dt_insert { get; private set; }
        public DateTime? dt_disponivel_faturamento { get; private set; }
        public Int32? portal { get; private set; }
        public string? mensagem_falha_faturamento { get; private set; }
        public Int32? id_tipo_b2c { get; private set; }
        public string? ecommerce_origem { get; private set; }
        public string? order_id { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public B2CConsultaPedidos() { }

        public B2CConsultaPedidos(
            Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidos record,
            string? recordXml
        )
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.id_pedido = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_pedido);
            this.dt_pedido = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.dt_pedido);
            this.cod_cliente_erp = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_cliente_erp);
            this.cod_cliente_b2c = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_cliente_b2c);
            this.vl_frete = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.vl_frete);
            this.forma_pgto = ConverterValidatorsExtensions.ConvertToInt32Validation(record.forma_pgto);
            this.plano_pagamento = ConverterValidatorsExtensions.ConvertToInt32Validation(record.plano_pagamento);
            this.taxa_impressao = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.taxa_impressao);
            this.finalizado = ConverterValidatorsExtensions.ConvertToInt32Validation(record.finalizado);
            this.valor_frete_gratis = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.valor_frete_gratis);
            this.tipo_frete = ConverterValidatorsExtensions.ConvertToInt32Validation(record.tipo_frete);
            this.id_status = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_status);
            this.cod_transportador = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_transportador);
            this.tipo_cobranca_frete = ConverterValidatorsExtensions.ConvertToInt32Validation(record.tipo_cobranca_frete);
            this.ativo = ConverterValidatorsExtensions.ConvertToInt32Validation(record.ativo);
            this.empresa = ConverterValidatorsExtensions.ConvertToInt32Validation(record.empresa);
            this.id_tabela_preco = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_tabela_preco);
            this.valor_credito = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.valor_credito);
            this.cod_vendedor = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_vendedor);
            this.dt_insert = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.dt_insert);
            this.dt_disponivel_faturamento = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.dt_disponivel_faturamento);
            this.id_tipo_b2c = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_tipo_b2c);
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);

            this.anotacao = record.anotacao;
            this.ecommerce_origem = record.ecommerce_origem;
            this.order_id = record.order_id;
            this.mensagem_falha_faturamento = record.mensagem_falha_faturamento;
            this.recordKey = $"[{record.empresa}]|[{record.id_pedido}]|[{record.cod_cliente_b2c}]|[{record.cod_cliente_erp}]|[{record.order_id}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
