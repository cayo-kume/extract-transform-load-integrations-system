using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxPlanosValidator : AbstractValidator<LinxPlanos>
    {
        public LinxPlanosValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.plano).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.plano));
            RuleFor(x => x.desc_plano).MaximumLength(35).WithMessage("O campo 'desc_plano' deve ter no máximo 35 caracteres.").When(x => !string.IsNullOrEmpty(x.desc_plano));
            RuleFor(x => x.qtde_parcelas).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtde_parcelas));
            RuleFor(x => x.prazo_entre_parcelas).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.prazo_entre_parcelas));
            RuleFor(x => x.tipo_plano).MaximumLength(1).WithMessage("O campo 'tipo_plano' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.tipo_plano));
            RuleFor(x => x.indice_plano).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.indice_plano));
            RuleFor(x => x.cod_forma_pgto).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_forma_pgto));
            RuleFor(x => x.forma_pgto).MaximumLength(50).WithMessage("O campo 'forma_pgto' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.forma_pgto));
            RuleFor(x => x.tipo_transacao).MaximumLength(1).WithMessage("O campo 'tipo_transacao' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.tipo_transacao));
            RuleFor(x => x.taxa_financeira).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.taxa_financeira));
            RuleFor(x => x.dt_upd).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.dt_upd));
            RuleFor(x => x.desativado).MaximumLength(1).WithMessage("O campo 'desativado' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.desativado));
            RuleFor(x => x.usa_tef).MaximumLength(1).WithMessage("O campo 'usa_tef' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.usa_tef));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}