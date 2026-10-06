using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxMovimentoCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<LinxMovimento> registros);
        string CreateInsertRecordQuery(string tableName);
    }
}
