namespace Domain.AfterSale.Dtos.Writes.Base
{
    /// <summary>
    /// common fields of the write routes responses (reverses and refunds).
    /// the error fields are not in the API documentation, but are returned when success is false.
    /// </summary>
    public abstract class ResponseRoot
    {
        public bool success { get; init; }
        public string? message { get; init; }

        public string? error { get; init; }
        public string? error_code { get; init; }
        public string? message_translated { get; init; }
    }
}
