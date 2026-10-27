using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers
{
    /// <summary>
    /// LinxRamosAtividade, tabela linxmicrovix.LinxRamosAtividade. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_ramo_atividade (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxRamosAtividade
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_ramo_atividade { get; private set; }
        public string? nome_ramo_atividade { get; private set; }
        public string? codigo_ramo_atividade { get; private set; }
        public long timestamp { get; private set; }

        public LinxRamosAtividade() { }

        public LinxRamosAtividade(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Customers.LinxRamosAtividade dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_ramo_atividade = MicrovixConverters.ToInt32(dto.id_ramo_atividade);
            nome_ramo_atividade = MicrovixConverters.ToNullableString(dto.nome_ramo_atividade, 50);
            codigo_ramo_atividade = MicrovixConverters.ToNullableString(dto.codigo_ramo_atividade, 12);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
