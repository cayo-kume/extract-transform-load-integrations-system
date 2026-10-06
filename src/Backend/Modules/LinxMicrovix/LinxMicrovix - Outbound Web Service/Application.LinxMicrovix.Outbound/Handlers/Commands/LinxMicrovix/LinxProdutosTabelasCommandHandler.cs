using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxProdutosTabelasCommandHandler : ILinxProdutosTabelasCommandHandler
    {
        public string CreateGetRegistersExistsQuery()
        {
            return "SELECT CONCAT('[', CNPJ_EMP, ']', '|', '[', ID_TABELA, ']', '|', '[', TIPO_TABELA, ']', '|',  '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxProdutosTabelas]";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}] 
                            ([lastupdateon],[id_tabela],[portal],[cnpj_emp],[nome_tabela],[ativa],[timestamp],[tipo_tabela],[codigo_integracao_ws])
                            Values
                            (@lastupdateon,@id_tabela,@portal,@cnpj_emp,@nome_tabela,@ativa,@timestamp,@tipo_tabela,@codigo_integracao_ws)";
        }
    }
}
