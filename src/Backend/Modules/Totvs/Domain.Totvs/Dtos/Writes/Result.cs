namespace Domain.Totvs.Dtos.Writes
{
    /// <summary>
    /// resultado das rotas de escrita, devolvido pelos services ao controller (docs/PLANO-TOTVS-METODOS-FALTANTES.md §1.1)
    /// </summary>
    public class Result
    {
        public bool success { get; init; }

        // chave do registro escrito, a mesma gravada em totvs.TotvsRequests (field_key_value)
        public string? key { get; init; }

        // status devolvido pela Totvs; 0 quando a API nem foi chamada (empresa, loja, configuração) ou quando a chamada lançou exceção
        public int statusCode { get; init; }

        // a rota da Totvs foi chamada: sem resposta (statusCode 0) ou com 5xx, a escrita pode ter sido feita
        public bool apiCalled { get; init; }

        // mensagem da Totvs (DomainNotificationMessage) ou do service
        public string? message { get; init; }

        // corpo devolvido pela Totvs
        public string? response { get; init; }

        // quando a Totvs respondeu 429 também na nova tentativa
        public int? retryAfterSeconds { get; init; }
    }
}
