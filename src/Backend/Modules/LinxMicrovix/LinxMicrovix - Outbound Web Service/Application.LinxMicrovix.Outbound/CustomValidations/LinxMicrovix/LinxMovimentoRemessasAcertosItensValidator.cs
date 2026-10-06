using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoRemessasAcertosItensValidator : AbstractValidator<LinxMovimentoRemessasAcertosItens>
    {
        public LinxMovimentoRemessasAcertosItensValidator()
        {
            RuleFor(x => x.id_remessas_acertos).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_remessas_acertos));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.transacao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.transacao));
            RuleFor(x => x.qtde_total).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.qtde_total));
            RuleFor(x => x.id_remessa_operacoes).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_remessa_operacoes));
            RuleFor(x => x.id_remessas_itens).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_remessas_itens));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}