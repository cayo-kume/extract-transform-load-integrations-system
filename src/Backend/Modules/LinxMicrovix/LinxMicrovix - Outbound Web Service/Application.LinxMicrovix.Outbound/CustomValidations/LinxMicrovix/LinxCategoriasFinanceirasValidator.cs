using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxCategoriasFinanceirasValidator : AbstractValidator<LinxCategoriasFinanceiras>
    {
        public LinxCategoriasFinanceirasValidator()
        {
            RuleFor(x => x.id_categorias_financeiras).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_categorias_financeiras));
            RuleFor(x => x.descricao).MaximumLength(50).WithMessage("O campo 'descricao' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.ativo).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.ativo));
            RuleFor(x => x.historico).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.historico));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
