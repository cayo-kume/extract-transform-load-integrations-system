using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxSetoresCommandHandler : ILinxSetoresCommandHandler
    {
        public string CreateGetRegistersExistsQuery()
        {
            return "SELECT CONCAT('[', ID_SETOR, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxSetores]";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}] 
                            ([lastupdateon],[id_setor],[desc_setor],[timestamp],[codigo_integracao_ws],[ativo])
                            Values
                            (@lastupdateon,@id_setor,@desc_setor,@timestamp,@codigo_integracao_ws,@ativo)";
        }
    }
}
