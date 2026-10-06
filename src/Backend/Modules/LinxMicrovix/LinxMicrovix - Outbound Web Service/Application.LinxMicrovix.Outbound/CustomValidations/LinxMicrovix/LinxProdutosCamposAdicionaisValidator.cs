using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxProdutosCamposAdicionaisValidator : AbstractValidator<LinxProdutosCamposAdicionais>
    {
        public LinxProdutosCamposAdicionaisValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cod_produto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.cod_produto));
            RuleFor(x => x.campo).MaximumLength(30).WithMessage("O campo 'campo' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.campo));
            RuleFor(x => x.valor).MaximumLength(30).WithMessage("O campo 'valor' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.valor));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
