using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxMovimentoPlanosCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<LinxMovimentoPlanos> registros);
        string CreateInsertRecordQuery(string tableName);
    }
}
