using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxCestValidator : AbstractValidator<LinxCest>
    {
        public LinxCestValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.id_cest).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_cest));
            RuleFor(x => x.descricao).MaximumLength(700).WithMessage("O campo 'descricao' deve ter no máximo 700 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.cest).MaximumLength(10).WithMessage("O campo 'cest' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.cest));
            RuleFor(x => x.id_segmento_mercadoria_bem).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_segmento_mercadoria_bem));
            RuleFor(x => x.ativo).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.ativo));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
