using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.PosSales
{
    /// <summary>
    /// LinxVendasPosTmp, tabela linxmicrovix.LinxVendasPosTmp. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_vendas_pos
    /// </summary>
    public class LinxVendasPosTmp
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public long id_vendas_pos { get; private set; }
        public long? numero_ficha { get; private set; }
        public int? numero_sequencial { get; private set; }
        public int? cod_cliente { get; private set; }
        public DateTime? data_lcto { get; private set; }
        public int? empresa { get; private set; }
        public int? cliente_indicador { get; private set; }
        public string? observacao_dav { get; private set; }
        public string? observacao { get; private set; }
        public int? codigo_sistema_terceiro { get; private set; }
        public bool? cancelado { get; private set; }
        public long timestamp { get; private set; }

        public LinxVendasPosTmp() { }

        public LinxVendasPosTmp(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.PosSales.LinxVendasPosTmp dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_vendas_pos = MicrovixConverters.ToInt64(dto.id_vendas_pos);
            numero_ficha = MicrovixConverters.ToNullableInt64(dto.numero_ficha);
            numero_sequencial = MicrovixConverters.ToNullableInt32(dto.numero_sequencial);
            cod_cliente = MicrovixConverters.ToNullableInt32(dto.cod_cliente);
            data_lcto = MicrovixConverters.ToNullableDateTime(dto.data_lcto);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            cliente_indicador = MicrovixConverters.ToNullableInt32(dto.cliente_indicador);
            observacao_dav = MicrovixConverters.ToNullableString(dto.observacao_dav, 4000);
            observacao = MicrovixConverters.ToNullableString(dto.observacao, 500);
            codigo_sistema_terceiro = MicrovixConverters.ToNullableInt32(dto.codigo_sistema_terceiro);
            cancelado = MicrovixConverters.ToNullableBoolean(dto.cancelado);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
