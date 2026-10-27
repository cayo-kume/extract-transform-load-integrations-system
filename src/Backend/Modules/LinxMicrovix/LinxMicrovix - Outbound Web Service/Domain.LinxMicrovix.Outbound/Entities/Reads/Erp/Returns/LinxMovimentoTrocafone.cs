using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Returns
{
    /// <summary>
    /// LinxMovimentoTrocafone, tabela linxmicrovix.LinxMovimentoTrocafone. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + identificador + num_vale (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxMovimentoTrocafone
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public Guid identificador { get; private set; }
        public long num_vale { get; private set; }
        public decimal? valor_vale { get; private set; }
        public string? voucher { get; private set; }
        public string? nome_produto { get; private set; }
        public string? condicao { get; private set; }
        public int? id_tradein_parceiro { get; private set; }
        public long timestamp { get; private set; }

        public LinxMovimentoTrocafone() { }

        public LinxMovimentoTrocafone(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Returns.LinxMovimentoTrocafone dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador) ?? Guid.Empty;
            num_vale = MicrovixConverters.ToInt64(dto.num_vale);
            valor_vale = MicrovixConverters.ToNullableDecimal(dto.valor_vale);
            voucher = MicrovixConverters.ToNullableString(dto.voucher, 100);
            nome_produto = MicrovixConverters.ToNullableString(dto.nome_produto, 250);
            condicao = MicrovixConverters.ToNullableString(dto.condicao, 250);
            id_tradein_parceiro = MicrovixConverters.ToNullableInt32(dto.id_tradein_parceiro);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
