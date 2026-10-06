
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxPedidosCompra
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? portal { get; private set; }
        public string? cnpj_emp { get; private set; }
        public Int32? cod_pedido { get; private set; }
        public DateTime? data_pedido { get; private set; }
        public Int32? transacao { get; private set; }
        public Int32? usuario { get; private set; }
        public Int32? codigo_fornecedor { get; private set; }
        public Int64? cod_produto { get; private set; }
        public decimal? quantidade { get; private set; }
        public decimal? valor_unitario { get; private set; }
        public Int32? cod_comprador { get; private set; }
        public decimal? valor_frete { get; private set; }
        public decimal? valor_total { get; private set; }
        public Int32? cod_plano_pagamento { get; private set; }
        public string? plano_pagamento { get; private set; }
        public string? obs { get; private set; }
        public string? aprovado { get; private set; }
        public string? cancelado { get; private set; }
        public string? encerrado { get; private set; }
        public DateTime? data_aprovacao { get; private set; }
        public string? numero_ped_fornec { get; private set; }
        public string? tipo_frete { get; private set; }
        public string? natureza_operacao { get; private set; }
        public DateTime? previsao_entrega { get; private set; }
        public string? numero_projeto_officina { get; private set; }
        public string? status_pedido { get; private set; }
        public decimal? qtde_entregue { get; private set; }
        public string? descricao_frete { get; private set; }
        public Int32? integrado_linx { get; private set; }
        public Int32? nf_gerada { get; private set; }
        public Int64? timestamp { get; private set; }
        public Int32? empresa { get; private set; }
        public string? nf_origem_ws { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxPedidosCompra() { }

        public LinxPedidosCompra(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxPedidosCompra record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.cod_pedido = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_pedido);
            this.transacao = ConverterValidatorsExtensions.ConvertToInt32Validation(record.transacao);
            this.usuario = ConverterValidatorsExtensions.ConvertToInt32Validation(record.usuario);
            this.codigo_fornecedor = ConverterValidatorsExtensions.ConvertToInt32Validation(record.codigo_fornecedor);
            this.cod_comprador = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_comprador);
            this.cod_plano_pagamento = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_plano_pagamento);
            this.nf_gerada = ConverterValidatorsExtensions.ConvertToInt32Validation(record.nf_gerada);
            this.empresa = ConverterValidatorsExtensions.ConvertToInt32Validation(record.empresa);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.cod_produto = ConverterValidatorsExtensions.ConvertToInt64Validation(record.cod_produto);
            this.data_pedido = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.data_pedido);
            this.data_aprovacao = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.data_aprovacao);
            this.previsao_entrega = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.previsao_entrega);
            this.quantidade = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.quantidade);
            this.valor_unitario = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.valor_unitario);
            this.valor_frete = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.valor_frete);
            this.valor_total = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.valor_total);
            this.qtde_entregue = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.qtde_entregue);
            this.integrado_linx = ConverterValidatorsExtensions.ConvertToInt32Validation(record.integrado_linx);
            this.status_pedido = record.status_pedido;
            this.tipo_frete = record.tipo_frete;
            this.nf_origem_ws = record.nf_origem_ws;
            this.descricao_frete = record.descricao_frete;
            this.numero_projeto_officina = record.numero_projeto_officina;
            this.natureza_operacao = record.natureza_operacao;
            this.numero_ped_fornec = record.numero_ped_fornec;
            this.encerrado = record.encerrado;
            this.aprovado = record.aprovado;
            this.cancelado = record.cancelado;
            this.obs = record.obs;
            this.plano_pagamento = record.plano_pagamento;
            this.cnpj_emp = record.cnpj_emp;
            this.recordKey = $"[{record.cnpj_emp}]|[{record.cod_pedido}]|[{record.codigo_fornecedor}]|[{record.cod_produto}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
