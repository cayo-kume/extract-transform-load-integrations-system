using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class SalesOrderCommandHandler : ISalesOrderCommandHandler
    {
        public string CreateGetExistingSalesOrdersQuery(List<string?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT ORDERCODE, MAXCHANGEFILTERDATE FROM [TOTVS].[SALEORDERS] WHERE ORDERCODE IN ({identificadores})";
        }
    }
}
