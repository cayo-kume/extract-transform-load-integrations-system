namespace Domain.Shared.Abstractions.Api
{
    /// <summary>
    /// resposta de uma chamada em que o status HTTP importa (rotas de escrita): status, corpo e cabeçalhos (ex.: retry-after)
    /// </summary>
    public record ApiResponse(int StatusCode, string? Content, IReadOnlyDictionary<string, string> Headers)
    {
        public bool IsSuccess => StatusCode is >= 200 and < 300;
    }
}
