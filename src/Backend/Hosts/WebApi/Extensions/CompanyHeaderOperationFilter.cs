using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Extensions
{
    /// <summary>
    /// o cabeçalho X-Company no Swagger, obrigatório nas rotas sem {docCompany} (as cargas): sem ele, a execução não tem
    /// empresa e o CompanyContextFilter responde 400. Fica de fora quem tem [AllowWithoutCompany]. Swagger:DefaultCompany, no
    /// appsettings, preenche o campo para não digitar o CNPJ a cada chamada. Nas rotas com {docCompany}, o parâmetro da rota
    /// ganha a descrição de onde a empresa está cadastrada
    /// </summary>
    public class CompanyHeaderOperationFilter : IOperationFilter
    {
        private readonly string? _defaultCompany;

        public CompanyHeaderOperationFilter(IConfiguration configuration) =>
            _defaultCompany = configuration["Swagger:DefaultCompany"] is { Length: > 0 } company ? company.Trim() : null;

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
            var docCompany = operation.Parameters?.OfType<OpenApiParameter>().FirstOrDefault(x => string.Equals(x.Name, "docCompany", StringComparison.OrdinalIgnoreCase));

            if (docCompany is not null)
            {
                docCompany.Description = "CNPJ da empresa, cadastrado em auditing.Companies, só dígitos";
                return;
            }

            if (metadata.OfType<AllowWithoutCompanyAttribute>().Any())
                return;

            operation.Parameters ??= [];
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = CompanyContextFilter.CompanyHeader,
                In = ParameterLocation.Header,
                Required = true,
                Description = "CNPJ da empresa da execução, cadastrado em auditing.Companies, só dígitos. No DataWarehouse e nos Hosts, o da holding",
                Schema = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Pattern = @"^\d{14}$",
                    Default = _defaultCompany is null ? null : JsonValue.Create(_defaultCompany)
                }
            });
        }
    }
}
