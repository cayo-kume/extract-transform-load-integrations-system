using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// LinxECF, tabela linxmicrovix.LinxECF. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_ecf (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxECF
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_ecf { get; private set; }
        public int? empresa { get; private set; }
        public int? ecf { get; private set; }
        public string? numeroserie { get; private set; }
        public string? indice_sangria { get; private set; }
        public int? modelo { get; private set; }
        public string? nome { get; private set; }
        public string? indice_suprimento { get; private set; }
        public bool? ativo { get; private set; }
        public string? indice_sinal { get; private set; }
        public string? indice_antecipacao { get; private set; }
        public string? indice_cartao_credito { get; private set; }
        public string? empresa_ecf { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxECF() { }

        public LinxECF(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.FiscalDocuments.LinxECF dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_ecf = MicrovixConverters.ToInt32(dto.id_ecf);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            ecf = MicrovixConverters.ToNullableInt32(dto.ecf);
            numeroserie = MicrovixConverters.ToNullableString(dto.numeroserie, 30);
            indice_sangria = MicrovixConverters.ToNullableString(dto.indice_sangria, 4);
            modelo = MicrovixConverters.ToNullableInt32(dto.modelo);
            nome = MicrovixConverters.ToNullableString(dto.nome, 50);   //fora da documentação
            indice_suprimento = MicrovixConverters.ToNullableString(dto.indice_suprimento, 4);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            indice_sinal = MicrovixConverters.ToNullableString(dto.indice_sinal, 53);
            indice_antecipacao = MicrovixConverters.ToNullableString(dto.indice_antecipacao, 50);
            indice_cartao_credito = MicrovixConverters.ToNullableString(dto.indice_cartao_credito, 50);
            empresa_ecf = MicrovixConverters.ToNullableString(dto.empresa_ecf, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
