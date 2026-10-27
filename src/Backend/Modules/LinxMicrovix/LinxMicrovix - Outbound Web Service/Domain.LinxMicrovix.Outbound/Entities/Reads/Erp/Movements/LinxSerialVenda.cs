using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements
{
    /// <summary>
    /// LinxSerialVenda, tabela linxmicrovix.LinxSerialVenda. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_serial_venda (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxSerialVenda
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_serial_venda { get; private set; }
        public int? transacao { get; private set; }
        public long? codigoproduto { get; private set; }
        public string? serial { get; private set; }
        public long timestamp { get; private set; }

        public LinxSerialVenda() { }

        public LinxSerialVenda(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Movements.LinxSerialVenda dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_serial_venda = MicrovixConverters.ToInt32(dto.id_serial_venda);
            transacao = MicrovixConverters.ToNullableInt32(dto.transacao);
            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            serial = MicrovixConverters.ToNullableString(dto.serial, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
