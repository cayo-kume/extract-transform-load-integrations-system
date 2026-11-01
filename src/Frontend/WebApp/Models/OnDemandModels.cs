namespace WebApp.Models
{
    /// <summary>
    /// Onde o parâmetro vai na chamada ao host, como no Swagger.
    /// </summary>
    public enum ParameterLocation
    {
        Header,
        Path,
        Query
    }

    /// <summary>
    /// Um parâmetro de uma operação sob demanda. Locked: preenchido pelo painel e travado (empresa logada e usuário).
    /// </summary>
    public sealed record OnDemandParameter(
        string Name,
        ParameterLocation Location,
        string Type,
        bool Required,
        string? Description,
        string? LockedValue = null)
    {
        public bool IsLocked => LockedValue is not null;
    }

    /// <summary>
    /// Uma integração sob demanda exposta pelo host WebApi: o job (IdJob, título e descrição), o verbo, a rota (usada na
    /// chamada, não mostrada na tela), a tag do Swagger e os parâmetros. HasBody: espera um corpo JSON, com BodyExample de
    /// exemplo e BodyModel com os campos.
    /// </summary>
    public sealed record OnDemandOperation(
        int IdApp,
        string AppTitle,
        int IdJob,
        string Title,
        string? Description,
        string Verb,
        string Route,
        string Tag,
        IReadOnlyList<OnDemandParameter> Parameters,
        bool HasBody,
        string? BodyExample,
        string? BodyModel)
    {
        /// <summary>
        /// GET só lê do sistema de origem e grava nas tabelas da integração; os outros verbos escrevem no sistema de origem.
        /// </summary>
        public bool Writes => Verb != "GET";
    }

    /// <summary>
    /// Resposta do host a uma chamada: status HTTP, corpo e a execução gravada em auditing.Logs, quando houve.
    /// </summary>
    public sealed record OnDemandResult(int StatusCode, string Reason, string Body, TimeSpan Duration, DateTime At, Guid? Execution)
    {
        public bool IsSuccess => StatusCode is >= 200 and < 300;
    }

    /// <summary>
    /// Uma chamada feita pela tela Sob Demanda: quem, quando, com que parâmetros e o resultado.
    /// </summary>
    public sealed record OnDemandCall(int IdJob, DateTime At, string User, string Parameters, int StatusCode, string Reason, Guid? Execution);
}
