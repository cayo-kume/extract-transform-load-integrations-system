using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxPlanosBandeirasValidator : AbstractValidator<LinxPlanosBandeiras>
    {
        public LinxPlanosBandeirasValidator()
        {
            RuleFor(x => x.plano).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.plano));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.bandeira).MaximumLength(30).WithMessage("O campo 'bandeira' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.bandeira));
            RuleFor(x => x.tipo_bandeira).MaximumLength(100).WithMessage("O campo 'tipo_bandeira' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.tipo_bandeira));
            RuleFor(x => x.adquirente).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.adquirente));
            RuleFor(x => x.nome_adquirente).MaximumLength(60).WithMessage("O campo 'nome_adquirente' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_adquirente));
            RuleFor(x => x.codigo_bandeira_sitef).MaximumLength(10).WithMessage("O campo 'codigo_bandeira_sitef' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_bandeira_sitef));
        }
    }
}
