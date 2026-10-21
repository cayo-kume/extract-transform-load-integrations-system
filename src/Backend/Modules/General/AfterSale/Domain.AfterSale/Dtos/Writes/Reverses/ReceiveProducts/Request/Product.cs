using System.ComponentModel.DataAnnotations;

namespace Domain.AfterSale.Dtos.Writes.Reverses.ReceiveProducts.Request
{
    public class Product
    {
        public long? id { get; init; }
        public int? qty { get; init; }
        public string? product_received_comment { get; init; }

        [AllowedValues(null, "repack", "recondition", "discard", "sale", ErrorMessage = "situation deve ser repack, recondition, discard ou sale.")]
        public string? situation { get; init; }

        [AllowedValues(null, "free_shipping", "used_without_defect", "wrong_package", "others", ErrorMessage = "fraud deve ser free_shipping, used_without_defect, wrong_package ou others.")]
        public string? fraud { get; init; }

        public long? reason_id { get; init; }
    }
}
