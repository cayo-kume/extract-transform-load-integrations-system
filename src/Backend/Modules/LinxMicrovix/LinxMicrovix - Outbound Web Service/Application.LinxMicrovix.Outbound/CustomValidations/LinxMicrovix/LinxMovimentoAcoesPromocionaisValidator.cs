using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoAcoesPromocionaisValidator : AbstractValidator<LinxMovimentoAcoesPromocionais>
    {
        public LinxMovimentoAcoesPromocionaisValidator()
        {
            RuleFor(x => x.identificador).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.transacao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.transacao));
            RuleFor(x => x.id_acoes_promocionais).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_acoes_promocionais));
            RuleFor(x => x.desconto_item).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.desconto_item));
            RuleFor(x => x.quantidade).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.quantidade));
        }
    }
}
