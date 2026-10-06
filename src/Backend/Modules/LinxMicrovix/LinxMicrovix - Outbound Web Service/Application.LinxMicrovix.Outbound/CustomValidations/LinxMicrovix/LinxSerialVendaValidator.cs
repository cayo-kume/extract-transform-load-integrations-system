using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxSerialVendaValidator : AbstractValidator<LinxSerialVenda>
    {
        public LinxSerialVendaValidator()
        {
            RuleFor(x => x.id_serial_venda).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_serial_venda));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.transacao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.transacao));
            RuleFor(x => x.codigoproduto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigoproduto));
            RuleFor(x => x.serial).MaximumLength(50).WithMessage("O campo 'serial' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.serial));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}