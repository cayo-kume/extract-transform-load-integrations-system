using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxCommerce
{
    public class B2CConsultaStatusCommandHandler : IB2CConsultaStatusCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<int?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CONCAT('[', ID_STATUS, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_commerce].[B2CCONSULTASTATUS] WHERE ID_STATUS IN ({identificadores})";
        }
    }
}
