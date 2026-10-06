using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxProdutosTabelasPrecosCommandHandler : ILinxProdutosTabelasPrecosCommandHandler
    {
        public string CreateGetProductsTablesIdsQuery()
        {
            return "SELECT distinct id_tabela FROM [linx_microvix_erp].[LinxProdutosTabelas] where ativa = 'S'";
        }

        public string CreateGetRegistersExistsQuery(List<long?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CONCAT('[', CNPJ_EMP, ']', '|', '[', COD_PRODUTO, ']', '|', '[', ID_TABELA, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxProdutosTabelasPrecos] WHERE cod_produto IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}] 
                            ([lastupdateon],[cod_produto],[portal],[cnpj_emp],[id_tabela],[precovenda],[timestamp])
                            Values
                            (@lastupdateon,@cod_produto,@portal,@cnpj_emp,@id_tabela,@precovenda,@timestamp)";
        }
    }
}
