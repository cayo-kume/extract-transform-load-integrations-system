using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands
{
    public class LinxClientesEnderecosEntregaCommandHandler : ILinxClientesEnderecosEntregaCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<int?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CONCAT('[', ID_ENDERECO_ENTREGA, ']', '|', '[', COD_CLIENTE, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxClientesEnderecosEntrega] WHERE ID_ENDERECO_ENTREGA IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}]
                            (lastupdateon,id_endereco_entrega,cod_cliente,endereco_cliente,numero_rua_cliente,complemento_end_cli,cep_cliente,bairro_cliente,
                             cidade_cliente,uf_cliente,descricao,principal,fone_cliente,fone_celular,timestamp,portal)
                            Values
                            (@lastupdateon,@id_endereco_entrega,@cod_cliente,@endereco_cliente,@numero_rua_cliente,@complemento_end_cli,@cep_cliente,@bairro_cliente,
                             @cidade_cliente,@uf_cliente,@descricao,@principal,@fone_cliente,@fone_celular,@timestamp,@portal)";
        }
    }
}
