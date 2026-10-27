using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Prices
{
    /// <summary>
    /// LinxCupomDescontoVendas, tabela linxmicrovix.LinxCupomDescontoVendas. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + identificador (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxCupomDescontoVendas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? empresa { get; private set; }
        public int? id_cupom_desconto_vendas { get; private set; }
        public int? id_cupom_desconto { get; private set; }
        public Guid identificador { get; private set; }
        public decimal? valor { get; private set; }
        public long timestamp { get; private set; }
        public int? id_vendas_pos { get; private set; }

        public LinxCupomDescontoVendas() { }

        public LinxCupomDescontoVendas(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Prices.LinxCupomDescontoVendas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            id_cupom_desconto_vendas = MicrovixConverters.ToNullableInt32(dto.id_cupom_desconto_vendas);
            id_cupom_desconto = MicrovixConverters.ToNullableInt32(dto.id_cupom_desconto);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador) ?? Guid.Empty;
            valor = MicrovixConverters.ToNullableDecimal(dto.valor);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            id_vendas_pos = MicrovixConverters.ToNullableInt32(dto.id_vendas_pos);
        }
    }
}
