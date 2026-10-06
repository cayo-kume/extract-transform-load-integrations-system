using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxAntecipacoesFinanceirasPlanosValidator : AbstractValidator<LinxAntecipacoesFinanceirasPlanos>
    {
        public LinxAntecipacoesFinanceirasPlanosValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.id_antecipacoes_financeiras).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_antecipacoes_financeiras));
            RuleFor(x => x.numero_antecipacao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.numero_antecipacao));
            RuleFor(x => x.forma_pgto).MaximumLength(50).WithMessage("O campo 'forma_pgto' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.forma_pgto));
            RuleFor(x => x.plano).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.plano));
            RuleFor(x => x.nome_plano).MaximumLength(35).WithMessage("O campo 'nome_plano' deve ter no máximo 35 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_plano));
            RuleFor(x => x.valor).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.id_ordservprod).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_ordservprod));
            RuleFor(x => x.id_vendas_pos_produtos_tmp).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_vendas_pos_produtos_tmp));
            RuleFor(x => x.id_vendas_pos).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_vendas_pos));
            RuleFor(x => x.cancelado).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.cancelado));
            RuleFor(x => x.previsao_entrega).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.previsao_entrega));
            RuleFor(x => x.numero_ficha).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.numero_ficha));
            RuleFor(x => x.id_vendas_pos_produtos_campos_adicionais_tmp).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_vendas_pos_produtos_campos_adicionais_tmp));
            RuleFor(x => x.id_link_pagamento_linx_pay_hub).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_link_pagamento_linx_pay_hub));
            RuleFor(x => x.codigo_gerencial).MaximumLength(20).WithMessage("O campo 'codigo_gerencial' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_gerencial));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
        }
    }
}
