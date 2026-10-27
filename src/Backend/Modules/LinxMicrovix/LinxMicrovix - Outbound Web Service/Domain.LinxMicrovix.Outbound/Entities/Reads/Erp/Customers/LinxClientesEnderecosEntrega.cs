using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers
{
    /// <summary>
    /// LinxClientesEnderecosEntrega, tabela linxmicrovix.LinxClientesEnderecosEntrega. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_endereco_entrega
    /// </summary>
    public class LinxClientesEnderecosEntrega
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_endereco_entrega { get; private set; }
        public int? cod_cliente { get; private set; }
        public string? endereco_cliente { get; private set; }
        public string? numero_rua_cliente { get; private set; }
        public string? complemento_end_cli { get; private set; }
        public string? cep_cliente { get; private set; }
        public string? bairro_cliente { get; private set; }
        public string? cidade_cliente { get; private set; }
        public string? uf_cliente { get; private set; }
        public string? descricao { get; private set; }
        public bool? principal { get; private set; }
        public string? fone_cliente { get; private set; }
        public string? fone_celular { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxClientesEnderecosEntrega() { }

        public LinxClientesEnderecosEntrega(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Customers.LinxClientesEnderecosEntrega dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_endereco_entrega = MicrovixConverters.ToInt32(dto.id_endereco_entrega);
            cod_cliente = MicrovixConverters.ToNullableInt32(dto.cod_cliente);
            endereco_cliente = MicrovixConverters.ToNullableString(dto.endereco_cliente, 250);
            numero_rua_cliente = MicrovixConverters.ToNullableString(dto.numero_rua_cliente, 20);
            complemento_end_cli = MicrovixConverters.ToNullableString(dto.complemento_end_cli, 60);
            cep_cliente = MicrovixConverters.ToNullableString(dto.cep_cliente, 9);
            bairro_cliente = MicrovixConverters.ToNullableString(dto.bairro_cliente, 60);
            cidade_cliente = MicrovixConverters.ToNullableString(dto.cidade_cliente, 40);
            uf_cliente = MicrovixConverters.ToNullableString(dto.uf_cliente, 2);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 250);
            principal = MicrovixConverters.ToNullableBoolean(dto.principal);
            fone_cliente = MicrovixConverters.ToNullableString(dto.fone_cliente, 20);
            fone_celular = MicrovixConverters.ToNullableString(dto.fone_celular, 20);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
