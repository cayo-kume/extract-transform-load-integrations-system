using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements
{
    /// <summary>
    /// LinxRespostaVenda, tabela linxmicrovix.LinxRespostaVenda. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_resposta_venda (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxRespostaVenda
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_resposta_venda { get; private set; }
        public string? descricao_resposta { get; private set; }
        public int? id_pergunta_venda { get; private set; }
        public long timestamp { get; private set; }

        public LinxRespostaVenda() { }

        public LinxRespostaVenda(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Movements.LinxRespostaVenda dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_resposta_venda = MicrovixConverters.ToInt32(dto.id_resposta_venda);
            descricao_resposta = MicrovixConverters.ToNullableString(dto.descricao_resposta, 100);
            id_pergunta_venda = MicrovixConverters.ToNullableInt32(dto.id_pergunta_venda);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
