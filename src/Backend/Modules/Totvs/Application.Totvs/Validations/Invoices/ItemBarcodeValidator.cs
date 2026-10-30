using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Invoices;
using FluentValidation;

namespace Application.Totvs.Validations.Invoices
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Fiscal onde ele informa, senão os conferidos numa amostra real
    /// (colunas do Script023_Create_Totvs_Invoices_Tables)
    /// </summary>
    public class ItemBarcodeValidator : AbstractValidator<ItemBarcode>
    {
        public ItemBarcodeValidator()
        {
            RuleFor(x => x.barcode)
                .MaximumLength(60)
                .WithMessage("O campo 'barcode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.barcode));
        }
    }
}
