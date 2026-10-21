using System.ComponentModel.DataAnnotations;

namespace Domain.AfterSale.Dtos.Writes.Reverses.ReturnedInvoice.Request
{
    /// <summary>
    /// body of PUT v3/api/reverses/{id}/products/returned-invoice
    /// </summary>
    public class Root
    {
        /// <summary>
        /// ids of the reverse products that receive the returned invoice
        /// </summary>
        [Required, MinLength(1)]
        public List<long> id { get; init; } = [];

        [Required]
        public string returned_invoice { get; init; } = string.Empty;
    }
}
