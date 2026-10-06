using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class PurchaseOrderCommandHandler : IPurchaseOrderCommandHandler
    {
        public string CreateGetExistingPurchaseOrdersQuery(List<string?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT ORDERCODE, MAXCHANGEFILTERDATE FROM [TOTVS].[PURCHASEORDERS] WHERE ORDERCODE IN ({identificadores})";
        }
    }
}
