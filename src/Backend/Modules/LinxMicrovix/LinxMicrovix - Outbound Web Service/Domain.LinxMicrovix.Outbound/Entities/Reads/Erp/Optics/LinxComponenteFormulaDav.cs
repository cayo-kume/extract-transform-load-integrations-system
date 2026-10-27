using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Optics
{
    /// <summary>
    /// LinxComponenteFormulaDav, tabela linxmicrovix.LinxComponenteFormulaDav. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_componente_formula_dav (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxComponenteFormulaDav
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_componente_formula_dav { get; private set; }
        public int? codigo_sistema_terceiro { get; private set; }
        public long? codigo_produto { get; private set; }
        public string? codigo_componente { get; private set; }
        public string? descricao_componente { get; private set; }
        public string? unidade { get; private set; }
        public decimal? quantidade { get; private set; }
        public decimal? valor_componente { get; private set; }
        public string? lote_componente { get; private set; }
        public DateTime? data_validade_lote { get; private set; }
        public string? codigo_ws { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxComponenteFormulaDav() { }

        public LinxComponenteFormulaDav(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Optics.LinxComponenteFormulaDav dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_componente_formula_dav = MicrovixConverters.ToInt32(dto.id_componente_formula_dav);
            codigo_sistema_terceiro = MicrovixConverters.ToNullableInt32(dto.codigo_sistema_terceiro);
            codigo_produto = MicrovixConverters.ToNullableInt64(dto.codigo_produto);
            codigo_componente = MicrovixConverters.ToNullableString(dto.codigo_componente, 10);
            descricao_componente = MicrovixConverters.ToNullableString(dto.descricao_componente, 50);
            unidade = MicrovixConverters.ToNullableString(dto.unidade, 5);
            quantidade = MicrovixConverters.ToNullableDecimal(dto.quantidade);
            valor_componente = MicrovixConverters.ToNullableDecimal(dto.valor_componente);
            lote_componente = MicrovixConverters.ToNullableString(dto.lote_componente, 30);
            data_validade_lote = MicrovixConverters.ToNullableDateTime(dto.data_validade_lote);
            codigo_ws = MicrovixConverters.ToNullableString(dto.codigo_ws, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
