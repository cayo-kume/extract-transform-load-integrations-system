using System.Collections.Generic;

namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix
{
    public interface ILinxClientesEnderecosEntregaCommandHandler
    {
        string CreateGetRegistersExistsQuery(List<int?> registros);
        string CreateInsertRecordQuery(string tableName);
    }
}
