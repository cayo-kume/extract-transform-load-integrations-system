using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxMovimentoTrocasCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<LinxMovimentoTrocas> registros);
        string CreateInsertRecordQuery(string tableName);
    }
}
