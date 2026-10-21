using System.ComponentModel.DataAnnotations;

namespace Domain.AfterSale.Dtos.Writes.Reverses.AdministrativeRecords.Request
{
    /// <summary>
    /// body of POST v3/api/reverses/{id}/administrative-records
    /// </summary>
    public class Root
    {
        [Required]
        public string message { get; init; } = string.Empty;

        /// <summary>
        /// reverse or refund
        /// </summary>
        [AllowedValues(null, "reverse", "refund", ErrorMessage = "inserted_from_type deve ser reverse ou refund.")]
        public string? inserted_from_type { get; init; }

        public string? inserted_from_id { get; init; }
    }
}
