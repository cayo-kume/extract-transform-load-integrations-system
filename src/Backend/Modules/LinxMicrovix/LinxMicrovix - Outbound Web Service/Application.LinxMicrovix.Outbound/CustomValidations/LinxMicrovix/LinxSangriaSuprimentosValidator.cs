using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxSangriaSuprimentosValidator : AbstractValidator<LinxSangriaSuprimentos>
    {
        public LinxSangriaSuprimentosValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.usuario).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.usuario));
            RuleFor(x => x.data).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data));
            RuleFor(x => x.valor).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor));
            RuleFor(x => x.cancelado).MaximumLength(1).WithMessage("O campo 'cancelado' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.cancelado));
            RuleFor(x => x.conferido).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.conferido));
            RuleFor(x => x.cod_historico).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.cod_historico));
            RuleFor(x => x.desc_historico).MaximumLength(50).WithMessage("O campo 'desc_historico' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.desc_historico));
            RuleFor(x => x.id_sangria_suprimentos).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_sangria_suprimentos));
        }
    }
}