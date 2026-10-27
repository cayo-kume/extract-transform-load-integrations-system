using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// B2CConsultaLegendasCadastrosAuxiliares, tabela linxmicrovix.B2CConsultaLegendasCadastrosAuxiliares. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + empresa (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaLegendasCadastrosAuxiliares
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int empresa { get; private set; }
        public string? legenda_setor { get; private set; }
        public string? legenda_linha { get; private set; }
        public string? legenda_marca { get; private set; }
        public string? legenda_colecao { get; private set; }
        public string? legenda_grade1 { get; private set; }
        public string? legenda_grade2 { get; private set; }
        public string? legenda_espessura { get; private set; }
        public string? legenda_classificacao { get; private set; }
        public long timestamp { get; private set; }

        public B2CConsultaLegendasCadastrosAuxiliares() { }

        public B2CConsultaLegendasCadastrosAuxiliares(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductAttributes.B2CConsultaLegendasCadastrosAuxiliares dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            empresa = MicrovixConverters.ToInt32(dto.empresa);
            legenda_setor = MicrovixConverters.ToNullableString(dto.legenda_setor, 20);
            legenda_linha = MicrovixConverters.ToNullableString(dto.legenda_linha, 20);
            legenda_marca = MicrovixConverters.ToNullableString(dto.legenda_marca, 20);
            legenda_colecao = MicrovixConverters.ToNullableString(dto.legenda_colecao, 20);
            legenda_grade1 = MicrovixConverters.ToNullableString(dto.legenda_grade1, 20);
            legenda_grade2 = MicrovixConverters.ToNullableString(dto.legenda_grade2, 20);
            legenda_espessura = MicrovixConverters.ToNullableString(dto.legenda_espessura, 20);
            legenda_classificacao = MicrovixConverters.ToNullableString(dto.legenda_classificacao, 20);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
