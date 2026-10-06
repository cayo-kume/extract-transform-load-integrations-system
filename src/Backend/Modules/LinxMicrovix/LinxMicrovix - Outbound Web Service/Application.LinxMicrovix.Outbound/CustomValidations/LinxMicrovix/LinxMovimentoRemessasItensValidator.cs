using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoRemessasItensValidator : AbstractValidator<LinxMovimentoRemessasItens>
    {
        public LinxMovimentoRemessasItensValidator()
        {
            RuleFor(x => x.id_remessas_itens).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_remessas_itens));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.id_remessas).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_remessas));
            RuleFor(x => x.transacao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.transacao));
            RuleFor(x => x.qtde_total).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.qtde_total));
            RuleFor(x => x.qtde_venda).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.qtde_venda));
            RuleFor(x => x.qtde_devolvido).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.qtde_devolvido));
            RuleFor(x => x.qtde_pendente).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.qtde_pendente));
            RuleFor(x => x.qtde_pendente_pagamento).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.qtde_pendente_pagamento));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}