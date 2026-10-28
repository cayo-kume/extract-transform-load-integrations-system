using Domain.Totvs.Dtos.Writes.Products.BarCodes;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.BarCodes
{
    public static class BarCodeRules
    {
        public static bool Unique(IEnumerable<string?> barCodes) =>
            barCodes.Where(b => !String.IsNullOrEmpty(b)).GroupBy(b => b!.Trim()).All(g => g.Count() == 1);
    }
}
