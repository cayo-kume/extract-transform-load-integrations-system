using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Application.LinxMicrovix.Outbound.WebService.Abstractions.Services
{
    public interface ILinxMicrovixService
    {
        public string? BuildBodyRequest(LinxAPIParam jobParameter, string? parametersList, string? cnpj_emp);
        public string? BuildBodyRequest(LinxAPIParam jobParameter, string? parametersList);
        public string? BuildBodyRequest(LinxAPIParam jobParameter);
        public List<Dictionary<string?, string?>> DeserializeResponseToXML(LinxAPIParam jobParameter, string? response);
    }
}
