using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxUsuariosCommandHandler : ILinxUsuariosCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<int?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CONCAT('[', usuario_id, ']', '|', '[', usuario_doc, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxUsuarios] WHERE usuario_id IN ({identificadores})";
        }
    }
}
