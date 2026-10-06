using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxProdutosSerialValidator : AbstractValidator<LinxProdutosSerial>
    {
        public LinxProdutosSerialValidator()
        {
            RuleFor(x => x.codigoproduto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigoproduto));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.serial).MaximumLength(50).WithMessage("O campo 'serial' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.serial));
            RuleFor(x => x.id_deposito).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_deposito));
            RuleFor(x => x.saldo).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.saldo));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
