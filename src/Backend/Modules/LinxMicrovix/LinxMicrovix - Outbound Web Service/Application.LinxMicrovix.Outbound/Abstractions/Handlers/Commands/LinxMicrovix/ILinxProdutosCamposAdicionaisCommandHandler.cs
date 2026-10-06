using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxProdutosCamposAdicionaisCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<LinxProdutosCamposAdicionais> registros);
        string CreateInsertRecordQuery(string tableName);
    }
}
