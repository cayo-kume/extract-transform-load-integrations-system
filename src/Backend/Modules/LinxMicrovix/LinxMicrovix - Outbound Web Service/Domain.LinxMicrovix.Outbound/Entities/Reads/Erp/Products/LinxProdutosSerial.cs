using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Products
{
    /// <summary>
    /// LinxProdutosSerial, tabela linxmicrovix.LinxProdutosSerial. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + codigoproduto + empresa + serial + id_deposito (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxProdutosSerial
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int empresa { get; private set; }
        public long codigoproduto { get; private set; }
        public string serial { get; private set; } = String.Empty;
        public int id_deposito { get; private set; }
        public bool? saldo { get; private set; }
        public long timestamp { get; private set; }

        public LinxProdutosSerial() { }

        public LinxProdutosSerial(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Products.LinxProdutosSerial dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToInt32(dto.empresa);
            codigoproduto = MicrovixConverters.ToInt64(dto.codigoproduto);
            serial = MicrovixConverters.ToNullableString(dto.serial, 50) ?? String.Empty;
            id_deposito = MicrovixConverters.ToInt32(dto.id_deposito);
            saldo = MicrovixConverters.ToNullableBoolean(dto.saldo);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
