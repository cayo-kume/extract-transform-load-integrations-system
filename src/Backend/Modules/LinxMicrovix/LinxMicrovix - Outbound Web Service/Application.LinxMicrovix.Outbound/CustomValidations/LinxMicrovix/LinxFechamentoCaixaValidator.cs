using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxFechamentoCaixaValidator : AbstractValidator<LinxFechamentoCaixa>
    {
        public LinxFechamentoCaixaValidator()
        {
            RuleFor(x => x.ativo).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.ativo));
            RuleFor(x => x.conferencia_efetuada).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.conferencia_efetuada));
            RuleFor(x => x.data).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.qtd_001).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtd_001));
            RuleFor(x => x.qtd_005).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtd_005));
            RuleFor(x => x.qtd_010).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtd_010));
            RuleFor(x => x.qtd_025).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtd_025));
            RuleFor(x => x.qtd_050).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtd_050));
            RuleFor(x => x.qtd_1).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtd_1));
            RuleFor(x => x.qtd_10).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtd_10));
            RuleFor(x => x.qtd_100).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtd_100));
            RuleFor(x => x.qtd_2).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtd_2));
            RuleFor(x => x.qtd_20).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtd_20));
            RuleFor(x => x.qtd_200).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtd_200));
            RuleFor(x => x.qtd_5).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtd_5));
            RuleFor(x => x.qtd_50).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtd_50));
            RuleFor(x => x.sangria).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.sangria));
            RuleFor(x => x.suprimentos).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.suprimentos));
            RuleFor(x => x.total_c_prazo).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.total_c_prazo));
            RuleFor(x => x.total_c_vista).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.total_c_vista));
            RuleFor(x => x.total_cartao).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.total_cartao));
            RuleFor(x => x.total_cartao_credito).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.total_cartao_credito));
            RuleFor(x => x.total_cartao_debito).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.total_cartao_debito));
            RuleFor(x => x.total_convenio).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.total_convenio));
            RuleFor(x => x.total_crediario).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.total_crediario));
            RuleFor(x => x.total_geral).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.total_geral));
            RuleFor(x => x.total_giftcard).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.total_giftcard));
            RuleFor(x => x.total_link_pagamento).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.total_link_pagamento));
            RuleFor(x => x.total_link_pagamento_credito).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.total_link_pagamento_credito));
            RuleFor(x => x.total_link_pagamento_debito).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.total_link_pagamento_debito));
            RuleFor(x => x.total_pix).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.total_pix));
            RuleFor(x => x.total_qr_linx).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.total_qr_linx));
            RuleFor(x => x.total_vale_compra).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.total_vale_compra));
            RuleFor(x => x.usuario).MaximumLength(10).WithMessage("O campo 'usuario' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.usuario));
            RuleFor(x => x.vale_compras_dev).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.vale_compras_dev));
        }
    }
}
