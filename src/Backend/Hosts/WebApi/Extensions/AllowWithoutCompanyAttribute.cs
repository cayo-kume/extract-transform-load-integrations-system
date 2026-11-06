namespace WebApi.Extensions
{
    /// <summary>
    /// rota que não roda job e não precisa de empresa (ex.: o token): o CompanyContextFilter não exige o X-Company e o Swagger
    /// não mostra o campo
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AllowWithoutCompanyAttribute : Attribute
    {
    }
}
