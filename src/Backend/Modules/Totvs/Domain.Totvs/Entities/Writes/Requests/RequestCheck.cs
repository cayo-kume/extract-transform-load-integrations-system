namespace Domain.Totvs.Entities.Writes.Requests
{
    /// <summary>
    /// chamada anterior em totvs.TotvsRequests lida pela trava de repetição das escritas (T5 do docs/PLANO-TOTVS-METODOS-FALTANTES.md)
    /// </summary>
    public class RequestCheck
    {
        public DateTime request_date { get; set; }
        public string? request_body { get; set; }
        public bool is_success { get; set; }
        public int? status_code { get; set; }
    }
}
