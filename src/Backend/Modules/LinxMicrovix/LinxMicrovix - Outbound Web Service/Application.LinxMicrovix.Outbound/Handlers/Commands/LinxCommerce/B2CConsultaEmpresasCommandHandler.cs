using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands
{
    public class B2CConsultaEmpresasCommandHandler : IB2CConsultaEmpresasCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<string> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CONCAT('[', EMPRESA, ']', '|', '[', CNPJ_EMP, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_commerce].[B2CCONSULTAEMPRESAS] WHERE CNPJ_EMP IN ({identificadores})";
        }
    }
}
