using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Sellers
{
    /// <summary>
    /// LinxListaDaVez, tabela linxmicrovix.LinxListaDaVez. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + cod_vendedor + data + timestamp
    /// </summary>
    public class LinxListaDaVez
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int cod_vendedor { get; private set; }
        public DateTime data { get; private set; }
        public string? motivo_nao_venda { get; private set; }
        public int? qtde_ocorrencias { get; private set; }
        public DateTime? data_hora_ini_atend { get; private set; }
        public DateTime? data_hora_fim_atend { get; private set; }
        public string? obs { get; private set; }
        public string? desc_produto_neg { get; private set; }
        public decimal? valor_produto_neg { get; private set; }
        public long timestamp { get; private set; }

        public LinxListaDaVez() { }

        public LinxListaDaVez(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Sellers.LinxListaDaVez dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            cod_vendedor = MicrovixConverters.ToInt32(dto.cod_vendedor);
            data = MicrovixConverters.ToNullableDateTime(dto.data) ?? new DateTime(1900, 1, 1);
            motivo_nao_venda = MicrovixConverters.ToNullableString(dto.motivo_nao_venda, 103);
            qtde_ocorrencias = MicrovixConverters.ToNullableInt32(dto.qtde_ocorrencias);
            data_hora_ini_atend = MicrovixConverters.ToNullableDateTime(dto.data_hora_ini_atend);
            data_hora_fim_atend = MicrovixConverters.ToNullableDateTime(dto.data_hora_fim_atend);
            obs = String.IsNullOrEmpty(dto.obs) ? null : dto.obs;
            desc_produto_neg = MicrovixConverters.ToNullableString(dto.desc_produto_neg, 50);
            valor_produto_neg = MicrovixConverters.ToNullableDecimal(dto.valor_produto_neg);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
