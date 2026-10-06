using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxProdutosCamposAdicionaisCommandHandler : ILinxProdutosCamposAdicionaisCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<LinxProdutosCamposAdicionais> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r.cod_produto}'"));
            return $"SELECT CONCAT('[', COD_PRODUTO, ']', '|', '[', CAMPO, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxProdutosCamposAdicionais] WHERE COD_PRODUTO IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}] 
                            ([lastupdateon],[portal],[cod_produto],[campo],[valor],[timestamp])
                            Values
                            (@lastupdateon,@portal,@cod_produto,@campo,@valor,@timestamp)";
        }
    }
}
