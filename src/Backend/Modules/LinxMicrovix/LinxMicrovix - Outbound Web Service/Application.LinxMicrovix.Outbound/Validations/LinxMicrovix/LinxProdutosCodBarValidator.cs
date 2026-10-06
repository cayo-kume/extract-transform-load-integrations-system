using Domain.Core.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.Validations.LinxMicrovix
{
    public class LinxProdutosCodBarValidator : AbstractValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosCodBar>
    {
        public LinxProdutosCodBarValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cod_produto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.cod_produto));
            RuleFor(x => x.cod_barra).MaximumLength(20).WithMessage("O campo 'cod_barra' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.cod_barra));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
