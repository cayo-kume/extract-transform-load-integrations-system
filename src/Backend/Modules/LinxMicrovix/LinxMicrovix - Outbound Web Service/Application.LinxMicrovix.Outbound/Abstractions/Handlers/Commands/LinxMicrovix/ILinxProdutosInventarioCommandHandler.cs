using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxProdutosInventarioCommandHandler
    {
        string CreateGetProductsDepositsIdsQuery();
        string CreateGetRegistersExistsQuery(List<LinxProdutosInventario> registros);
        string CreateInsertRecordQuery(string tableName);
    }
}
