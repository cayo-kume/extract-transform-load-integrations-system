using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Consignments
{
    /// <summary>
    /// LinxMovimentoRemessasAcertosItens, tabela linxmicrovix.LinxMovimentoRemessasAcertosItens. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + transacao
    /// </summary>
    public class LinxMovimentoRemessasAcertosItens
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? empresa { get; private set; }
        public int? id_remessas_acertos { get; private set; }
        public int transacao { get; private set; }
        public decimal? qtde_total { get; private set; }
        public int? id_remessa_operacoes { get; private set; }
        public int? id_remessas_itens { get; private set; }
        public long timestamp { get; private set; }

        public LinxMovimentoRemessasAcertosItens() { }

        public LinxMovimentoRemessasAcertosItens(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Consignments.LinxMovimentoRemessasAcertosItens dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            id_remessas_acertos = MicrovixConverters.ToNullableInt32(dto.id_remessas_acertos);
            transacao = MicrovixConverters.ToInt32(dto.transacao);
            qtde_total = MicrovixConverters.ToNullableDecimal(dto.qtde_total);
            id_remessa_operacoes = MicrovixConverters.ToNullableInt32(dto.id_remessa_operacoes);
            id_remessas_itens = MicrovixConverters.ToNullableInt32(dto.id_remessas_itens);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
