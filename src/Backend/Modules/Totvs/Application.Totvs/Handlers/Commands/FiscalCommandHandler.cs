using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class FiscalCommandHandler : IFiscalCommandHandler
    {
        public string CreateGetExistingInvoicesQuery(List<string?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT INVOICESEQUENCE, MAXCHANGEFILTERDATE FROM [TOTVS].[INVOICES] WHERE INVOICESEQUENCE IN ({identificadores})";
        }
    }
}
