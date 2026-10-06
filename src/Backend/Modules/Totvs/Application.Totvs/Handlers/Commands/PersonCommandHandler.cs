using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class PersonCommandHandler : IPersonCommandHandler
    {
        public string CreateGetExistingLegalEntitiesQuery(List<string?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CODE, MAXCHANGEFILTERDATE FROM [TOTVS].[LEGALENTITIES] WHERE CODE IN ({identificadores})";
        }

        public string CreateGetExistingCustomersQuery(List<string?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CODE, MAXCHANGEFILTERDATE FROM [TOTVS].[PEOPLE] WHERE CODE IN ({identificadores})";
        }

        public string CreateGetExistingBranchesQuery(List<string?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CODE, MAXCHANGEFILTERDATE FROM [TOTVS].[BRANCHES] WHERE CODE IN ({identificadores})";
        }
    }
}
