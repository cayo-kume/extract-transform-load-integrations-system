using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class SellerCommandHandler : ISellerCommandHandler
    {
        public string CreateGetExistingSellersQuery(List<string?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT SELLERCODE, MAXCHANGEFILTERDATE FROM [TOTVS].[SELLERS] WHERE SELLERCODE IN ({identificadores})";
        }
    }
}
