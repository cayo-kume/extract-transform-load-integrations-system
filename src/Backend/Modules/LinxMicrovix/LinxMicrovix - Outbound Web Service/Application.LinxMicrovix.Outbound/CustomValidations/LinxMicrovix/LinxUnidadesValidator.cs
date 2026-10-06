using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxUnidadesValidator : AbstractValidator<LinxUnidades>
    {
        public LinxUnidadesValidator()
        {
            RuleFor(x => x.idUnidade).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.idUnidade));
            RuleFor(x => x.unidade).MaximumLength(50).WithMessage("O campo 'unidade' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.unidade));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.descricao).MaximumLength(200).WithMessage("O campo 'descricao' deve ter no máximo 200 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
        }
    }
}