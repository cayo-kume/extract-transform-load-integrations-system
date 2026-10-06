using Domain.Stone.Entities;

namespace Application.Stone.Abstractions.Handlers.Commands
{
    public interface IStoneLogisticaCommandHandler
    {
        string CreateGetParametersQuery();
        string CreateGetExistingReferenceKeysQuery();
        string CreateGetRegistersExistsQuery();
        string CreateGetRegistersExistsQuery(List<Guid> registros);

        string CreateInsertZplsQuery(List<Zpl> zpls);
        string CreateInsertRecordQuery(string tableName);
    }
}
