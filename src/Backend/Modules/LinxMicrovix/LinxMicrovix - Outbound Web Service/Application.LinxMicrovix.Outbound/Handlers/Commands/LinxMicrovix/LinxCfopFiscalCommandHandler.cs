using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands
{
    public class LinxCfopFiscalCommandHandler : ILinxCfopFiscalCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<int> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CONCAT('[', id_cfop_fiscal, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxCfopFiscal] WHERE id_cfop_fiscal IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}]
                            ([lastupdateon],[portal],[id_cfop_fiscal],[cfop_fiscal],[descricao],[excluido],[timestamp])
                            Values
                            (@lastupdateon,@portal,@id_cfop_fiscal,@cfop_fiscal,@descricao,@excluido,@timestamp)";
        }
    }
}
