using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxProdutosDetalhesCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<LinxProdutosDetalhes> registros);
        string CreateGetPendingRegistersQuery();
        string CreateInsertRecordQuery(string tableName);
    }
}
