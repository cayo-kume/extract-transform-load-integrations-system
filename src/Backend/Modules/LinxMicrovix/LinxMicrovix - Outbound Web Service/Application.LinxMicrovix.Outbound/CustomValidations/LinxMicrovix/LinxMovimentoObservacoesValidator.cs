using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoObservacoesValidator : AbstractValidator<LinxMovimentoObservacoes>
    {
        public LinxMovimentoObservacoesValidator()
        {
            RuleFor(x => x.id_obs).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_obs));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.titulo_obs).MaximumLength(50).WithMessage("O campo 'titulo_obs' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.titulo_obs));
            RuleFor(x => x.descricao_obs).MaximumLength(250).WithMessage("O campo 'descricao_obs' deve ter no máximo 250 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao_obs));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}