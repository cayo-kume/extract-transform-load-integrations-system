using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxClientesFornecClassesValidator : AbstractValidator<LinxClientesFornecClasses>
    {
        public LinxClientesFornecClassesValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cod_cliente).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_cliente));
            RuleFor(x => x.cod_classe).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_classe));
            RuleFor(x => x.nome_classe).MaximumLength(50).WithMessage("O campo 'nome_classe' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_classe));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
