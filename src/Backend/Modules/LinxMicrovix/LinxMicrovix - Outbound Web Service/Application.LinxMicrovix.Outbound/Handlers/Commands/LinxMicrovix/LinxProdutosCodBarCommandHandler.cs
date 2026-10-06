using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxProdutosCodBarCommandHandler : ILinxProdutosCodBarCommandHandler
    {
        public string CreateGetProductsSetorIdsQuery()
        {
            return "SELECT distinct id_setor FROM [linx_microvix_erp].[LinxSetores]";
        }

        public string CreateGetRegistersExistsQuery(List<long?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CONCAT('[', COD_PRODUTO, ']', '|', '[', COD_BARRA, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxProdutosCodBar] WHERE cod_produto IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}] 
                          ([lastupdateon], [portal], [cod_produto], [cod_barra], [timestamp]) 
                          Values 
                          (@lastupdateon, @portal, @cod_produto, @cod_barra, @timestamp)";
        }
    }
}
