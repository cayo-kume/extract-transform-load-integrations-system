namespace Domain.Movidesk.Dtos.Writes
{
    /// <summary>
    /// resultado das rotas de escrita (tickets e pessoas), devolvido pelos services ao controller
    /// </summary>
    public class Result
    {
        public bool success { get; init; }

        // número do ticket ou Cód. Ref. da pessoa (no POST, o criado pela API)
        public string? id { get; init; }

        // status devolvido pela Movidesk; 0 quando a API nem foi chamada
        public int statusCode { get; init; }

        // mensagem da API ou do service
        public string? message { get; init; }

        // corpo devolvido pela Movidesk (ex.: detalhes dos anexos)
        public string? response { get; init; }

        // quando a Movidesk bloqueou por excesso de erros
        public int? retryAfterSeconds { get; init; }
    }
}
