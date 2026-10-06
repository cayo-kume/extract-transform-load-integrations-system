using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class AccountsPayableCommandHandler : IAccountsPayableCommandHandler
    {
        public string CreateGetExistingDuplicatesQuery(List<string?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT DUPLICATECODE, MAXCHANGEFILTERDATE FROM [TOTVS].[DUPLICATES] WHERE DUPLICATECODE IN ({identificadores})";
        }

        public string CreateGetExistingComissionsPaidQuery(List<string?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT COMMISSIONCODE, MAXCHANGEFILTERDATE FROM [TOTVS].[COMISSIONSPAID] WHERE COMMISSIONCODE IN ({identificadores})";
        }
    }
}
