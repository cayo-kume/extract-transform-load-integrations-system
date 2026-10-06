using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxAcoesPromocionaisValidator : AbstractValidator<LinxAcoesPromocionais>
    {
        public LinxAcoesPromocionaisValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.id_acoes_promocionais).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_acoes_promocionais));
            RuleFor(x => x.descricao).MaximumLength(100).WithMessage("O campo 'descricao' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.vigencia_inicio).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.vigencia_inicio));
            RuleFor(x => x.vigencia_fim).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.vigencia_fim));
            RuleFor(x => x.ativa).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.ativa));
            RuleFor(x => x.excluida).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.excluida));
            RuleFor(x => x.integrada).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.integrada));
            RuleFor(x => x.qtde_integrada).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtde_integrada));
            RuleFor(x => x.valor_pago_franqueadora).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_pago_franqueadora));
        }
    }
}
