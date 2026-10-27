using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// LinxOrdensServicoProdutos, tabela linxmicrovix.LinxOrdensServicoProdutos. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_ordservprod (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxOrdensServicoProdutos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? numero_os { get; private set; }
        public long? cod_produto_serv { get; private set; }
        public int id_ordservprod { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxOrdensServicoProdutos() { }

        public LinxOrdensServicoProdutos(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ServiceOrders.LinxOrdensServicoProdutos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            numero_os = MicrovixConverters.ToNullableInt32(dto.numero_os);
            cod_produto_serv = MicrovixConverters.ToNullableInt64(dto.cod_produto_serv);
            id_ordservprod = MicrovixConverters.ToInt32(dto.id_ordservprod);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
