using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class ProductCommandHandler : IProductCommandHandler
    {
        public string CreateGetExistingSkusQuery(List<string?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT REFERENCEID, MAXCHANGEFILTERDATE FROM [TOTVS].[SKUS] WHERE REFERENCEID IN ({identificadores})";
        }
    }
}
