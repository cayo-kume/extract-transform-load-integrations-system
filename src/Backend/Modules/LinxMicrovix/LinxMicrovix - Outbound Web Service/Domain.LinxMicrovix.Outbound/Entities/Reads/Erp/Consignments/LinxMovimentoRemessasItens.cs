using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Consignments
{
    /// <summary>
    /// LinxMovimentoRemessasItens, tabela linxmicrovix.LinxMovimentoRemessasItens. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_remessas_itens
    /// </summary>
    public class LinxMovimentoRemessasItens
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? empresa { get; private set; }
        public int id_remessas_itens { get; private set; }
        public int? id_remessas { get; private set; }
        public int? transacao { get; private set; }
        public decimal? qtde_total { get; private set; }
        public decimal? qtde_venda { get; private set; }
        public decimal? qtde_devolvido { get; private set; }
        public decimal? qtde_pendente { get; private set; }
        public decimal? qtde_pendente_pagamento { get; private set; }
        public long timestamp { get; private set; }

        public LinxMovimentoRemessasItens() { }

        public LinxMovimentoRemessasItens(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Consignments.LinxMovimentoRemessasItens dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            id_remessas_itens = MicrovixConverters.ToInt32(dto.id_remessas_itens);
            id_remessas = MicrovixConverters.ToNullableInt32(dto.id_remessas);
            transacao = MicrovixConverters.ToNullableInt32(dto.transacao);
            qtde_total = MicrovixConverters.ToNullableDecimal(dto.qtde_total);
            qtde_venda = MicrovixConverters.ToNullableDecimal(dto.qtde_venda);
            qtde_devolvido = MicrovixConverters.ToNullableDecimal(dto.qtde_devolvido);
            qtde_pendente = MicrovixConverters.ToNullableDecimal(dto.qtde_pendente);
            qtde_pendente_pagamento = MicrovixConverters.ToNullableDecimal(dto.qtde_pendente_pagamento);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
