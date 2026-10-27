using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Prices
{
    /// <summary>
    /// LinxCupomDesconto, tabela linxmicrovix.LinxCupomDesconto. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + cupom
    /// </summary>
    public class LinxCupomDesconto
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? empresa { get; private set; }
        public int? id_cupom_desconto { get; private set; }
        public string cupom { get; private set; } = String.Empty;
        public string? descricao { get; private set; }
        public decimal? percentual_desconto { get; private set; }
        public DateTime? data_inicial_vigencia { get; private set; }
        public DateTime? data_final_vigencia { get; private set; }
        public int? qtde_original { get; private set; }
        public int? qtde_disponivel { get; private set; }
        public int? cod_vendedor { get; private set; }
        public bool? disponivel { get; private set; }
        public long timestamp { get; private set; }

        public LinxCupomDesconto() { }

        public LinxCupomDesconto(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Prices.LinxCupomDesconto dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            id_cupom_desconto = MicrovixConverters.ToNullableInt32(dto.id_cupom_desconto);
            cupom = MicrovixConverters.ToNullableString(dto.cupom, 50) ?? String.Empty;
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 255);
            percentual_desconto = MicrovixConverters.ToNullableDecimal(dto.percentual_desconto);
            data_inicial_vigencia = MicrovixConverters.ToNullableDateTime(dto.data_inicial_vigencia);
            data_final_vigencia = MicrovixConverters.ToNullableDateTime(dto.data_final_vigencia);
            qtde_original = MicrovixConverters.ToNullableInt32(dto.qtde_original);
            qtde_disponivel = MicrovixConverters.ToNullableInt32(dto.qtde_disponivel);
            cod_vendedor = MicrovixConverters.ToNullableInt32(dto.cod_vendedor);
            disponivel = MicrovixConverters.ToNullableBoolean(dto.disponivel);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
