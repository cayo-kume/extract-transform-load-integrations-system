using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Consignments
{
    /// <summary>
    /// LinxRemessasIdentificadores, tabela linxmicrovix.LinxRemessasIdentificadores. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + identificador_venda + identificador_remessa + transacao_acerto (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxRemessasIdentificadores
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public Guid identificador_venda { get; private set; }
        public Guid identificador_remessa { get; private set; }
        public int? id_remessas { get; private set; }
        public int? id_remessas_acertos { get; private set; }
        public int transacao_acerto { get; private set; }
        public decimal? qtde_total_acerto { get; private set; }
        public int? transacao_remessa { get; private set; }
        public Guid? identificador_devolucao { get; private set; }
        public int? id_remessa_operacoes { get; private set; }
        public int? portal { get; private set; }

        public LinxRemessasIdentificadores() { }

        public LinxRemessasIdentificadores(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Consignments.LinxRemessasIdentificadores dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            identificador_venda = MicrovixConverters.ToNullableGuid(dto.identificador_venda) ?? Guid.Empty;
            identificador_remessa = MicrovixConverters.ToNullableGuid(dto.identificador_remessa) ?? Guid.Empty;
            id_remessas = MicrovixConverters.ToNullableInt32(dto.id_remessas);
            id_remessas_acertos = MicrovixConverters.ToNullableInt32(dto.id_remessas_acertos);
            transacao_acerto = MicrovixConverters.ToInt32(dto.transacao_acerto);
            qtde_total_acerto = MicrovixConverters.ToNullableDecimal(dto.qtde_total_acerto);
            transacao_remessa = MicrovixConverters.ToNullableInt32(dto.transacao_remessa);
            identificador_devolucao = MicrovixConverters.ToNullableGuid(dto.identificador_devolucao);
            id_remessa_operacoes = MicrovixConverters.ToNullableInt32(dto.id_remessa_operacoes);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
