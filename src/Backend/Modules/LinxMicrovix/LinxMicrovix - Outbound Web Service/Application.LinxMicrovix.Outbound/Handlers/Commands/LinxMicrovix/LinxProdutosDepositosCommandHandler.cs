using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxProdutosDepositosCommandHandler : ILinxProdutosDepositosCommandHandler
    {
        public string CreateGetRegistersExistsQuery()
        {
            return "SELECT CONCAT('[', COD_DEPOSITO, ']', '|',  '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxProdutosDepositos]";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}] 
                            ([lastupdateon],[portal],[cod_deposito],[nome_deposito],[disponivel],[disponivel_transferencia],[timestamp])
                            Values
                            (@lastupdateon,@portal,@cod_deposito,@nome_deposito,@disponivel,@disponivel_transferencia,@timestamp)";
        }
    }
}
