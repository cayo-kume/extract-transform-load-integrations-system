using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Returns
{
    /// <summary>
    /// LinxDevolucaoRemanejoFabricaItem, tabela linxmicrovix.LinxDevolucaoRemanejoFabricaItem. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_devolucao_remanejo_fabrica_item (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxDevolucaoRemanejoFabricaItem
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_devolucao_remanejo_fabrica_item { get; private set; }
        public int? id_devolucao_remanejo_fabrica { get; private set; }
        public long? codigoproduto { get; private set; }
        public string? codigo_produto_franqueadora { get; private set; }
        public int? quantidade { get; private set; }
        public string? codebar { get; private set; }
        public string? serial { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxDevolucaoRemanejoFabricaItem() { }

        public LinxDevolucaoRemanejoFabricaItem(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Returns.LinxDevolucaoRemanejoFabricaItem dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_devolucao_remanejo_fabrica_item = MicrovixConverters.ToInt32(dto.id_devolucao_remanejo_fabrica_item);
            id_devolucao_remanejo_fabrica = MicrovixConverters.ToNullableInt32(dto.id_devolucao_remanejo_fabrica);
            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            codigo_produto_franqueadora = MicrovixConverters.ToNullableString(dto.codigo_produto_franqueadora, 50);
            quantidade = MicrovixConverters.ToNullableInt32(dto.quantidade);
            codebar = MicrovixConverters.ToNullableString(dto.codebar, 20);
            serial = MicrovixConverters.ToNullableString(dto.serial, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
