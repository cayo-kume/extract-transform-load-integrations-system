using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxRotinaOrigemCommandHandler : ILinxRotinaOrigemCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<int?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CONCAT('[', codigo_rotina, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxRotinaOrigem] WHERE codigo_rotina IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}]
                            ([lastupdateon],[codigo_rotina],[portal],[descricao_rotina],[timestamp])
                            Values
                            (@lastupdateon,@codigo_rotina,@portal,@descricao_rotina,@timestamp)";
        }
    }
}
