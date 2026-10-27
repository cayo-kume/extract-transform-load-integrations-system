using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Sellers
{
    /// <summary>
    /// LinxFuncionariosFuncoes, tabela linxmicrovix.LinxFuncionariosFuncoes. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_funcionarios_funcao (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxFuncionariosFuncoes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_funcionarios_funcao { get; private set; }
        public string? descricao { get; private set; }
        public bool? ativo { get; private set; }
        public long timestamp { get; private set; }

        public LinxFuncionariosFuncoes() { }

        public LinxFuncionariosFuncoes(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Sellers.LinxFuncionariosFuncoes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_funcionarios_funcao = MicrovixConverters.ToInt32(dto.id_funcionarios_funcao);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 100);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
