using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoObservacaoCFValidator : AbstractValidator<LinxMovimentoObservacaoCF>
    {
        public LinxMovimentoObservacaoCFValidator()
        {
            RuleFor(x => x.identificador).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador));
            RuleFor(x => x.documento_cliente).MaximumLength(14).WithMessage("O campo 'documento_cliente' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.documento_cliente));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}