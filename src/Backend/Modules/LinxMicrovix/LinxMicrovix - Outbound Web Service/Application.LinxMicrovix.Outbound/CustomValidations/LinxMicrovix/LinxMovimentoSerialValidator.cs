using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoSerialValidator : AbstractValidator<LinxMovimentoSerial>
    {
        public LinxMovimentoSerialValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.identificador).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador));
            RuleFor(x => x.transacao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.transacao));
            RuleFor(x => x.serial).MaximumLength(50).WithMessage("O campo 'serial' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.serial));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
