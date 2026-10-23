namespace Domain.Totvs.Dtos.Reads.Authorization
{
    /// <summary>
    /// resposta do POST authorization/v2/token (fluxo password); não vai para o banco
    /// </summary>
    public class Token
    {
        public string? access_token { get; init; }
        public string? token_type { get; init; }
        public int expires_in { get; init; }
        public string? refresh_token { get; init; }
    }
}
