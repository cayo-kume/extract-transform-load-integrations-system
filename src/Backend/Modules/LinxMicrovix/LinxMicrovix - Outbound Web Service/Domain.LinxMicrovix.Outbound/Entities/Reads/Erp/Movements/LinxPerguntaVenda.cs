using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements
{
    /// <summary>
    /// LinxPerguntaVenda, tabela linxmicrovix.LinxPerguntaVenda. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_pergunta_venda (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxPerguntaVenda
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_pergunta_venda { get; private set; }
        public string? descricao_pergunta { get; private set; }
        public long timestamp { get; private set; }

        public LinxPerguntaVenda() { }

        public LinxPerguntaVenda(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Movements.LinxPerguntaVenda dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_pergunta_venda = MicrovixConverters.ToInt32(dto.id_pergunta_venda);
            descricao_pergunta = MicrovixConverters.ToNullableString(dto.descricao_pergunta, 200);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
