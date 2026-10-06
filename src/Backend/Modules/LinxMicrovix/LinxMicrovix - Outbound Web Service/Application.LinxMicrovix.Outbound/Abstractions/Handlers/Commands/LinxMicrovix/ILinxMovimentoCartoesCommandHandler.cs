using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxMovimentoCartoesCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<LinxMovimentoCartoes> registros);
        string CreateInsertRecordQuery(string tableName);
    }
}
