using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxCentroCustoValidator : AbstractValidator<LinxCentroCusto>
    {
        public LinxCentroCustoValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.CNPJ).MaximumLength(14).WithMessage("O campo 'CNPJ' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.CNPJ));
            RuleFor(x => x.id_centrocusto).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_centrocusto));
            RuleFor(x => x.nome_centrocusto).MaximumLength(50).WithMessage("O campo 'nome_centrocusto' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_centrocusto));
            RuleFor(x => x.ativo).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.ativo));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
