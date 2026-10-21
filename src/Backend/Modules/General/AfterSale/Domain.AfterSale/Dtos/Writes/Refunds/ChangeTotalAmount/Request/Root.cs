using System.ComponentModel.DataAnnotations;

namespace Domain.AfterSale.Dtos.Writes.Refunds.ChangeTotalAmount.Request
{
    /// <summary>
    /// body of POST v3/api/refunds/{id}/change-total-amount
    /// </summary>
    public class Root
    {
        //limites lidos com ponto decimal independente da cultura da máquina (pt-BR usa vírgula)
        [Required, Range(typeof(decimal), "0", "99999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
        public decimal? total_amount { get; init; }
    }
}
