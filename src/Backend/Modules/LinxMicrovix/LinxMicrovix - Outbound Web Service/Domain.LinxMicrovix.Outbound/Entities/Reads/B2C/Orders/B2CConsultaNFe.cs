using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders
{
    /// <summary>
    /// B2CConsultaNFe, tabela linxmicrovix.B2CConsultaNFe. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + id_nfe (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaNFe
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_nfe { get; private set; }
        public int? id_pedido { get; private set; }
        public int? documento { get; private set; }
        public DateTime? data_emissao { get; private set; }
        public string? chave_nfe { get; private set; }
        public int? situacao { get; private set; }
        public string? xml { get; private set; }
        public bool? excluido { get; private set; }
        public string? identificador_microvix { get; private set; }
        public DateTime? dt_insert { get; private set; }
        public decimal? valor_nota { get; private set; }
        public string? serie { get; private set; }
        public decimal? frete { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }
        public string? nProt { get; private set; }
        public string? codigo_modelo_nf { get; private set; }
        public string? justificativa { get; private set; }
        public int? tpAmb { get; private set; }

        public B2CConsultaNFe() { }

        public B2CConsultaNFe(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Orders.B2CConsultaNFe dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_nfe = MicrovixConverters.ToInt32(dto.id_nfe);
            id_pedido = MicrovixConverters.ToNullableInt32(dto.id_pedido);
            documento = MicrovixConverters.ToNullableInt32(dto.documento);
            data_emissao = MicrovixConverters.ToNullableDateTime(dto.data_emissao);
            chave_nfe = MicrovixConverters.ToNullableString(dto.chave_nfe, 44);
            situacao = MicrovixConverters.ToNullableInt32(dto.situacao);
            xml = String.IsNullOrEmpty(dto.xml) ? null : dto.xml;
            excluido = MicrovixConverters.ToNullableBoolean(dto.excluido);
            identificador_microvix = MicrovixConverters.ToNullableString(dto.identificador_microvix, 1000);   //fora da documentação
            dt_insert = MicrovixConverters.ToNullableDateTime(dto.dt_insert);
            valor_nota = MicrovixConverters.ToNullableDecimal(dto.valor_nota);
            serie = MicrovixConverters.ToNullableString(dto.serie, 10);
            frete = MicrovixConverters.ToNullableDecimal(dto.frete);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            nProt = MicrovixConverters.ToNullableString(dto.nProt, 15);
            codigo_modelo_nf = MicrovixConverters.ToNullableString(dto.codigo_modelo_nf, 3);
            justificativa = MicrovixConverters.ToNullableString(dto.justificativa, 255);
            tpAmb = MicrovixConverters.ToNullableInt32(dto.tpAmb);
        }
    }
}
