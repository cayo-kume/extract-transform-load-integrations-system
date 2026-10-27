using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Financial
{
    /// <summary>
    /// LinxPlanoContas, tabela linxmicrovix.LinxPlanoContas. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + empresa + id_conta
    /// </summary>
    public class LinxPlanoContas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int empresa { get; private set; }
        public string? CNPJ { get; private set; }
        public int id_conta { get; private set; }
        public string? nome_conta { get; private set; }
        public string? sintetica { get; private set; }
        public string? indice { get; private set; }
        public string? ativa { get; private set; }
        public string? fluxo_caixa { get; private set; }
        public string? CONTA_CONTABIL { get; private set; }
        public int? id_natureza_conta { get; private set; }
        public bool? conta_bancaria { get; private set; }
        public long timestamp { get; private set; }

        public LinxPlanoContas() { }

        public LinxPlanoContas(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Financial.LinxPlanoContas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToInt32(dto.empresa);
            CNPJ = MicrovixConverters.ToNullableString(dto.CNPJ, 14);
            id_conta = MicrovixConverters.ToInt32(dto.id_conta);
            nome_conta = MicrovixConverters.ToNullableString(dto.nome_conta, 50);
            sintetica = MicrovixConverters.ToNullableString(dto.sintetica, 1);
            indice = MicrovixConverters.ToNullableString(dto.indice, 150);
            ativa = MicrovixConverters.ToNullableString(dto.ativa, 1);
            fluxo_caixa = MicrovixConverters.ToNullableString(dto.fluxo_caixa, 1);
            CONTA_CONTABIL = MicrovixConverters.ToNullableString(dto.CONTA_CONTABIL, 20);
            id_natureza_conta = MicrovixConverters.ToNullableInt32(dto.id_natureza_conta);
            conta_bancaria = MicrovixConverters.ToNullableBoolean(dto.conta_bancaria);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
